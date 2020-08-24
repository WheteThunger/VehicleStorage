using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("Larger Car Storage", "WhiteThunder", "1.0.0")]
    [Description("Increases the capacity of storage modules on modular cars.")]
    internal class LargerCarStorage : CovalencePlugin
    {
        #region Fields

        private static LargerCarStorage PluginInstance;

        private LargerCarStorageConfig PluginConfig;

        #endregion

        #region Hooks

        private void Init()
        {
            PluginInstance = this;
            PluginConfig = Config.ReadObject<LargerCarStorageConfig>();
            PluginConfig.Validate();
        }

        private void Unload()
        {
            PluginInstance = null;
        }

        private void OnServerInitialized(bool isServerInitializing)
        {
            // The OnEntitySpawned hook already covers server init so this is just for late loading
            if (!isServerInitializing)
            {
                foreach (var engineModule in BaseNetworkable.serverEntities.OfType<VehicleModuleStorage>())
                    AdjustStorageCapacity(engineModule);
            }
        }

        private void OnEntitySpawned(VehicleModuleStorage storageModule)
        {
            NextTick(() => AdjustStorageCapacity(storageModule));
        }

        #endregion

        #region Helper Methods

        private void AdjustStorageCapacity(VehicleModuleStorage storageModule)
        {
            if (storageModule == null || storageModule is VehicleModuleEngine) return;

            var container = storageModule.GetContainer() as StorageContainer;
            if (container == null) return;

            container.panelName = PluginConfig.GlobalSettings.LootPanelName;
            container.inventory.capacity = PluginConfig.GlobalSettings.StorageCapacity;
        }

        #endregion

        #region Configuration

        protected override void LoadDefaultConfig() => Config.WriteObject(new LargerCarStorageConfig(), true);

        internal class LargerCarStorageConfig
        {
            [JsonProperty("GlobalSettings")]
            public GlobalStorageSettings GlobalSettings = new GlobalStorageSettings();

            [JsonIgnore]
            private readonly Dictionary<string, int> MaxCapacityByPanelName = new Dictionary<string, int>
            {
                ["smallwoodbox"] = 12,
                ["modularcar.storage"] = 18,
                ["largewoodbox"] = 30,
                ["generic"] = 36,
                ["genericlarge"] = 42,
            };

            public void Validate()
            {
                int maxCapacity;
                if (MaxCapacityByPanelName.TryGetValue(GlobalSettings.LootPanelName, out maxCapacity) && GlobalSettings.StorageCapacity > maxCapacity)
                {
                    PluginInstance.LogWarning("Panel name '{0}' does not support {1} capacity. Reducing to {2}.", GlobalSettings.LootPanelName, GlobalSettings.StorageCapacity, maxCapacity);
                    GlobalSettings.StorageCapacity = maxCapacity;
                    PluginInstance.Config.WriteObject(this, true);
                }
            }
        }

        internal class GlobalStorageSettings
        {
            [JsonProperty("LootPanelName")]
            public string LootPanelName = "genericlarge";

            [JsonProperty("StorageCapacity")]
            public int StorageCapacity = 42;
        }

        #endregion
    }
}
