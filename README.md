# ZetTweaks

Most listed changes can be configured and disable.

Some changes will be disabled if another mod already does that change.

Fix console error spam from TeleShow and NoLockedInteractables.

- Change lunar equipment droplet color.

- Prevent Focus Crystal, Crowbar and Delicate Watch from increasing self-damage.

- Only place PrimordialTeleporter on last stage of a loop.

- Equipment isn't triggered by Gesture in the Bazaar.

- Prevent the Newt from kicking you from the Bazaar.

- Recover 50% of health and shields when a VoidCell is activated.

- Start the first 5 stages with enough money to open a chest.

- BloodShrines give a minimum of 2 chests worth of money per 100% health taken.

- CombatDirector gains 10% more credits for first 5 stages.

- Decaying boost to money earned from kills while Multitudes is enabled.

- Adjust teleporter boss drop chance, reducing amount of boss tier items on large reward count.

- Prevent teleporter from locking interactables.

- Prevent Eclipse from affecting small holdout zones.

- Adjust base radius and duration of holdout zones:

| HoldoutZone | radius | time |
|--|--|--|
| Teleporter  | 60m -> 90m | 90s |
| Void Cell| 15m -> 20m | 60s -> 45s |
| DeepVoid Signal| 20m -> 30m | 60s -> 45s |
| Pillar of Mass| 20m -> 30m | 60s -> 45s |
| Rescue Ship | 40m -> 60m | 60s -> 45s |

- Pillars required to fight Mithrix changed from 4 to 3.

- VoidSignals required to fight Voidling changed from 4 to 3.

- Items and equipment don't drop in command essence if there is only one choice.

- Remove unobtainable items from the command menu.

- Pickups that go out of bounds are teleported back onto the stage.

- Increase Huntress targeting range by 2m every level and prioritize enemies near the crosshair.

- Prevent allied drones from targeting allies after certain items proc.

- EquipmentDrones have a 25% chance to drop equipment after they are destroyed.

- TC280 and GunnerTurret can be repurchased after they are destroyed.

- TC280 and EquipmentDrone can appear on any stage where regular drones spawn.

- Failing a ChanceShrines reduces fail chance and improves item rarity. Resets when rewarded.

- Configurable ChanceShrine reward count, cost scaling, reward scaling, and time between each use.

## Installation:

Requires Bepinex and HookGenPatcher.

Use r2modman or place inside of Risk of Rain 2/Bepinex/Plugins/

## Changelog:

v1.1.8 - ZetAspects compatibility for CommandDropletFix.

v1.1.7 - Fixed for latest game update.

v1.1.6 - Fixed prevention of self damage increasing effects.

v1.1.5 - Change lunar equipment droplet color. Remove unobtainable items in the command menu. ChanceShrines improve with failures and are configurable.

v1.1.4 - Prevent MultitudeGoldBoost from granting less then the base gold reward.

v1.1.3 - Fixed some drones still retaliating against allies. BloodShrines now provide a minimum reward that scales with difficulty. VoidSignals required reduced to 3.

v1.1.2 - Check for PickupPickerController on out of bounds items. Add ShareSuite support for startingMoney. Fix VoidCell Holdout Radius and made finished VoidCell scale bubble based on actual radius.

v1.1.1 - Removed debug drone health decay.

v1.1.0 - Updated for latest game version.

v1.0.5 - Prevent Bazaar kickout. Prevent some drones targeting allies. Drones can be repurchased and appear in more areas. EquipmentDrone chance to drop equipment on death.

v1.0.4 - Only place PrimordialTeleporter on last stage of a loop. Recovery when Void Cell is activated. Fix HuntressRange not working with HuntressAutoaimFix.

v1.0.3 - Prevent tweakBossDropRewards from affecting drops with red tier items.

v1.0.2 - Fix TeleShowFix NullRef when a profile was not selected.

v1.0.1 - Split mod out into StatAdjustment and ProcConfig. Gesture doesn't trigger equipment in bazaar.

v1.0.0 - Initial Release.
