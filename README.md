**Larger Car Storage** allows you to alter the capacity of storage modules on modular cars. Supports a global setting for all cars, as well as a permission system to selectively increase the capacity of storage modules on cars owned by privileged players.

Compatible with [Claim Vehicle Ownership](https://umod.org/plugins/claim-vehicle-ownership). Meaning, changing ownership of a car automatically updates storage capacity of all modules on the car.

## Permissions

Granting one of the following permissions to a player will increase the number of usable storage **rows** of each storage module on cars that player owns. For example, if you grant a player the `largercarstorage.size.5` permission and then spawn a car for them using a plugin that sets the player's Steam ID on the car, all storage modules on that car (current and future) will have 5 rows of storage.

- `largercarstorage.size.4`
- `largercarstorage.size.5`
- `largercarstorage.size.6`
- `largercarstorage.size.7`

Notes:
- Car ownership is determined by the `OwnerID` property of the car, which is usually a player's Steam ID, or `0` for no owner. Various plugins can spawn cars with a set owner, or allow the owner to change with certain events.
- Granting or revoking permissions will immediately take effect on all owned cars.
- If a player has multiple size permissions, the largest will be used.

## Configuration

Default configuration:
```json
{
  "DefaultCapacityRows": 3
}
```

- `DefaultCapacityRows` -- The minimum number of storage rows that should be usable for storage modules on all cars, owned and unowned. Storage modules on cars owned by players with additional permissions may have more storage rows than this amount, but not fewer.

## Disclaimer

If you increase and then later decrease the capacity of a car's storage modules, either via the plugin configuration or player permissions, be mindful that some items already in storage modules may no longer be visible if they were placed in the lower slots. This is actually pretty safe to do because the module cannot be removed from the car while it has items in it, and the items are recoverable by clicking the "take internal items" button while the storage module is selected at a car lift. The items will also drop if the car is destroyed.

## Uninstallation

To uninstall this plugin, revoke all permissions, change the config to the following, then reload the plugin. This will update all current cars to vanilla capacity. You can then safely delete the plugin and config file. If you don't follow these steps and instead simply delete the plugin, some storage modules may have altered capacity until the next server restart.

```json
{
  "DefaultCapacityRows": 3
}
```

## Developer API

#### API_RefreshStorageCapacity

Plugins can call this API to refresh a car's storage capacity according to the permissions of its current owner. This should be done if a plugin allows the ownership (`OwnerID`) of a car to change and the developer wants the storage capacity to be updated. If called while the car has no owner, capacity will be updated to the default from the plugin configuration.

```csharp
API_RefreshStorage(ModularCar car)
```
