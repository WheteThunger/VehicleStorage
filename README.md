**Larger Car Storage** increases the capacity of storage modules on modular cars.

## Configuration

Default configuration:
```json
{
  "GlobalSettings": {
    "LootPanelName": "genericlarge",
    "StorageCapacity": 42
  }
}
```

- `LootPanelName` -- Determines the size of the loot panel and the maximum number of visible slots. Some examples below.
  - `smallwoodbox` (12 capacity)
  - `modularcar.storage` (18 capacity)
  - `largewoodbox` (30 capacity)
  - `generic` (36 capacity)
  - `genericlarge` (42 capacity)
- `StorageCapacity` -- Number of item slots to allow. If this is set lower than the visible number of item slots (as determined by the loot panel name), the extra visible slots will simply be disabled.

While reducing capacity, be mindful that some items already in storage modules may no longer be visible if they were placed in the lower slots. However, this is pretty safe as the module cannot be removed while it has items in it, and the items are recoverable by clicking the "take internal items" button while the storage module is selected at a car lift. The items will also drop if the car is destroyed.

## Uninstallation

To uninstall this plugin, change the config to the following, reload the plugin, then delete the plugin and config file.

```json
{
  "GlobalSettings": {
    "LootPanelName": "modularcar.storage",
    "StorageCapacity": 18
  }
}
```
