# ZetTweaks

Most listed changes can be configured and disable.

Some changes will be disabled if another mod already does that change.

Fix console error spam from TeleShow and NoLockedInteractables.

## StatModule:

- Base critical strike chance increased from 1% to 5%.

- Players gain an additional jump and 20% movement speed.

- Minimum base health is 120 and is rounded up to a multiple of 15.

- Health growth changed from 30% to 33.3% base health per level.

- Player health regeneration doubled while level multiplier halved.

- Burning now causes health regeneration to halve instead of disabling it.

- Barrier now decays based on current barrier.

- Aegis and Ironclad Affix reduce barrier decay rate.

## GameplayModule:

- Prevent Focus Crystal and Crowbar from increasing self-damage.

- Start the first 5 stages with enough money to open a chest.

- Combat Director gains 10% more credits for first 5 stages.

- Decaying boost to money earned from kills while multitudes is enabled.

- Adjust teleporter boss drop chance, reducing boss item count on large reward count.

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

- Increase Huntress targeting range by 2m every level.

## ProcModule:

- Configure proc-coefficients of most items.

- On-Kill Items proc-coefficients reduced to 0.

- Most other items proc-coefficients reduced to 0.1 - 0.25.

## Installation:

Requires Bepinex and HookGenPatcher.

Use r2modman or place inside of Risk of Rain 2/Bepinex/Plugins/

## Changelog:

v1.0.0 - Initial Release.
