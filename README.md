## Features

- Allows increasing capacity of built-in vehicle storage
- Allows adding unlimited configurable storage containers to vehicles
- Allows both of the above to be dynamic based on vehicle owner permissions

#### Notes

- Additional storage containers will drop their items when the vehicle is destroyed.
- Compatible with [Claim Vehicle Ownership](https://umod.org/plugins/claim-vehicle-ownership), allowing storage to dynamically change when a player claims a vehicle.
- Granting permission to a user, granting permission to a group, or adding a user to a group will automatically add or update storage on applicable owned vehicles.

## Limitations

- Once a storage container's capacity has been increased, it will never be reduced until server restart, even if the configuration changes or if the player loses permission. This can be improved upon request.
- Once a storage container has been spawned on a vehicle, it will not be removed until the vehicle is destroyed. This can be improved upon request.

## Permissions

***Note: Permissions are an optional feature of this plugin. If you simply want to alter the storage capacity of all vehicles of a given type, or if you want to add extra storage containers to all vehicles of a given type, skip ahead to the Configuration section and configure the `DefaultProfile` of the vehicles you care about.***

Permissions in this plugin apply solely based on vehicle ownership. Vehicle ownership is **not** a vanilla Rust concept. This means that, except for Kayaks, you will need another plugin to assign vehicle ownership. Fortunately, many plugins already do, and some are quite immersive.

- [Vehicle Vendor Options](https://umod.org/plugins/vehicle-vendor-options) -- Automatically assigns ownership of vehicles purchased at vanilla NPC vendors if the player has permission.
- [Craft Car Chassis](https://umod.org/plugins/craft-car-chassis) -- Automatically assigns ownership of cars built by players, if the config option for that is enabled.
- [Claim Vehicle Ownership](https://umod.org/plugins/claim-vehicle-ownership) -- Allows players with permission to claim ownership of unowned vehicles using a command on cooldown.
- [Vehicle License](https://umod.org/plugins/vehicle-license), [Spawn Modular Car](https://umod.org/plugins/spawn-modular-car) and other similar plugins that spawn vehicles for players will automatically assign ownership.

The following permissions come with this plugin's **default configuration**. You can fully customize these presets, as well as add new ones in the plugin configuration. If a player has permission to multiple presets for a given vehicle, the last will be used, based on the order in the config.

### Ground vehicles

#### Ridable horse

- `vehiclestorage.ridablehorse.1stash` -- 1 x 42-slot stash (42 total capacity)
- `vehiclestorage.ridablehorse.2stashes` -- 2 x 42-slot stash (84 total capacity)

#### Modular car camper module

- `vehiclestorage.modularcarcamper.3rows` -- 3 rows (18 total capacity)
- `vehiclestorage.modularcarcamper.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.modularcarcamper.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.modularcarcamper.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.modularcarcamper.7rows` -- 7 rows (42 total capacity)

#### Modular car storage module

- `vehiclestorage.modularcarstorage.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.modularcarstorage.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.modularcarstorage.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.modularcarstorage.7rows` -- 7 rows (42 total capacity)

#### Snowmobile

- `vehiclestorage.snowmobile.3rows` -- 3 rows (18 total capacity)
- `vehiclestorage.snowmobile.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.snowmobile.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.snowmobile.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.snowmobile.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.snowmobile.2stashes` -- 7 rows + 2 x 42-slot stash (126 total capacity)

#### Tomaha snowmobile

- `vehiclestorage.tomaha.3rows` -- 3 rows (18 total capacity)
- `vehiclestorage.tomaha.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.tomaha.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.tomaha.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.tomaha.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.tomaha.2stashes` -- 7 rows + 2 x 42-slot stash (126 total capacity)

#### Workcart

- `vehiclestorage.workcart.1box` -- 1 x 42-slot box (42 total capacity)
- `vehiclestorage.workcart.2boxes` -- 2 x 42-slot box (84 total capacity)

#### Sedan

- `vehiclestorage.sedan.1stash` -- 1 x 42-slot stash (42 total capacity)

#### MagnetCrane

- `vehiclestorage.magnetcrane.1stash` -- 1 x 42-slot stash (42 total capacity)

### Boats

#### Kayak

- `vehiclestorage.kayak.1stash` -- 1 x 42-slot stash (42 total capacity)
- `vehiclestorage.kayak.2stashes` -- 2 x 42-slot stash (84 total capacity)

#### Rowboat

- `vehiclestorage.rowboat.3rows` -- 3 rows (18 total capacity)
- `vehiclestorage.rowboat.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.rowboat.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.rowboat.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.rowboat.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.rowboat.2stashes` -- 7 rows + 1 x 42-slot stash (84 total capacity)

#### RHIB

- `vehiclestorage.rhib.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.rhib.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.rhib.3boxes` -- 7 rows + 2 x 42-slot box (126 total capacity)

#### SoloSubmarine

- `vehiclestorage.solosub.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.solosub.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.solosub.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.solosub.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.solosub.1stash` -- 7 rows + 1 x 42-slot stash (84 total capacity)

#### Duo submarine

- `vehiclestorage.duosub.3rows` -- 3 rows (18 total capacity)
- `vehiclestorage.duosub.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.duosub.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.duosub.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.duosub.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.duosub.1stash` -- 7 rows + 1 x 42-slot stash (84 total capacity)
- `vehiclestorage.duosub.2stashes` -- 7 rows + 2 x 42-slot stash (126 total capacity)

### Aircrafts

#### Hot air balloon

- `vehiclestorage.hotairballoon.3rows` -- 3 rows (18 total capacity)
- `vehiclestorage.hotairballoon.4rows` -- 4 rows (24 total capacity)
- `vehiclestorage.hotairballoon.5rows` -- 5 rows (30 total capacity)
- `vehiclestorage.hotairballoon.6rows` -- 6 rows (36 total capacity)
- `vehiclestorage.hotairballoon.7rows` -- 7 rows (42 total capacity)
- `vehiclestorage.hotairballoon.2stashes` -- 7 rows + 1 x 42-slot stash (84 total capacity)
- `vehiclestorage.hotairballoon.3stashes` -- 7 rows + 2 x 42-slot stash (126 total capacity)
- `vehiclestorage.hotairballoon.4stashes` -- 7 rows + 3 x 42-slot stash (168 total capacity)

#### Minicopter

- `vehiclestorage.minicopter.1stash` -- 1 x 42-slot stash (42 total capacity)
- `vehiclestorage.minicopter.2stashes` -- 2 x 42-slot stash (84 total capacity)
- `vehiclestorage.minicopter.3stashes` -- 3 x 42-slot stash (126 total capacity)
- `vehiclestorage.minicopter.4stashes` -- 4 x 42-slot stash (168 total capacity)

#### ScrapTransportHelicopter

- `vehiclestorage.scraptransport.1box` -- 1 x 42-slot box (42 total capacity)
- `vehiclestorage.scraptransport.2boxes` -- 2 x 42-slot box (84 total capacity)

#### Chinook

- `vehiclestorage.chinook.2boxes` -- 2 x 42-slot box (84 total capacity)
- `vehiclestorage.chinook.2boxes` -- 4 x 42-slot box (168 total capacity)

## Configuration

Default configuration:

```json
{
  "Chinook": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "2boxes",
        "AdditionalStorage": {
          "Front Left Box": 42,
          "Front Right Box": 42
        }
      },
      {
        "PermissionSuffix": "4boxes",
        "AdditionalStorage": {
          "Front Left Box": 42,
          "Front Right Box": 42,
          "Front Upper Left Box": 42,
          "Front Upper Right Box": 42
        }
      }
    ],
    "ContainerPresets": {
      "Front Left Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": -0.91,
          "y": 1.259,
          "z": 2.845
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 90.0,
          "z": 0.0
        }
      },
      "Front Right Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.91,
          "y": 1.259,
          "z": 2.845
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 270.0,
          "z": 0.0
        }
      },
      "Front Upper Left Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": -0.91,
          "y": 1.806,
          "z": 2.845
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 90.0,
          "z": 0.0
        }
      },
      "Front Upper Right Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.91,
          "y": 1.806,
          "z": 2.845
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 270.0,
          "z": 0.0
        }
      }
    }
  },
  "DuoSubmarine": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 12,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "3rows",
        "BuiltInStorageCapacity": 18
      },
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "1stash",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Front Stash": 42
        }
      },
      {
        "PermissionSuffix": "2stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Front Stash": 42,
          "Back Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Front Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.0,
          "y": 1.56,
          "z": 0.55
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      "Back Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.0,
          "y": 1.56,
          "z": -1.18
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 180.0,
          "z": 0.0
        }
      }
    }
  },
  "HotAirBalloon": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 12,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "3rows",
        "BuiltInStorageCapacity": 18
      },
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "2stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Front Left Stash": 42
        }
      },
      {
        "PermissionSuffix": "3stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Front Left Stash": 42,
          "Front Right Stash": 42
        }
      },
      {
        "PermissionSuffix": "4stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Front Left Stash": 42,
          "Front Right Stash": 42,
          "Back Right Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Front Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 1.2,
          "y": 0.6,
          "z": 1.2
        },
        "RotationAngles": {
          "x": 330.0,
          "y": 225.0,
          "z": 0.0
        }
      },
      "Front Right Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 1.2,
          "y": 0.6,
          "z": -1.2
        },
        "RotationAngles": {
          "x": 330.0,
          "y": 315.0,
          "z": 0.0
        }
      },
      "Back Right Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -1.2,
          "y": 0.6,
          "z": -1.2
        },
        "RotationAngles": {
          "x": 330.0,
          "y": 45.0,
          "z": 0.0
        }
      }
    }
  },
  "Kayak": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1stash",
        "AdditionalStorage": {
          "Back Middle Stash": 42
        }
      },
      {
        "PermissionSuffix": "2stashes",
        "AdditionalStorage": {
          "Back Left Stash": 42,
          "Back Right Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.28,
          "y": 0.23,
          "z": -1.18
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 180.0,
          "z": 0.0
        }
      },
      "Back Middle Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.075,
          "y": 0.23,
          "z": -1.18
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 180.0,
          "z": 0.0
        }
      },
      "Back Right Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.17,
          "y": 0.23,
          "z": -1.18
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 180.0,
          "z": 0.0
        }
      }
    }
  },
  "MagnetCrane": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1stash",
        "AdditionalStorage": {
          "Front Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Front Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.78,
          "y": -1.445,
          "z": 0.0
        },
        "RotationAngles": {
          "x": 90.0,
          "y": 0.0,
          "z": 90.0
        },
        "ParentBone": "Top"
      }
    }
  },
  "Minicopter": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1stash",
        "AdditionalStorage": {
          "Stash Below Pilot Seat": 42
        }
      },
      {
        "PermissionSuffix": "2stashes",
        "AdditionalStorage": {
          "Stash Below Pilot Seat": 42,
          "Stash Below Front Seat": 42
        }
      },
      {
        "PermissionSuffix": "3stashes",
        "AdditionalStorage": {
          "Stash Below Pilot Seat": 42,
          "Stash Below Front Seat": 42,
          "Stash Behind Fuel Tank": 42
        }
      },
      {
        "PermissionSuffix": "4stashes",
        "AdditionalStorage": {
          "Stash Below Pilot Seat": 42,
          "Stash Below Front Seat": 42,
          "Stash Below Left Seat": 42,
          "Stash Below Right Seat": 42
        }
      }
    ],
    "ContainerPresets": {
      "Stash Below Pilot Seat": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.01,
          "y": 0.33,
          "z": 0.21
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 90.0,
          "z": 0.0
        }
      },
      "Stash Below Front Seat": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.01,
          "y": 0.22,
          "z": 1.32
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 90.0,
          "z": 0.0
        }
      },
      "Stash Below Left Seat": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.6,
          "y": 0.32,
          "z": -0.41
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      "Stash Below Right Seat": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.6,
          "y": 0.32,
          "z": -0.41
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      "Stash Behind Fuel Tank": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.0,
          "y": 1.025,
          "z": -0.63
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 180.0,
          "z": 0.0
        }
      },
      "Box Below Fuel Tank": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.0,
          "y": 0.31,
          "z": -0.57
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 90.0,
          "z": 0.0
        }
      },
      "Back Middle Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.0,
          "y": 0.07,
          "z": -1.05
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 180.0,
          "z": 0.0
        }
      },
      "Back Left Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": -0.48,
          "y": 0.07,
          "z": -1.05
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 180.0,
          "z": 0.0
        }
      },
      "Back Right Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.48,
          "y": 0.07,
          "z": -1.05
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 180.0,
          "z": 0.0
        }
      }
    }
  },
  "ModularCarCamperModule": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 12
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "3rows",
        "BuiltInStorageCapacity": 18
      },
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      }
    ]
  },
  "ModularCarStorageModule": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 18
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      }
    ]
  },
  "RHIB": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 30,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "3boxes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Back Left Box": 42,
          "Back Right Box": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": -0.4,
          "y": 1.255,
          "z": -2.25
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 270.0,
          "z": 0.0
        }
      },
      "Back Right Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.4,
          "y": 1.255,
          "z": -2.25
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 90.0,
          "z": 0.0
        }
      }
    }
  },
  "RidableHorse": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1stash",
        "AdditionalStorage": {
          "Back Left Stash": 42
        }
      },
      {
        "PermissionSuffix": "2stashes",
        "AdditionalStorage": {
          "Back Left Stash": 42,
          "Back Right Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.1,
          "y": 0.1,
          "z": 0.0
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 285.0,
          "z": 0.0
        },
        "ParentBone": "L_Hip"
      },
      "Back Right Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.1,
          "y": -0.1,
          "z": 0.0
        },
        "RotationAngles": {
          "x": 90.0,
          "y": 105.0,
          "z": 0.0
        },
        "ParentBone": "R_Hip"
      }
    }
  },
  "Rowboat": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 12,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "3rows",
        "BuiltInStorageCapacity": 18
      },
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "2stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Back Left Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.42,
          "y": 0.52,
          "z": -1.7
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 180.0,
          "z": 0.0
        }
      }
    }
  },
  "ScrapTransportHelicopter": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1box",
        "AdditionalStorage": {
          "LeftBox": 42
        }
      },
      {
        "PermissionSuffix": "2boxes",
        "AdditionalStorage": {
          "LeftBox": 42,
          "RightBox": 42
        }
      }
    ],
    "ContainerPresets": {
      "LeftBox": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": -0.5,
          "y": 0.85,
          "z": 1.75
        }
      },
      "RightBox": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.5,
          "y": 0.85,
          "z": 1.75
        }
      }
    }
  },
  "Sedan": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1stash",
        "AdditionalStorage": {
          "Middle Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Middle Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.0,
          "y": 0.83,
          "z": 0.55
        },
        "RotationAngles": {
          "x": 270.0,
          "y": 180.0,
          "z": 0.0
        }
      }
    }
  },
  "Snowmobile": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 12,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "3rows",
        "BuiltInStorageCapacity": 18
      },
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "2stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Back Left Stash": 42,
          "Back Right Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.21,
          "y": 0.555,
          "z": -1.08
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 270.0,
          "z": 270.0
        }
      },
      "Back Right Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.21,
          "y": 0.555,
          "z": -1.08
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 90.0,
          "z": 90.0
        }
      }
    }
  },
  "SoloSubmarine": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 18,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "1stash",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Back Left Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.34,
          "y": 1.16,
          "z": -0.7
        },
        "RotationAngles": {
          "x": 300.0,
          "y": 270.0,
          "z": 270.0
        }
      }
    }
  },
  "Tomaha": {
    "DefaultProfile": {
      "BuiltInStorageCapacity": 12,
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "3rows",
        "BuiltInStorageCapacity": 18
      },
      {
        "PermissionSuffix": "4rows",
        "BuiltInStorageCapacity": 24
      },
      {
        "PermissionSuffix": "5rows",
        "BuiltInStorageCapacity": 30
      },
      {
        "PermissionSuffix": "6rows",
        "BuiltInStorageCapacity": 36
      },
      {
        "PermissionSuffix": "7rows",
        "BuiltInStorageCapacity": 42
      },
      {
        "PermissionSuffix": "2stashes",
        "BuiltInStorageCapacity": 42,
        "AdditionalStorage": {
          "Back Left Stash": 42,
          "Back Right Stash": 42
        }
      }
    ],
    "ContainerPresets": {
      "Back Left Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": -0.21,
          "y": 0.37,
          "z": -1.08
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 270.0,
          "z": 270.0
        }
      },
      "Back Right Stash": {
        "Prefab": "assets/prefabs/deployable/hot air balloon/subents/hab_storage.prefab",
        "Position": {
          "x": 0.21,
          "y": 0.37,
          "z": -1.08
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 90.0,
          "z": 90.0
        }
      }
    }
  },
  "Workcart": {
    "DefaultProfile": {
      "AdditionalStorage": {}
    },
    "ProfilesRequiringPermission": [
      {
        "PermissionSuffix": "1box",
        "AdditionalStorage": {
          "Front Box": 42
        }
      },
      {
        "PermissionSuffix": "2boxes",
        "AdditionalStorage": {
          "Front Box": 42,
          "Back Box": 42
        }
      }
    ],
    "ContainerPresets": {
      "Front Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.85,
          "y": 2.63,
          "z": 1.43
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 180.0,
          "z": 0.0
        }
      },
      "Back Box": {
        "Prefab": "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab",
        "Position": {
          "x": 0.85,
          "y": 2.63,
          "z": 0.7
        },
        "RotationAngles": {
          "x": 0.0,
          "y": 180.0,
          "z": 0.0
        }
      }
    }
  }
}
```

Each vehicle type has the following options.
- `DefaultProfile` -- Determines the default capacity and additional storage containers for vehicles of this type. The default profile will be ignored for vehicles owned by players with permission to any profile in `ProfilesRequiringPermission`.
  - `BuiltInStorageCapacity` -- Determines the capacity of storage containers that naturally occur on the vehicle.
    - This is applicable to Rowboats, RHIBs, Solo & Duo Submarines, Hot Air Balloons, and Modular Car storage modules.
  - `AdditionalStorage` -- Determines which additional storage containers will be added to the vehicle, and how much capacity each of those containers will have.
    - Example: `"Front Box": 42` would spawn the "Front Box" container and assign it 42 slots of capacity. For this to work, "Front Box" would have to be a valid preset in the `ContainerPresets` section.
- `ProfilesRequiringPermission` -- Defines profiles that will override the `DefaultProfile` when the vehicle is owned by a player with a permission to any profile.
  - `PermissionSuffix` -- Determines the generated permission of format `vehiclestorage.<vehicle>.<suffix>`.
  - `BuiltInStorageCapacity` -- Equivalent to the option of the same name in `DefaultProfile`.
  - `AdditionalStorage` -- Equivalent to the option of the same name in `DefaultProfile`.
- `ContainerPresets` -- Defines named container presets. Referring to a preset in a profile allows spawning additional containers.
  - `Prefab` -- Determines the container prefab to spawn. Must be a valid storage container.
  - `Position` -- Determines the position of the storage container relative to the vehicle.
  - `RotationAngles` -- Determines the rotation of the storage container relative to the vehicle.
  - `ParentBone` -- Determines which bone the storage container will be parented relative to on the vehicle.

## Developer API

#### API_RefreshVehicleStorage

```csharp
API_RefreshVehicleStorage(BaseEntity vehicle)
```

Plugins can call this API to refresh a vehicle storage capacity and containers.

## Developer Hooks

#### OnVehicleStorageUpdate

```csharp
bool? OnVehicleStorageUpdate(BaseEntity vehicle)
```

- Called when this plugin is about to alter storage container capacity and/or spawn storage containers on a vehicle
- Returning `false` will prevent this plugin from doing anything to the vehicle
- Returning `null` will result in the default behavior

#### OnVehicleStorageSpawn

```csharp
bool? OnVehicleStorageSpawn(BaseEntity vehicle)
```

- Called when this plugin is about to spawn a storage container on a vehicle
- Returning `false` will prevent the storage container from spawning
- Returning `null` will result in the default behavior

#### OnVehicleStorageSpawned

```csharp
void OnVehicleStorageSpawned(BaseEntity vehicle, StorageContainer container)
```

- Called after this plugin has spawned a storage container on a vehicle
- No return behavior
