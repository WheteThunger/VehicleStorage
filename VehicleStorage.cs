﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Vehicle Storage", "WhiteThunder", "3.1.1")]
    [Description("Allows adding storage containers to vehicles and increasing built-in storage capacity.")]
    internal class VehicleStorage : CovalencePlugin
    {
        #region Fields

        [PluginReference]
        private Plugin CargoTrainEvent;

        private Configuration _pluginConfig;

        private const string BasePermissionPrefix = "vehiclestorage";

        private const string RhibStoragePrefab = "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab";
        private const string HabStoragePrefab = "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab";

        private const string StashDeployEffectPrefab = "assets/prefabs/deployable/small stash/effects/small-stash-deploy.prefab";
        private const string BoxDeployEffectPrefab = "assets/prefabs/deployable/woodenbox/effects/wooden-box-deploy.prefab";

        private const string ResizableLootPanelName = "generic_resizable";
        private const int MaximumCapacity = 42;

        private readonly Dictionary<VehicleConfig, HashSet<BaseEntity>> _allSupportedVehicles = new Dictionary<VehicleConfig, HashSet<BaseEntity>>();

        #endregion

        #region Hooks

        private void Init()
        {
            Unsubscribe(nameof(OnEntitySpawned));
        }

        private void OnServerInitialized()
        {
            _pluginConfig.Init(this);

            foreach (var networkable in BaseNetworkable.serverEntities)
            {
                var baseEntity = networkable as BaseEntity;
                if (baseEntity == null)
                    continue;

                OnEntitySpawned(baseEntity);
            }

            Subscribe(nameof(OnEntitySpawned));
        }

        private void OnEntitySpawned(BaseEntity entity)
        {
            var vehicleConfig = _pluginConfig.GetVehicleConfig(entity);
            if (vehicleConfig == null)
                return;

            HashSet<BaseEntity> vehicleList;
            if (!_allSupportedVehicles.TryGetValue(vehicleConfig, out vehicleList))
            {
                vehicleList = new HashSet<BaseEntity>();
                _allSupportedVehicles[vehicleConfig] = vehicleList;
            }

            vehicleList.Add(entity);

            // Wait 2 ticks to give Vehicle Vendor Options an opportunity to set ownership.
            NextTick(() => NextTick(() =>
            {
                if (entity == null)
                    return;

                RefreshVehicleStorage(entity, vehicleConfig);
            }));
        }

        private void OnEntityKill(BaseEntity entity)
        {
            var vehicleConfig = _pluginConfig.GetVehicleConfig(entity);
            if (vehicleConfig == null)
                return;

            HashSet<BaseEntity> vehicleList;
            if (_allSupportedVehicles.TryGetValue(vehicleConfig, out vehicleList))
            {
                vehicleList.Remove(entity);

                foreach (var child in entity.children)
                {
                    var container = child as StorageContainer;
                    if (container == null)
                        continue;

                    if (vehicleConfig.ContainerPresets?.ContainsKey(container.name) ?? false)
                        container.DropItems();
                }
            }
        }

        private void OnUserPermissionGranted(string userId, string perm)
        {
            if (perm.StartsWith(BasePermissionPrefix))
                HandlePermissionChanged(userId);
        }

        private void OnGroupPermissionGranted(string group, string perm)
        {
            if (perm.StartsWith(BasePermissionPrefix))
                HandlePermissionChanged();
        }

        private void OnUserGroupAdded(string userId, string groupName)
        {
            var permList = permission.GetGroupPermissions(groupName, parents: true);
            foreach (var perm in permList)
            {
                if (perm.StartsWith(BasePermissionPrefix))
                {
                    HandlePermissionChanged(userId);
                    return;
                }
            }
        }

        private void OnRidableAnimalClaimed(RidableHorse horse, BasePlayer player)
        {
            RefreshVehicleStorage(horse);
        }

        // Compatibility with plugin: Claim Vehicle Ownership
        private void OnVehicleOwnershipChanged(BaseEntity vehicle) => RefreshVehicleStorage(vehicle);

        #endregion

        #region Dependencies

        private bool IsCargoTrain(BaseEntity entity)
        {
            var workcart = entity as TrainEngine;
            if (workcart == null)
                return false;

            var result = CargoTrainEvent?.Call("IsTrainSpecial", workcart.net.ID);
            return result is bool && (bool)result;
        }

        #endregion

        #region API

        private void API_RefreshVehicleStorage(BaseEntity vehicle)
        {
            RefreshVehicleStorage(vehicle);
        }

        #endregion

        #region Exposed Hooks

        private bool AlterStorageWasBlocked(BaseEntity vehicle)
        {
            object hookResult = Interface.CallHook("OnVehicleStorageUpdate", vehicle);
            if (hookResult is bool && (bool)hookResult == false)
                return true;

            if (IsCargoTrain(vehicle))
                return true;

            return false;
        }

        private bool SpawnStorageWasBlocked(BaseEntity vehicle)
        {
            object hookResult = Interface.CallHook("OnVehicleStorageSpawn", vehicle);
            return hookResult is bool && (bool)hookResult == false;
        }

        private void CallHookVehicleStorageSpawned(BaseEntity vehicle, StorageContainer container)
        {
            Interface.CallHook("OnVehicleStorageSpawned", vehicle, container);
        }

        #endregion

        #region Helper Methods

        private void RemoveProblemComponents(BaseEntity entity)
        {
            foreach (var meshCollider in entity.GetComponentsInChildren<MeshCollider>())
                UnityEngine.Object.DestroyImmediate(meshCollider);

            UnityEngine.Object.DestroyImmediate(entity.GetComponent<DestroyOnGroundMissing>());
            UnityEngine.Object.DestroyImmediate(entity.GetComponent<GroundWatch>());
        }

        private void SetupStorage(StorageContainer container, ContainerPreset preset)
        {
            container.pickup.enabled = false;
            container.dropsLoot = true;
            RemoveProblemComponents(container);
        }

        private StorageContainer SpawnStorage(BaseEntity vehicle, ContainerPreset preset, int capacity)
        {
            if (SpawnStorageWasBlocked(vehicle))
                return null;

            var createdEntity = GameManager.server.CreateEntity(preset.Prefab, preset.Position, preset.Rotation);
            if (createdEntity == null)
                return null;

            var container = createdEntity as StorageContainer;
            if (container == null)
            {
                UnityEngine.Object.Destroy(createdEntity);
                return null;
            }

            container.name = preset.Name;
            SetupStorage(container, preset);
            container.SetParent(vehicle, preset.ParentBone);
            container.Spawn();
            MaybeIncreaseCapacity(container, capacity);
            CallHookVehicleStorageSpawned(vehicle, container);

            var deployEffect = preset.Prefab == RhibStoragePrefab
                ? BoxDeployEffectPrefab
                : preset.Prefab == HabStoragePrefab
                ? StashDeployEffectPrefab
                : null;

            if (deployEffect != null)
                Effect.server.Run(deployEffect, container.transform.position);

            return container;
        }

        private StorageContainer FindStorageContainerForPreset(BaseEntity entity, ContainerPreset preset)
        {
            foreach (var child in entity.children)
            {
                var container = child as StorageContainer;
                if (container == null)
                    continue;

                if (container.PrefabName == preset.Prefab
                    && (container.transform.localPosition == preset.Position || container.name == preset.Name))
                    return container;
            }

            return null;
        }

        private void MaybeIncreaseCapacity(StorageContainer container, int capacity)
        {
            container.panelName = ResizableLootPanelName;

            // Don't decrease capacity, in case there are items in those slots.
            // It's possible to handle that better, but not a priority as of writing this.
            if (capacity != -1 && container.inventory.capacity < capacity)
                container.inventory.capacity = capacity;
        }

        private void AddOrUpdateExtraContainers(BaseEntity vehicle, VehicleProfile vehicleProfile)
        {
            foreach (var entry in vehicleProfile.ValidAdditionalStorage)
            {
                var containerPreset = entry.Key;
                var capacity = entry.Value;

                var container = FindStorageContainerForPreset(vehicle, containerPreset);
                if (container == null)
                {
                    container = SpawnStorage(vehicle, containerPreset, capacity);
                }
                else
                {
                    SetupStorage(container, containerPreset);
                    MaybeIncreaseCapacity(container, capacity);
                    var transform = container.transform;
                    if (transform.localPosition != containerPreset.Position || transform.localRotation != containerPreset.Rotation)
                    {
                        transform.localPosition = containerPreset.Position;
                        transform.localRotation = containerPreset.Rotation;
                        container.InvalidateNetworkCache();
                        container.SendNetworkUpdate_Position();
                    }
                }
            }
        }

        private void RefreshVehicleStorage(BaseEntity vehicle, VehicleConfig vehicleConfig)
        {
            var vehicleProfile = vehicleConfig.GetProfileForVehicle(permission, vehicle);
            if (vehicleProfile == null)
            {
                // Probably not a supported vehicle.
                return;
            }

            if (AlterStorageWasBlocked(vehicle))
            {
                // Another plugin blocked altering storage.
                return;
            }

            var defaultContainer = vehicleConfig.GetDefaultContainer(vehicle);
            if (defaultContainer != null)
                MaybeIncreaseCapacity(defaultContainer, vehicleProfile.BuiltInStorageCapacity);

            if (vehicleProfile.ValidAdditionalStorage != null)
                AddOrUpdateExtraContainers(vehicle, vehicleProfile);
        }

        private void RefreshVehicleStorage(BaseEntity vehicle)
        {
            if (vehicle is ModularCar)
            {
                foreach (var child in vehicle.children)
                {
                    var module = child as BaseVehicleModule;
                    if (module != null)
                    {
                        RefreshVehicleStorage(module);
                    }
                }
                return;
            }

            var vehicleConfig = _pluginConfig.GetVehicleConfig(vehicle);
            if (vehicleConfig == null)
                return;

            RefreshVehicleStorage(vehicle, vehicleConfig);
        }

        private void HandlePermissionChanged(string userIdString = "")
        {
            foreach (var entry in _allSupportedVehicles)
            {
                var vehicleConfig = entry.Key;
                var vehicleList = entry.Value;

                foreach (var vehicle in vehicleList)
                {
                    var ownerId = vehicleConfig.GetOwnerId(vehicle);
                    if (ownerId == 0)
                    {
                        // Unowned vehicles cannot be affected by permissions so there's nothing to do.
                        continue;
                    }

                    if (userIdString != string.Empty && userIdString != vehicle.OwnerID.ToString())
                    {
                        // Permissions changed for a specific player, but they don't own the vehicle, so nothing to do.
                        continue;
                    }

                    RefreshVehicleStorage(vehicle, vehicleConfig);
                }
            }
        }

        #endregion

        #region Configuration

        private class ContainerPreset
        {
            [JsonProperty("Prefab")]
            public string Prefab;

            [JsonProperty("Position")]
            public Vector3 Position;

            [JsonProperty("RotationAngles", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Vector3 RotationAngles;

            [JsonProperty("ParentBone", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string ParentBone;

            [JsonIgnore]
            private Quaternion? _rotation;

            [JsonIgnore]
            public Quaternion Rotation
            {
                get
                {
                    if (_rotation == null)
                        _rotation = Quaternion.Euler(RotationAngles);

                    return (Quaternion)_rotation;
                }
            }

            [JsonIgnore]
            public string Name;
        }

        private class VehicleProfile
        {
            [JsonProperty("PermissionSuffix", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string PermissionSuffix;

            [JsonProperty("BuiltInStorageCapacity", DefaultValueHandling = DefaultValueHandling.Ignore)]
            [DefaultValue(-1)]
            public int BuiltInStorageCapacity = -1;

            [JsonProperty("AdditionalStorage", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Dictionary<string, int> AdditionalStorage;

            [JsonIgnore]
            public Dictionary<ContainerPreset, int> ValidAdditionalStorage { get; private set; }

            [JsonIgnore]
            public string Permission { get; private set; }

            public void Init(VehicleStorage pluginInstance, VehicleConfig vehicleConfig)
            {
                if (PermissionSuffix != null)
                {
                    Permission = $"{BasePermissionPrefix}.{vehicleConfig.VehicleType}.{PermissionSuffix}";
                    pluginInstance.permission.RegisterPermission(Permission, pluginInstance);
                }

                if (AdditionalStorage != null && AdditionalStorage.Count != 0)
                {
                    ValidAdditionalStorage = new Dictionary<ContainerPreset, int>(AdditionalStorage.Count);

                    foreach (var presetEntry in AdditionalStorage)
                    {
                        var presetName = presetEntry.Key;
                        var capacity = presetEntry.Value;

                        ContainerPreset preset;
                        if (!vehicleConfig.ContainerPresets.TryGetValue(presetName, out preset))
                        {
                            pluginInstance.LogError($"Storage preset {vehicleConfig.VehicleType} -> \"{presetName}\" does not exist.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(preset.Prefab))
                        {
                            pluginInstance.LogError($"Missing prefab for preset {vehicleConfig.VehicleType} -> \"{presetName}\".");
                            continue;
                        }

                        preset.Name = presetName;
                        ValidAdditionalStorage[preset] = capacity;
                    }
                }
            }
        }

        private abstract class VehicleConfig
        {
            [JsonProperty("DefaultProfile")]
            public VehicleProfile DefaultProfile = new VehicleProfile();

            [JsonProperty("ProfilesRequiringPermission")]
            public VehicleProfile[] ProfilesRequiringPermission;

            [JsonProperty("ContainerPresets", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public Dictionary<string, ContainerPreset> ContainerPresets;

            [JsonIgnore]
            public abstract string VehicleType { get; }

            [JsonIgnore]
            public abstract string PrefabPath { get; }

            public virtual StorageContainer GetDefaultContainer(BaseEntity enitty) => null;

            public virtual ulong GetOwnerId(BaseEntity entity) => entity.OwnerID;

            public void Init(VehicleStorage pluginInstance)
            {
                if (ProfilesRequiringPermission == null)
                    return;

                DefaultProfile.Init(pluginInstance, this);

                foreach (var profile in ProfilesRequiringPermission)
                    profile.Init(pluginInstance, this);
            }

            public VehicleProfile GetProfileForVehicle(Permission permissionSystem, BaseEntity vehicle)
            {
                var ownerId = GetOwnerId(vehicle);
                if (ownerId == 0 || (ProfilesRequiringPermission?.Length ?? 0) == 0)
                    return DefaultProfile;

                var ownerIdString = ownerId.ToString();

                for (var i = ProfilesRequiringPermission.Length - 1; i >= 0; i--)
                {
                    var profile = ProfilesRequiringPermission[i];
                    if (profile.Permission != null && permissionSystem.UserHasPermission(ownerIdString, profile.Permission))
                        return profile;
                }

                return DefaultProfile;
            }
        }

        private class ChinookConfig : VehicleConfig
        {
            public override string VehicleType => "chinook";
            public override string PrefabPath => "assets/prefabs/npc/ch47/ch47.entity.prefab";
        }

        private class DuoSubmarineConfig : SoloSubmarineConfig
        {
            public override string VehicleType => "duosub";
            public override string PrefabPath => "assets/content/vehicles/submarine/submarineduo.entity.prefab";
        }

        private class HotAirBalloonConfig : VehicleConfig
        {
            public override string VehicleType => "hotairballoon";
            public override string PrefabPath => "assets/prefabs/deployable/hot air balloon/hotairballoon.prefab";

            public override StorageContainer GetDefaultContainer(BaseEntity entity) =>
                (entity as HotAirBalloon)?.storageUnitInstance.Get(serverside: true) as StorageContainer;
        }

        private class KayakConfig : VehicleConfig
        {
            public override string VehicleType => "kayak";
            public override string PrefabPath => "assets/content/vehicles/boats/kayak/kayak.prefab";
        }

        private class MagnetCraneConfig : VehicleConfig
        {
            public override string VehicleType => "magnetcrane";
            public override string PrefabPath => "assets/content/vehicles/crane_magnet/magnetcrane.entity.prefab";
        }

        private class MinicopterConfig : VehicleConfig
        {
            public override string VehicleType => "minicopter";
            public override string PrefabPath => "assets/content/vehicles/minicopter/minicopter.entity.prefab";
        }

        private class ModularCarStorageModuleConfig : VehicleConfig
        {
            public override string VehicleType => "modularcarstorage";
            public override string PrefabPath => "assets/content/vehicles/modularcar/module_entities/1module_storage.prefab";

            public override StorageContainer GetDefaultContainer(BaseEntity entity) =>
                (entity as VehicleModuleStorage)?.GetContainer() as StorageContainer;

            public override ulong GetOwnerId(BaseEntity entity) =>
                (entity as VehicleModuleStorage)?.Vehicle.OwnerID ?? 0;
        }

        private class ModularCarCamperModuleConfig : VehicleConfig
        {
            public override string VehicleType => "modularcarcamper";
            public override string PrefabPath => "assets/content/vehicles/modularcar/module_entities/2module_camper.prefab";

            public override StorageContainer GetDefaultContainer(BaseEntity entity) =>
                (entity as VehicleModuleCamper)?.activeStorage.Get(serverside: true);

            public override ulong GetOwnerId(BaseEntity entity) =>
                (entity as VehicleModuleCamper)?.Vehicle.OwnerID ?? 0;
        }

        private class RHIBConfig : RowboatConfig
        {
            public override string VehicleType => "rhib";
            public override string PrefabPath => "assets/content/vehicles/boats/rhib/rhib.prefab";
        }

        private class RidableHorseConfig : VehicleConfig
        {
            public override string VehicleType => "ridablehorse";
            public override string PrefabPath => "assets/rust.ai/nextai/testridablehorse.prefab";
        }

        private class RowboatConfig : VehicleConfig
        {
            public override string VehicleType => "rowboat";
            public override string PrefabPath => "assets/content/vehicles/boats/rowboat/rowboat.prefab";

            public override StorageContainer GetDefaultContainer(BaseEntity entity) =>
                (entity as MotorRowboat)?.storageUnitInstance.Get(serverside: true) as StorageContainer;
        }

        private class ScrapTransportHelicopterConfig : VehicleConfig
        {
            public override string VehicleType => "scraptransport";
            public override string PrefabPath => "assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab";
        }

        private class SedanConfig : VehicleConfig
        {
            public override string VehicleType => "sedan";
            public override string PrefabPath => "assets/content/vehicles/sedan_a/sedantest.entity.prefab";
        }

        private class SoloSubmarineConfig : VehicleConfig
        {
            public override string VehicleType => "solosub";
            public override string PrefabPath => "assets/content/vehicles/submarine/submarinesolo.entity.prefab";

            public override StorageContainer GetDefaultContainer(BaseEntity entity) =>
                (entity as BaseSubmarine)?.GetItemContainer();
        }

        private class WorkcartConfig : VehicleConfig
        {
            public override string VehicleType => "workcart";
            public override string PrefabPath => "assets/content/vehicles/workcart/workcart.entity.prefab";
        }

        private class Configuration : SerializableConfiguration
        {
            [JsonProperty("Chinook")]
            public ChinookConfig Chinook = new ChinookConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "2boxes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Left Box"] = 42,
                            ["Front Right Box"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4boxes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Left Box"] = 42,
                            ["Front Right Box"] = 42,
                            ["Front Upper Left Box"] = 42,
                            ["Front Upper Right Box"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Front Left Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(-0.91f, 1.259f, 2.845f),
                        RotationAngles = new Vector3(0, 90, 0),
                    },
                    ["Front Right Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.91f, 1.259f, 2.845f),
                        RotationAngles = new Vector3(0, 270, 0),
                    },
                    ["Front Upper Left Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(-0.91f, 1.806f, 2.845f),
                        RotationAngles = new Vector3(0, 90, 0),
                    },
                    ["Front Upper Right Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.91f, 1.806f, 2.845f),
                        RotationAngles = new Vector3(0, 270, 0),
                    },
                },
            };

            [JsonProperty("DuoSubmarine")]
            public DuoSubmarineConfig DuoSubmarine = new DuoSubmarineConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 12,
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "3rows",
                        BuiltInStorageCapacity = 18,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4rows",
                        BuiltInStorageCapacity = 24,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "5rows",
                        BuiltInStorageCapacity = 30,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Stash"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2stashes",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Stash"] = 42,
                            ["Back Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Front Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0, 1.56f, 0.55f),
                        RotationAngles = new Vector3(270, 0, 0),
                    },
                    ["Back Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0, 1.56f, -1.18f),
                        RotationAngles = new Vector3(270, 180, 0),
                    },
                },
            };

            [JsonProperty("HotAirBalloon")]
            public HotAirBalloonConfig HotAirBalloon = new HotAirBalloonConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 12,
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "3rows",
                        BuiltInStorageCapacity = 18,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4rows",
                        BuiltInStorageCapacity = 24,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "5rows",
                        BuiltInStorageCapacity = 30,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2stashes",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Left Stash"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "3stashes",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Left Stash"] = 42,
                            ["Front Right Stash"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4stashes",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Left Stash"] = 42,
                            ["Front Right Stash"] = 42,
                            ["Back Right Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Front Left Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(1.2f, 0.6f, 1.2f),
                        RotationAngles = new Vector3(330f, 225f, 0),
                    },
                    ["Front Right Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(1.2f, 0.6f, -1.2f),
                        RotationAngles = new Vector3(330f, 315, 0),
                    },
                    ["Back Right Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-1.2f, 0.6f, -1.2f),
                        RotationAngles = new Vector3(330f, 45f, 0),
                    },
                },
            };

            [JsonProperty("Kayak")]
            public KayakConfig Kayak = new KayakConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Back Middle Stash"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2stashes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Back Left Stash"] = 42,
                            ["Back Right Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Back Left Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.28f, 0.23f, -1.18f),
                        RotationAngles = new Vector3(270, 180, 0),
                    },
                    ["Back Middle Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.075f, 0.23f, -1.18f),
                        RotationAngles = new Vector3(270, 180, 0),
                    },
                    ["Back Right Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0.17f, 0.23f, -1.18f),
                        RotationAngles = new Vector3(270, 180, 0),
                    },
                },
            };

            [JsonProperty("MagnetCrane")]
            public MagnetCraneConfig MagnetCrane = new MagnetCraneConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Front Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.78f, -1.445f, 0f),
                        RotationAngles = new Vector3(90, 0, 90),
                        ParentBone = "Top",
                    },
                },
            };

            [JsonProperty("Minicopter")]
            public MinicopterConfig Minicopter = new MinicopterConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Stash Below Pilot Seat"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2stashes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Stash Below Pilot Seat"] = 42,
                            ["Stash Below Front Seat"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "3stashes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Stash Below Pilot Seat"] = 42,
                            ["Stash Below Front Seat"] = 42,
                            ["Stash Behind Fuel Tank"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4stashes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Stash Below Pilot Seat"] = 42,
                            ["Stash Below Front Seat"] = 42,
                            ["Stash Below Left Seat"] = 42,
                            ["Stash Below Right Seat"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Stash Below Pilot Seat"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0.01f, 0.33f, 0.21f),
                        RotationAngles = new Vector3(270, 90, 0),
                    },
                    ["Stash Below Front Seat"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0.01f, 0.22f, 1.32f),
                        RotationAngles = new Vector3(270, 90, 0),
                    },
                    ["Stash Below Left Seat"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0.6f, 0.32f, -0.41f),
                        RotationAngles = new Vector3(270, 0, 0),
                    },
                    ["Stash Below Right Seat"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.6f, 0.32f, -0.41f),
                        RotationAngles = new Vector3(270, 0, 0),
                    },
                    ["Stash Behind Fuel Tank"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0, 1.025f, -0.63f),
                        RotationAngles = new Vector3(0, 180, 0),
                    },
                    ["Box Below Fuel Tank"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0, 0.31f, -0.57f),
                        RotationAngles = new Vector3(0, 90, 0),
                    },
                    ["Back Middle Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.0f, 0.07f, -1.05f),
                        RotationAngles = new Vector3(0, 180, 0),
                    },
                    ["Back Left Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(-0.48f, 0.07f, -1.05f),
                        RotationAngles = new Vector3(0, 180, 0),
                    },
                    ["Back Right Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.48f, 0.07f, -1.05f),
                        RotationAngles = new Vector3(0, 180, 0),
                    },
                },
            };

            [JsonProperty("ModularCarCamperModule")]
            public ModularCarCamperModuleConfig ModularCarCamperModule = new ModularCarCamperModuleConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 12,
                },

                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "3rows",
                        BuiltInStorageCapacity = 18,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4rows",
                        BuiltInStorageCapacity = 24,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "5rows",
                        BuiltInStorageCapacity = 30,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                },
            };

            [JsonProperty("ModularCarStorageModule")]
            public ModularCarStorageModuleConfig ModularCarStorageModule = new ModularCarStorageModuleConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 18,
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "4rows",
                        BuiltInStorageCapacity = 24,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "5rows",
                        BuiltInStorageCapacity = 30,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                },
            };

            [JsonProperty("RHIB")]
            public RHIBConfig RHIB = new RHIBConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 30,
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "3boxes",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Back Left Box"] = 42,
                            ["Back Right Box"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Back Left Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(-0.4f, 1.255f, -2.25f),
                        RotationAngles = new Vector3(0, 270, 0),
                    },
                    ["Back Right Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.4f, 1.255f, -2.25f),
                        RotationAngles = new Vector3(0, 90, 0),
                    },
                },
            };

            [JsonProperty("RidableHorse")]
            public RidableHorseConfig RidableHorse = new RidableHorseConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Back Left Stash"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2stashes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Back Left Stash"] = 42,
                            ["Back Right Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Back Left Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.1f, 0.1f, 0),
                        RotationAngles = new Vector3(270, 285, 0),
                        ParentBone = "L_Hip",
                    },
                    ["Back Right Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.1f, -0.1f, 0),
                        RotationAngles = new Vector3(90, 105, 0),
                        ParentBone = "R_Hip",
                    },
                },
            };

            [JsonProperty("Rowboat")]
            public RowboatConfig Rowboat = new RowboatConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 12,
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "3rows",
                        BuiltInStorageCapacity = 18,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "4rows",
                        BuiltInStorageCapacity = 24,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "5rows",
                        BuiltInStorageCapacity = 30,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2stashes",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Back Left Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Back Left Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.42f, 0.52f, -1.7f),
                        RotationAngles = new Vector3(270, 180, 0),
                    },
                },
            };

            [JsonProperty("ScrapTransportHelicopter")]
            public ScrapTransportHelicopterConfig ScrapTransportHelicopter = new ScrapTransportHelicopterConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1box",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["LeftBox"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2boxes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["LeftBox"] = 42,
                            ["RightBox"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["LeftBox"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(-0.5f, 0.85f, 1.75f),
                    },
                    ["RightBox"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.5f, 0.85f, 1.75f),
                    },
                },
            };

            [JsonProperty("Sedan")]
            public SedanConfig Sedan = new SedanConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Middle Stash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Middle Stash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(0, 0.83f, 0.55f),
                        RotationAngles = new Vector3(270, 180, 0),
                    },
                },
            };

            [JsonProperty("SoloSubmarine")]
            public SoloSubmarineConfig SoloSubmarine = new SoloSubmarineConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    BuiltInStorageCapacity = 18,
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "4rows",
                        BuiltInStorageCapacity = 24,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "5rows",
                        BuiltInStorageCapacity = 30,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "6rows",
                        BuiltInStorageCapacity = 36,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "7rows",
                        BuiltInStorageCapacity = 42,
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "1stash",
                        BuiltInStorageCapacity = 42,
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["BackLeftStash"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["BackLeftStash"] = new ContainerPreset
                    {
                        Prefab = HabStoragePrefab,
                        Position = new Vector3(-0.34f, 1.16f, -0.7f),
                        RotationAngles = new Vector3(300, 270, 270),
                    },
                },
            };

            [JsonProperty("Workcart")]
            public WorkcartConfig Workcart = new WorkcartConfig
            {
                DefaultProfile = new VehicleProfile
                {
                    AdditionalStorage = new Dictionary<string, int>(),
                },
                ProfilesRequiringPermission = new VehicleProfile[]
                {
                    new VehicleProfile
                    {
                        PermissionSuffix = "1box",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Box"] = 42,
                        },
                    },
                    new VehicleProfile
                    {
                        PermissionSuffix = "2boxes",
                        AdditionalStorage = new Dictionary<string, int>
                        {
                            ["Front Box"] = 42,
                            ["Back Box"] = 42,
                        },
                    },
                },
                ContainerPresets = new Dictionary<string, ContainerPreset>
                {
                    ["Front Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.85f, 2.63f, 1.43f),
                        RotationAngles = new Vector3(0, 180, 0),
                    },
                    ["Back Box"] = new ContainerPreset
                    {
                        Prefab = RhibStoragePrefab,
                        Position = new Vector3(0.85f, 2.63f, 0.7f),
                        RotationAngles = new Vector3(0, 180, 0),
                    },
                },
            };

            [JsonIgnore]
            private readonly Dictionary<uint, VehicleConfig> _vehicleConfigsByPrefabId = new Dictionary<uint, VehicleConfig>();

            public void Init(VehicleStorage pluginInstance)
            {
                var allVehicleConfigs = new VehicleConfig[]
                {
                    Chinook,
                    DuoSubmarine,
                    HotAirBalloon,
                    Kayak,
                    MagnetCrane,
                    Minicopter,
                    ModularCarCamperModule,
                    ModularCarStorageModule,
                    RHIB,
                    RidableHorse,
                    Rowboat,
                    ScrapTransportHelicopter,
                    Sedan,
                    SoloSubmarine,
                    Workcart,
                };

                foreach (var vehicleConfig in allVehicleConfigs)
                {
                    // Map the configs by prefab id for fast lookup.
                    var prefabId = StringPool.Get(vehicleConfig.PrefabPath);
                    _vehicleConfigsByPrefabId[prefabId] = vehicleConfig;

                    // Validate correctness, and register permissions.
                    vehicleConfig.Init(pluginInstance);
                }
            }

            public VehicleConfig GetVehicleConfig(BaseEntity entity)
            {
                VehicleConfig vehicleConfig;
                return _vehicleConfigsByPrefabId.TryGetValue(entity.prefabID, out vehicleConfig)
                    ? vehicleConfig
                    : null;
            }
        }

        private Configuration GetDefaultConfig() => new Configuration();

        #endregion

        #region Configuration Boilerplate

        private class SerializableConfiguration
        {
            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonHelper.Deserialize(ToJson()) as Dictionary<string, object>;
        }

        private static class JsonHelper
        {
            public static object Deserialize(string json) => ToObject(JToken.Parse(json));

            private static object ToObject(JToken token)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        return token.Children<JProperty>()
                                    .ToDictionary(prop => prop.Name,
                                                  prop => ToObject(prop.Value));

                    case JTokenType.Array:
                        return token.Select(ToObject).ToList();

                    default:
                        return ((JValue)token).Value;
                }
            }
        }

        private bool MaybeUpdateConfig(SerializableConfiguration config)
        {
            var currentWithDefaults = config.ToDictionary();
            var currentRaw = Config.ToDictionary(x => x.Key, x => x.Value);
            return MaybeUpdateConfigDict(currentWithDefaults, currentRaw);
        }

        private bool MaybeUpdateConfigDict(Dictionary<string, object> currentWithDefaults, Dictionary<string, object> currentRaw)
        {
            bool changed = false;

            foreach (var key in currentWithDefaults.Keys)
            {
                object currentRawValue;
                if (currentRaw.TryGetValue(key, out currentRawValue))
                {
                    var defaultDictValue = currentWithDefaults[key] as Dictionary<string, object>;
                    var currentDictValue = currentRawValue as Dictionary<string, object>;

                    if (defaultDictValue != null)
                    {
                        if (currentDictValue == null)
                        {
                            currentRaw[key] = currentWithDefaults[key];
                            changed = true;
                        }
                        else if (MaybeUpdateConfigDict(defaultDictValue, currentDictValue))
                            changed = true;
                    }
                }
                else
                {
                    currentRaw[key] = currentWithDefaults[key];
                    changed = true;
                }
            }

            return changed;
        }

        protected override void LoadDefaultConfig() => _pluginConfig = GetDefaultConfig();

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _pluginConfig = Config.ReadObject<Configuration>();
                if (_pluginConfig == null)
                {
                    throw new JsonException();
                }

                if (MaybeUpdateConfig(_pluginConfig))
                {
                    LogWarning("Configuration appears to be outdated; updating and saving");
                    SaveConfig();
                }
            }
            catch (Exception e)
            {
                LogError(e.Message);
                LogWarning($"Configuration file {Name}.json is invalid; using defaults");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig()
        {
            Log($"Configuration changes saved to {Name}.json");
            Config.WriteObject(_pluginConfig, true);
        }

        #endregion
    }
}
