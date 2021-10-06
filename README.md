# ZetTweaks

Most listed changes can be configured and disable.

Some changes will be disabled if another mod already does that change.

Fix console error spam from TeleShow and NoLockedInteractables.

- Prevent Focus Crystal and Crowbar from increasing self-damage.

- Only place PrimordialTeleporter on last stage of a loop.

- Equipment isn't triggered by Gesture in the Bazaar.

- Recover 50% of health and shields when a Void Cell is activated.

- Start the first 5 stages with enough money to open a chest.

- Combat Director gains 10% more credits for first 5 stages.

- Decaying boost to money earned from kills while multitudes is enabled.

- Adjust teleporter boss drop chance, reducing amount of boss tier items on large reward count.

- Prevent teleporter from locking interactables.

- Prevent eclipse from affecting small holdout zones.

- Adjust base radius and duration of holdout zones:

| HoldoutZone | radius | time |
|--|--|--|
| Teleporter  | 60m -> 90m | 90s |
| Void Cell| 15m -> 20m | 60s -> 45s |
| Pillar of Mass| 20m -> 30m | 60s -> 45s |
| Rescue Ship | 40m -> 60m | 60s -> 45s |

- Pillars required to fight Mithrix changed from 4 to 3.

- Items and equipment don't drop in command essence if there is only one choice.

- Pickups that go out of bounds are teleported back onto the stage.

- Increase Huntress targeting range by 2m every level and prioritize enemies near the crosshair.

## Installation:

Requires Bepinex and HookGenPatcher.

Use r2modman or place inside of Risk of Rain 2/Bepinex/Plugins/

## Changelog:

v1.0.4 - Only place PrimordialTeleporter on last stage of a loop. Recovery when Void Cell is activated. Fix HuntressRange not working with HuntressAutoaimFix.

v1.0.3 - Prevent tweakBossDropRewards from affecting drops with red tier items.

v1.0.2 - Fix TeleShowFix NullRef when a profile was not selected.

v1.0.1 - Split mod out into StatAdjustment and ProcConfig. Gesture doesn't trigger equipment in bazaar.

v1.0.0 - Initial Release.
