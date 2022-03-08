using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using System;
using UnityEngine;
using UnityEngine.Networking;

using EntityStates.Missions.Arena.NullWard;

namespace TPDespair.ZetTweaks
{
	public static class GameplayModule
	{
		public static ConfigEntry<bool> EnableModuleCfg { get; set; }
		public static ConfigEntry<bool> FixSelfDamageCfg { get; set; }
		public static ConfigEntry<bool> FixMeanderTeleporterCfg { get; set; }
		public static ConfigEntry<bool> BazaarGestureCfg { get; set; }
		public static ConfigEntry<bool> BazaarPreventKickoutCfg { get; set; }
		public static ConfigEntry<float> VoidHealthRecoveryCfg { get; set; }
		public static ConfigEntry<float> VoidShieldRecoveryCfg { get; set; }
		public static ConfigEntry<float> MoneyChestGivenCfg { get; set; }
		public static ConfigEntry<int> MoneyStageLimitCfg { get; set; }
		public static ConfigEntry<float> DirectorMoneyCfg { get; set; }
		public static ConfigEntry<int> DirectorStageLimitCfg { get; set; }
		public static ConfigEntry<bool> MultitudeMoneyCfg { get; set; }
		public static ConfigEntry<bool> BossDropTweakCfg { get; set; }
		public static ConfigEntry<bool> UnlockInteractablesCfg { get; set; }
		public static ConfigEntry<bool> EclipseHoldoutLimitCfg { get; set; }
		public static ConfigEntry<bool> ModifyHoldoutValueCfg { get; set; }
		public static ConfigEntry<float> TeleRadiusCfg { get; set; }
		public static ConfigEntry<float> TeleTimeCfg { get; set; }
		public static ConfigEntry<float> VoidRadiusCfg { get; set; }
		public static ConfigEntry<float> VoidTimeCfg { get; set; }
		public static ConfigEntry<float> VoidBossRadiusCfg { get; set; }
		public static ConfigEntry<float> VoidBossTimeCfg { get; set; }
		public static ConfigEntry<float> MassRadiusCfg { get; set; }
		public static ConfigEntry<float> MassTimeCfg { get; set; }
		public static ConfigEntry<float> ShipRadiusCfg { get; set; }
		public static ConfigEntry<float> ShipTimeCfg { get; set; }
		public static ConfigEntry<int> MoonHoldoutZonesCfg { get; set; }
		public static ConfigEntry<bool> CommandDropletFixCfg { get; set; }
		public static ConfigEntry<bool> TeleportLostDropletCfg { get; set; }
		public static ConfigEntry<bool> ModifyHuntressAimCfg { get; set; }
		public static ConfigEntry<bool> TargetSortAngleCfg { get; set; }
		public static ConfigEntry<float> BaseTargetingRangeCfg { get; set; }
		public static ConfigEntry<float> LevelTargetingRangeCfg { get; set; }
		public static ConfigEntry<bool> DroneFriendlyFireFixCfg { get; set; }
		public static ConfigEntry<float> DroneEquipmentDropCfg { get; set; }
		public static ConfigEntry<bool> DroneTC280AnywhereCfg { get; set; }
		public static ConfigEntry<bool> DroneEquipmentAnywhereCfg { get; set; }
		public static ConfigEntry<bool> DroneTC280RepurchasableCfg { get; set; }
		public static ConfigEntry<bool> DroneTurretRepurchasableCfg { get; set; }



		private static readonly Lazy<GameObject> _backupDroneMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/DroneBackupMaster"));
		private static readonly Lazy<GameObject> _drone1Master = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/Drone1Master"));
		private static readonly Lazy<GameObject> _flameDroneMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/FlameDroneMaster"));
		private static readonly Lazy<GameObject> _missileDroneMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/DroneMissileMaster"));
		private static readonly Lazy<GameObject> _turret1Master = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/Turret1Master"));
		private static readonly Lazy<GameObject> _megaDroneMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster"));
		private static readonly Lazy<GameObject> _equipmentDroneMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/EquipmentDroneMaster"));
		private static readonly Lazy<GameObject> _engiTurretMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/EngiTurretMaster"));
		private static readonly Lazy<GameObject> _engiWalkerTurretMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/EngiWalkerTurretMaster"));
		private static readonly Lazy<GameObject> _engiBeamTurretMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/EngiBeamTurretMaster"));
		private static readonly Lazy<GameObject> _beetleGuardAllyMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/BeetleGuardAllyMaster"));
		private static readonly Lazy<GameObject> _roboBallGreenBuddyMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/roboBallGreenBuddyMaster"));
		private static readonly Lazy<GameObject> _roboBallRedBuddyMaster = new Lazy<GameObject>(() => LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/roboBallRedBuddyMaster"));

		private static readonly Lazy<SpawnCard> _turret1SpawnCard = new Lazy<SpawnCard>(() => LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1"));
		private static readonly Lazy<SpawnCard> _megaDroneSpawnCard = new Lazy<SpawnCard>(() => LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMegaDrone"));
		private static readonly Lazy<SpawnCard> _equipDroneSpawnCard = new Lazy<SpawnCard>(() => LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEquipmentDrone"));

		internal static GameObject BackupDroneMaster { get => _backupDroneMaster.Value; }
		internal static GameObject Drone1Master { get => _drone1Master.Value; }
		internal static GameObject FlameDroneMaster { get => _flameDroneMaster.Value; }
		internal static GameObject MissileDroneMaster { get => _missileDroneMaster.Value; }
		internal static GameObject Turret1Master { get => _turret1Master.Value; }
		internal static GameObject MegaDroneMaster { get => _megaDroneMaster.Value; }
		internal static GameObject EquipmentDroneMaster { get => _equipmentDroneMaster.Value; }
		internal static GameObject EngiTurretMaster { get => _engiTurretMaster.Value; }
		internal static GameObject EngiWalkerTurretMaster { get => _engiWalkerTurretMaster.Value; }
		internal static GameObject EngiBeamTurretMaster { get => _engiBeamTurretMaster.Value; }
		internal static GameObject BeetleGuardAllyMaster { get => _beetleGuardAllyMaster.Value; }
		internal static GameObject RoboBallGreenBuddyMaster { get => _roboBallGreenBuddyMaster.Value; }
		internal static GameObject RoboBallRedBuddyMaster { get => _roboBallRedBuddyMaster.Value; }

		internal static SpawnCard Turret1SpawnCard { get => _turret1SpawnCard.Value; }
		internal static SpawnCard MegaDroneSpawnCard { get => _megaDroneSpawnCard.Value; }
		internal static SpawnCard EquipDroneSpawnCard { get => _equipDroneSpawnCard.Value; }



		internal static void SetupConfig()
		{
			ConfigFile Config = ZetTweaksPlugin.ConfigFile;

			EnableModuleCfg = Config.Bind(
				"2a-Gameplay - Enable", "enableGameplayModule", true,
				"Enable Gameplay Module."
			);

			FixSelfDamageCfg = Config.Bind(
				"2b-Gameplay - Fixes", "fixSelfDamage", true,
				"Prevent Focus Crystal and Crowbar from increasing self damage."
			);
			FixMeanderTeleporterCfg = Config.Bind(
				"2b-Gameplay - Fixes", "endloopTeleporter", true,
				"Always spawn Primordial Teleporter on the last stage of a loop."
			);

			BazaarGestureCfg = Config.Bind(
				"2b-Gameplay - QOL", "disableBazaarGesture", true,
				"Prevent Gesture from firing equipment in Bazaar."
			);
			BazaarPreventKickoutCfg = Config.Bind(
				"2b-Gameplay - QOL", "disableBazaarKickout", true,
				"Prevent damaging the Newt from kicking players from the Bazaar."
			);
			VoidHealthRecoveryCfg = Config.Bind(
				"2b-Gameplay - QOL", "voidHealthRecovery", 0.5f,
				"Recover health fraction when a voidcell is activated. 0 to disable."
			);
			VoidShieldRecoveryCfg = Config.Bind(
				"2b-Gameplay - QOL", "voidShieldRecovery", 0.5f,
				"Recover shield fraction when a voidcell is activated. 0 to disable."
			);

			MoneyChestGivenCfg = Config.Bind(
				"2c-Gameplay - Economy", "startingMoney", 1.25f,
				"Money given at stage start measured in chests."
			);
			MoneyStageLimitCfg = Config.Bind(
				"2c-Gameplay - Economy", "stageMoneyLimit", 5,
				"Last stage to give start money. 0 to disable."
			);
			DirectorMoneyCfg = Config.Bind(
				"2c-Gameplay - Economy", "directorMoney", 1.1f,
				"Combat director money wave amount multiplier."
			);
			DirectorStageLimitCfg = Config.Bind(
				"2c-Gameplay - Economy", "stageDirectorLimit", 5,
				"Last stage to modify combat director money. 0 to disable."
			);
			MultitudeMoneyCfg = Config.Bind(
				"2c-Gameplay - Economy", "multitudeMoneyBoost", true,
				"Decaying boost to money from kills when multitudes is enabled."
			);

			BossDropTweakCfg = Config.Bind(
				"2d-Gameplay - TeleporterBoss", "tweakBossDropRewards", true,
				"Adjust Teleporter boss drop chance when creating ring of drops - reduced at start and increased near end."
			);
			UnlockInteractablesCfg = Config.Bind(
				"2d-Gameplay - TeleporterBoss", "bossUnlockInteractable", true,
				"Prevent Teleporter from locking outside interactables."
			);

			EclipseHoldoutLimitCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "eclipseHoldoutLimiter", true,
				"Prevent Eclipse from shrinking small holdout zones."
			);
			ModifyHoldoutValueCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "modifyHoldoutValues", true,
				"Enable or disable changing radius and charge times of holdout zones except eclipse limiter."
			);
			TeleRadiusCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "teleHoldoutRadius", 90f,
				"Base radius of teleporter holdout zone. Vanilla is 60"
			);
			TeleTimeCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "teleHoldoutTime", 90f,
				"Base duration of teleporter holdout zone. Vanilla is 90"
			);
			VoidRadiusCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "voidHoldoutRadius", 20f,
				"Base radius of void field holdout zone. Vanilla is 15"
			);
			VoidTimeCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "voidHoldoutTime", 45f,
				"Base duration of void field holdout zone. Vanilla is 60"
			);
			VoidBossRadiusCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "voidBossHoldoutRadius", 20f,
				"Base radius of deep void holdout zone. Vanilla is 20"
			);
			VoidBossTimeCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "voidBossHoldoutTime", 45f,
				"Base duration of deep void holdout zone. Vanilla is 60"
			);
			MassRadiusCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "massHoldoutRadius", 30f,
				"Base radius of pillar of mass holdout zone. Vanilla is 20"
			);
			MassTimeCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "massHoldoutTime", 45f,
				"Base duration of pillar of mass holdout zone. Vanilla is 60"
			);
			ShipRadiusCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "shipHoldoutRadius", 60f,
				"Base radius of rescue ship holdout zone. Vanilla is 40"
			);
			ShipTimeCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "shipHoldoutTime", 45f,
				"Base duration of rescue ship holdout zone. Vanilla is 60"
			);
			MoonHoldoutZonesCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "moonHoldoutZones", 3,
				"Pillars required to fight Mithrix. Range : 1 - 4"
			);

			CommandDropletFixCfg = Config.Bind(
				"2f-Gameplay - Droplet", "commandDropletFix", true,
				"No command droplet for items that don't give choices."
			);
			TeleportLostDropletCfg = Config.Bind(
				"2f-Gameplay - Droplet", "teleportLostDroplet", true,
				"Teleport pickup droplets that go out of bounds."
			);

			ModifyHuntressAimCfg = Config.Bind(
				"2g-Gameplay - Skill", "modifyHuntressTargeting", true,
				"Enable or disable changing huntress targeting."
			);
			TargetSortAngleCfg = Config.Bind(
				"2g-Gameplay - Skill", "sortHuntressByAngle", true,
				"Prioritize targets closer to the targeting reticule."
			);
			BaseTargetingRangeCfg = Config.Bind(
				"2g-Gameplay - Skill", "baseHuntressTargetingRange", 60f,
				"Huntress base targeting range. Vanilla is 60"
			);
			LevelTargetingRangeCfg = Config.Bind(
				"2g-Gameplay - Skill", "levelHuntressTargetingRange", 2f,
				"Huntress level targeting range. Vanilla is 0"
			);

			DroneFriendlyFireFixCfg = Config.Bind(
				"2H-Gameplay - Drone", "droneFriendlyFireFix", true,
				"Prevent most drones from targeting allies."
			);
			DroneEquipmentDropCfg = Config.Bind(
				"2H-Gameplay - Drone", "droneEquipmentDrop", 0.25f,
				"Chance for equipment drones to drop their equipment. 1 = 100% chance"
			);
			DroneTC280RepurchasableCfg = Config.Bind(
				"2H-Gameplay - Drone", "droneTC280Repurchasable", true,
				"Make the TC280 repurchasable after it is destroyed."
			);
			DroneTurretRepurchasableCfg = Config.Bind(
				"2H-Gameplay - Drone", "droneTurretRepurchasable", true,
				"Make the Gunner Turret repurchasable after it is destroyed."
			);
			DroneTC280AnywhereCfg = Config.Bind(
				"2H-Gameplay - Drone", "droneTC280Anywhere", true,
				"Allow the TC-280 to spawn anywhere regular drones spawn."
			);
			DroneEquipmentAnywhereCfg = Config.Bind(
				"2H-Gameplay - Drone", "droneEquipmentAnywhere", true,
				"Allow Equipment Drones to spawn anywhere regular drones spawn."
			);
		}

		internal static void Init()
		{
			if (EnableModuleCfg.Value)
			{
				if (FixMeanderTeleporterCfg.Value) PlaceTeleporterHook();
				VoidRecoveryHook();

				if (DirectorStageLimitCfg.Value > 0) DirectorMoneyHook();
				if (MultitudeMoneyCfg.Value) GoldFromKillHook();

				MoonBatteryMissionController.onInstanceChangedGlobal += ChangeRequiredBatteries;

				if (DroneTC280AnywhereCfg.Value || DroneEquipmentAnywhereCfg.Value) SceneDirector.onGenerateInteractableCardSelection += AddMissingDroneSpawnCards;
			}
		}

		internal static void LateInit()
		{
			if (EnableModuleCfg.Value)
			{
				if (FixSelfDamageCfg.Value && !Compat.DisableSelfDamageFix)
				{
					SelfCrowbarHook();
					SelfFocusHook();
				}

				if (BazaarGestureCfg.Value && !Compat.DisableBazaarGesture) BazaarGestureHook();
				if (BazaarPreventKickoutCfg.Value && !Compat.DisableBazaarPreventKickout) BazaarPreventKickoutHook();

				if (MoneyStageLimitCfg.Value > 0 && !Compat.DisableStarterMoney) SceneDirector.onPostPopulateSceneServer += GiveMoney;

				if (BossDropTweakCfg.Value && !Compat.DisableBossDropTweak) BossDropHook();
				if (UnlockInteractablesCfg.Value || Compat.UnlockInteractables) UnlockInteractablesHook();

				if (ModifyHoldoutValueCfg.Value || EclipseHoldoutLimitCfg.Value)
				{
					VoidCellHook();
					HoldoutZoneHook();
				}

				if (CommandDropletFixCfg.Value && !Compat.DisableCommandDropletFix) CommandDropletFix();
				if (TeleportLostDropletCfg.Value && !Compat.DisableTeleportLostDroplet)
				{
					PickupBackgroundCollision();
					PickupTeleportHook();
				}

				if (ModifyHuntressAimCfg.Value)
				{
					if (TargetSortAngleCfg.Value && !Compat.DisableHuntressAimFix) HuntressTargetFix();
					if (!Compat.DisableHuntressRange) HuntressRangeBuff();
				}

				if (!Compat.ChenGradius)
				{
					if (DroneFriendlyFireFixCfg.Value)
					{
						Debug.LogWarning("ZetTweaks - Modifying Drone SkillDrivers");
						ModifyDroneTargeting();
					}

					HandleDroneDeathHook();
				}

				//DroneDecay();
			}
		}



		private static void SelfCrowbarHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				ILLabel jumpTo = null;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(21),
					x => x.MatchLdcI4(0),
					x => x.MatchBle(out jumpTo)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<HealthComponent, CharacterBody, bool>>((healthComponent, atkBody) =>
					{
						if (healthComponent.body == atkBody) return true;

						return false;
					});
					c.Emit(OpCodes.Brtrue, jumpTo);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - SelfCrowbarHook Failed!");
				}
			};
		}

		private static void SelfFocusHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				ILLabel jumpTo = null;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(26),
					x => x.MatchLdcI4(0),
					x => x.MatchBle(out jumpTo)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<HealthComponent, CharacterBody, bool>>((healthComponent, atkBody) =>
					{
						if (healthComponent.body == atkBody) return true;

						return false;
					});
					c.Emit(OpCodes.Brtrue, jumpTo);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - SelfFocusHook Failed!");
				}
			};
		}

		private static void PlaceTeleporterHook()
		{
			On.RoR2.SceneDirector.PlaceTeleporter += (orig, self) =>
			{
				if (self.teleporterSpawnCard != null)
				{
					bool flag = Run.instance.NetworkstageClearCount % Run.stagesPerLoop == Run.stagesPerLoop - 1;
					string cardName = flag ? "iscLunarTeleporter" : "iscTeleporter";

					SpawnCard spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/" + cardName);

					if (spawnCard) self.teleporterSpawnCard = spawnCard;
					else Debug.LogWarning("ZetTweaks [GameplayModule] - interactablespawncard/" + cardName + " could not be found!");
				}

				orig(self);
			};
		}

		private static void BazaarGestureHook()
		{
			IL.RoR2.EquipmentSlot.FixedUpdate += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("AutoCastEquipment")),
					x => x.MatchCall<Inventory>("GetItemCount")
				);

				if (found)
				{
					c.Index += 2;

					c.EmitDelegate<Func<int, int>>((count) =>
					{
						if (SceneInfo.instance.sceneDef.nameToken == "MAP_BAZAAR_TITLE") return 0;

						return count;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - BazaarGestureHook Failed!");
				}
			};
		}

		private static void BazaarPreventKickoutHook()
		{
			On.EntityStates.NewtMonster.KickFromShop.FixedUpdate += (orig, self) =>
			{
				self.outer.SetNextStateToMain();
			};
		}

		private static void VoidRecoveryHook()
		{
			On.RoR2.ArenaMissionController.BeginRound += (orig, self) =>
			{
				orig(self);

				if (NetworkServer.active)
				{
					float healthFraction = VoidHealthRecoveryCfg.Value;
					float shieldFraction = VoidShieldRecoveryCfg.Value;

					if (healthFraction > 0f || shieldFraction > 0f)
					{
						for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
						{
							CharacterBody body = CharacterMaster.readOnlyInstancesList[i].GetBody();
							if (body)
							{
								HealthComponent hc = body.healthComponent;
								if (!Compat.DisableVoidHealthHeal && healthFraction > 0f) hc.Heal(hc.fullHealth * healthFraction, default, true);
								if (shieldFraction > 0f) hc.RechargeShield(hc.fullShield * shieldFraction);
							}
						}
					}
				}
			};
		}

		private static void GiveMoney(SceneDirector director)
		{
			if (Run.instance)
			{
				if (Run.instance.stageClearCount < MoneyStageLimitCfg.Value)
				{
					int scaledCost = Run.instance.GetDifficultyScaledCost(25);
					uint money = 1u + (uint)Mathf.Round(scaledCost * MoneyChestGivenCfg.Value);

					if (ShareSuiteCompat.Enabled && ShareSuiteCompat.moneyMethodFound)
					{
						ShareSuiteCompat.AddMoneyShareSuite((int)money);
					}
					else
					{
						foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
						{
							pcmc.master.GiveMoney(money);
						}
					}
				}
			}
		}

		private static void DirectorMoneyHook()
		{
			IL.RoR2.CombatDirector.Awake += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStfld("RoR2.CombatDirector/DirectorMoneyWave", "multiplier")
				);

				if (found)
				{
					c.EmitDelegate<Func<float, float>>((value) =>
					{
						Run run = Run.instance;
						if (run && run.stageClearCount < DirectorStageLimitCfg.Value) value *= DirectorMoneyCfg.Value;

						return value;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - DirectorMoneyHook Failed!");
				}
			};
		}

		private static void GoldFromKillHook()
		{
			IL.RoR2.DeathRewards.OnKilledServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchCallOrCallvirt<DeathRewards>("get_goldReward"),
					x => x.MatchStloc(2)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, 2);
					c.EmitDelegate<Func<uint, uint>>((reward) =>
					{
						int playerCount = Run.instance.participatingPlayerCount;
						int realPlayerCount = PlayerCharacterMasterController.instances.Count;

						if (playerCount != realPlayerCount)
						{
							int multiplier = playerCount / realPlayerCount;
							float mult = multiplier - 1f;
							int maxStages = multiplier + 2;
							int stageClearCount = Run.instance.stageClearCount;
							float factor = 0.5f + (0.5f / mult) * Math.Min(mult, realPlayerCount - 1);
							factor *= Math.Max(0, maxStages - stageClearCount) / (float)maxStages;

							return (uint)(reward * (mult * factor + 1));
						}

						return reward;
					});
					c.Emit(OpCodes.Stloc, 2);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - GoldFromKillHook Failed!");
				}
			};
		}

		private static void BossDropHook()
		{
			IL.RoR2.BossGroup.DropRewards += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<BossGroup>("bossDropChance")
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 6);
					c.Emit(OpCodes.Ldloc, 2);
					c.EmitDelegate<Func<float, BossGroup, int, int, float>>((rng, bossGroup, i, count) =>
					{
						if (bossGroup.forceTier3Reward) return rng;

						float progress = (i + 1f) / count;
						int left = count - i;
						float chance = 0f;

						if (progress > 0.39f) chance = 0.05f;
						if (progress > 0.59f) chance = 0.10f;
						if (progress > 0.79f) chance = 0.20f;

						if (left <= 4) chance = Mathf.Max(0.15f, chance);
						if (left <= 2) chance = Mathf.Max(0.25f, chance);

						Debug.LogWarning("Drop Reward - " + i + " (" + (i + 1) + " of " + count + ") progress: " + progress + " rng: " + rng + " chance: " + chance + " - " + (rng <= chance));

						if (rng <= chance) return 0f;
						return 1f;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - BossDropHook Failed!");
				}
			};
		}

		private static void UnlockInteractablesHook()
		{
			On.RoR2.OutsideInteractableLocker.OnEnable += (orig, self) => { };
			On.RoR2.OutsideInteractableLocker.OnDisable += (orig, self) => { };
			On.RoR2.OutsideInteractableLocker.FixedUpdate += (orig, self) => { };
		}

		private static void ChangeRequiredBatteries()
		{
			if (MoonBatteryMissionController.instance)
			{
				MoonBatteryMissionController.instance._numRequiredBatteries = Mathf.Clamp(MoonHoldoutZonesCfg.Value, 1, 4);
			}
		}

		private static float VoidCellExitRadius = 15f;

		private static void VoidCellHook()
		{
			On.EntityStates.Missions.Arena.NullWard.NullWardBaseState.OnEnter += (orig, self) =>
			{
				float targetRadius = ModifyHoldoutValueCfg.Value ? VoidRadiusCfg.Value : NullWardBaseState.wardRadiusOn;

				if (Run.instance && EclipseHoldoutLimitCfg.Value)
				{
					bool isEclipse = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse2;
					if (ZetTweaksPlugin.EclipseArtifact != ArtifactIndex.None && RunArtifactManager.instance.IsArtifactEnabled(ZetTweaksPlugin.EclipseArtifact)) isEclipse = true;

					if (isEclipse)
					{
						if (targetRadius <= 20f) targetRadius *= 2f;
						else if (targetRadius < 40f) targetRadius = 40f;
					}
				}

				NullWardBaseState.wardRadiusOn = targetRadius;

				orig(self);
			};

			On.EntityStates.Missions.Arena.NullWard.Active.OnExit += (orig, self) =>
			{
				HoldoutZoneController holdoutZoneController = self.holdoutZoneController;
				if (holdoutZoneController)
				{
					VoidCellExitRadius = holdoutZoneController.currentRadius;

					ChildLocator childLocator = self.childLocator;
					if (childLocator)
					{
						Transform transform = self.childLocator.FindChild("CompleteEffect");
						if (transform)
						{
							float scale = VoidCellExitRadius * 0.933f;
							transform.localScale = new Vector3(scale, scale, scale);
						}
					}
				}

				orig(self);
			};



            IL.EntityStates.Missions.Arena.NullWard.Complete.OnEnter += ReplaceWardRadius;
			IL.EntityStates.Missions.Arena.NullWard.Complete.FixedUpdate += ReplaceWardRadius;
		}

        private static void ReplaceWardRadius(ILContext il)
        {
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(typeof(NullWardBaseState).GetField("wardRadiusOn"))
			);

			if (found)
			{
				c.Index += 1;

				c.Emit(OpCodes.Pop);
				c.EmitDelegate<Func<float>>(() =>
				{
					return Mathf.Max(10f, VoidCellExitRadius);
				});
			}
		}

        private static void HoldoutZoneHook()
		{
			On.RoR2.HoldoutZoneController.Awake += (orig, self) =>
			{
				if (ModifyHoldoutValueCfg.Value) ModifyHoldoutValues(self);

				if (EclipseHoldoutLimitCfg.Value)
				{
					bool isEclipse = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse2;
					if (ZetTweaksPlugin.EclipseArtifact != ArtifactIndex.None && RunArtifactManager.instance.IsArtifactEnabled(ZetTweaksPlugin.EclipseArtifact)) isEclipse = true;

					if (isEclipse)
					{
						if (self.baseRadius <= 20f) self.baseRadius *= 2f;
						else if (self.baseRadius < 40f) self.baseRadius = 40f;
					}
				}

				orig(self);
			};
		}

		private static void ModifyHoldoutValues(HoldoutZoneController self)
		{
			SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
			string sceneName = sceneDefForCurrentScene ? sceneDefForCurrentScene.baseSceneName : "";

			//Debug.LogWarning("Scene - " + sceneName + "]");
			//Debug.LogWarning("HoldoutZone Base - Radius : " + self.baseRadius + "   Duration : " + self.baseChargeDuration);

			if (sceneName == "arena")
			{
				if (self.baseRadius == 15f && self.baseChargeDuration == 60f)
				{
					self.baseRadius = VoidRadiusCfg.Value;
					self.baseChargeDuration = VoidTimeCfg.Value;
				}
			}
			else if (sceneName == "voidstage")
			{
				if (self.baseRadius == 20f && self.baseChargeDuration == 60f)
				{
					self.baseRadius = VoidBossRadiusCfg.Value;
					self.baseChargeDuration = VoidBossTimeCfg.Value;
				}
			}
			else if (sceneName == "moon2")
			{
				// rescue ship
				if (self.baseRadius == 40f && self.baseChargeDuration == 60f)
				{
					self.baseRadius = ShipRadiusCfg.Value;
					self.baseChargeDuration = ShipTimeCfg.Value;
				}

				// pillar of mass
				else if (self.baseRadius == 20f && self.baseChargeDuration == 60f)
				{
					self.baseRadius = MassRadiusCfg.Value;
					self.baseChargeDuration = MassTimeCfg.Value;
				}
				/*
				// pillar of design - pillar of soul
				else if (self.baseRadius == 20f && self.baseChargeDuration == 30f)
				{
					//self.baseRadius = 20f;
					//self.baseChargeDuration = 20f;
				}
				// pillar of blood
				else if (self.baseRadius == 20f && self.baseChargeDuration == 10f)
				{
					//self.baseRadius = 20f;
					//self.baseChargeDuration = 10f;
				}
				*/
			}
			else
			{
				if (self.baseRadius == 60f && self.baseChargeDuration == 90f && !Compat.ReallyBigTeleporter)
				{
					self.baseRadius = TeleRadiusCfg.Value;
					self.baseChargeDuration = TeleTimeCfg.Value;
				}
			}

			//Debug.LogWarning("HoldoutZone New - Radius : " + self.baseRadius + "   Duration : " + self.baseChargeDuration);
		}

        private static void CommandDropletFix()
		{
			IL.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(1),
					x => x.MatchLdloc(1)
				);

				if (found)
				{
					c.Index += 2;

					c.EmitDelegate<Func<PickupDef, PickupDef>>((pickupDef) =>
					{
						if (pickupDef.itemIndex != ItemIndex.None)
						{
							ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
							if (itemDef.ContainsTag(ItemTag.WorldUnique)) return null;
						}

						if (pickupDef.equipmentIndex != EquipmentIndex.None)
						{
							EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex);
							if (!equipDef.canDrop) return null;
						}

						return pickupDef;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - CommandDropletFix Failed!");
				}
			};
		}

		private static void PickupBackgroundCollision()
		{
			// - 15 : Background , 13 : Pickup
			Physics.IgnoreLayerCollision(15, 13, false);
		}

		private static void PickupTeleportHook()
		{
			On.RoR2.MapZone.TryZoneStart += (orig, self, collider) =>
			{
				if (self.zoneType == MapZone.ZoneType.OutOfBounds)
				{
					if (ColliderPickup(collider))
					{
						SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
						spawnCard.hullSize = HullClassification.Human;
						spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
						spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");

						DirectorPlacementRule placementRule = new DirectorPlacementRule
						{
							placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
							position = collider.transform.position
						};

						GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng));

						if (gameObject)
						{
							Debug.Log("tp item back");
							TeleportHelper.TeleportGameObject(collider.gameObject, gameObject.transform.position);
							UnityEngine.Object.Destroy(gameObject);
						}

						UnityEngine.Object.Destroy(spawnCard);
					}
				}

				orig(self, collider);
			};
		}

		private static bool ColliderPickup(Collider collider) {
			if (collider.GetComponent<PickupDropletController>()) return true;
			if (collider.GetComponent<GenericPickupController>()) return true;
			if (collider.GetComponent<PickupPickerController>()) return true;

			return false;
		}

		private static void HuntressTargetFix()
		{
			IL.RoR2.HuntressTracker.SearchForTarget += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcI4(1),
					x => x.MatchStfld<BullseyeSearch>("sortMode")
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 2);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - HuntressTargetFix Failed!");
				}
			};
		}

		private static void HuntressRangeBuff()
		{
			IL.RoR2.HuntressTracker.FixedUpdate += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(MoveType.After,
					x => x.MatchCallOrCallvirt<HuntressTracker>("SearchForTarget")
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<HuntressTracker, float>>((tracker) =>
					{
						float level = tracker.GetComponent<CharacterBody>().level;
						return BaseTargetingRangeCfg.Value + (level - 1f) * LevelTargetingRangeCfg.Value;
					});
					c.Emit(OpCodes.Stfld, typeof(HuntressTracker).GetField("maxTrackingDistance"));
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - HuntressRangeBuff Failed!");
				}
			};
		}

		private static void AddMissingDroneSpawnCards(SceneDirector sceneDirector, DirectorCardCategorySelection dccs)
		{
			int DroneCatagoryIndex = -1;

			bool basicDroneInDeck = false;
			bool megaDroneInDeck = false;
			bool equipDroneInDeck = false;

			for (int i = 0; i < dccs.categories.Length; i++)
			{
				DirectorCardCategorySelection.Category cat = dccs.categories[i];

				if (cat.name == "Drones") DroneCatagoryIndex = i;

				for (int j = cat.cards.Length - 1; j >= 0; j--)
				{
					DirectorCard obj = cat.cards[j];
					/*
					Debug.LogWarning(obj.spawnCard);
					Debug.LogWarning(obj.selectionWeight);
					Debug.LogWarning(obj.spawnDistance);
					Debug.LogWarning(obj.preventOverhead);
					Debug.LogWarning(obj.minimumStageCompletions);
					//*/
					if (!basicDroneInDeck)
					{
						if (obj.spawnCard.name == "iscBrokenDrone1") basicDroneInDeck = true;
						if (obj.spawnCard.name == "iscBrokenDrone2") basicDroneInDeck = true;
					}

					if (obj.spawnCard.name == "iscBrokenMegaDrone") megaDroneInDeck = true;
					if (obj.spawnCard.name == "iscBrokenEquipmentDrone") equipDroneInDeck = true;
				}
			}

			if (DroneCatagoryIndex != -1 && basicDroneInDeck)
			{
				if (DroneTC280AnywhereCfg.Value && !megaDroneInDeck && MegaDroneSpawnCard != null)
				{
					DirectorCard megaDroneDirectorCard = new DirectorCard
					{
						spawnCard = MegaDroneSpawnCard,
						selectionWeight = 1,
						spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
						preventOverhead = false,
						minimumStageCompletions = 1
					};

					Debug.LogWarning("Adding iscBrokenMegaDrone DirectorCard into Deck.");
					dccs.AddCard(DroneCatagoryIndex, megaDroneDirectorCard);
				}

				if (DroneEquipmentAnywhereCfg.Value && !equipDroneInDeck && EquipDroneSpawnCard != null)
				{
					DirectorCard equipDroneDirectorCard = new DirectorCard
					{
						spawnCard = EquipDroneSpawnCard,
						selectionWeight = 2,
						spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
						preventOverhead = false,
						minimumStageCompletions = 0
					};

					Debug.LogWarning("Adding iscBrokenEquipmentDrone DirectorCard into Deck.");
					dccs.AddCard(DroneCatagoryIndex, equipDroneDirectorCard);
				}
			}
		}

		private static void ModifyDroneTargeting()
		{
			AimAtEnemy(Turret1Master);
			AimAtEnemy(EngiTurretMaster);
			AimAtEnemy(EngiWalkerTurretMaster);
			AimAtEnemy(EngiBeamTurretMaster);
			AimAtEnemy(Drone1Master);
			AimAtEnemy(MegaDroneMaster);
			AimAtEnemy(MissileDroneMaster);
			AimAtEnemy(BackupDroneMaster);
			AimAtEnemy(FlameDroneMaster);
			AimAtEnemy(EquipmentDroneMaster);
			AimAtEnemy(BeetleGuardAllyMaster);
			AimAtEnemy(RoboBallGreenBuddyMaster);
			AimAtEnemy(RoboBallRedBuddyMaster);
		}

		public static void AimAtEnemy(GameObject masterObject)
		{
			AimAtEnemy(masterObject.GetComponents<AISkillDriver>());
		}

		public static void AimAtEnemy(AISkillDriver[] skillDrivers)
		{
			foreach (var skillDriver in skillDrivers) skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
		}
		
		private static void DroneDecay()
		{
			On.RoR2.CharacterMaster.OnBodyStart += (orig, master, body) =>
			{
				if (NetworkServer.active && master && body)
				{
					if (body.bodyIndex == ZetTweaksPlugin.megaDroneBodyIndex)
					{
						master.inventory.GiveItem(RoR2Content.Items.HealthDecay, 10);
					}
					if (body.bodyIndex == ZetTweaksPlugin.equipDroneBodyIndex)
					{
						master.inventory.GiveItem(RoR2Content.Items.HealthDecay, 10);
					}
				}

				orig(master, body);
			};
		}
		
		private static void HandleDroneDeathHook()
		{
			IL.RoR2.CharacterMaster.OnBodyDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				c.Index = 0;

				c.Emit(OpCodes.Ldarg, 1);
				c.Emit(OpCodes.Ldarg, 0);
				c.EmitDelegate<Action<CharacterBody, CharacterMaster>>((body, master) =>
				{
					OnDroneBodyDeath(master, body);
				});
			};
		}

		private static void OnDroneBodyDeath(CharacterMaster master, CharacterBody body)
		{
			if (NetworkServer.active && master && body)
			{
				if (master.IsDeadAndOutOfLivesServer())
				{
					BodyIndex bodyIndex = body.bodyIndex;
					if (bodyIndex != BodyIndex.None)
					{
						if (bodyIndex == ZetTweaksPlugin.turret1BodyIndex)
						{
							if (DroneTurretRepurchasableCfg.Value && Turret1SpawnCard != null) SpawnDrone(Turret1SpawnCard, body);
						}
						if (bodyIndex == ZetTweaksPlugin.megaDroneBodyIndex)
						{
							if (DroneTC280RepurchasableCfg.Value && MegaDroneSpawnCard != null) SpawnDrone(MegaDroneSpawnCard, body);
						}
						if (bodyIndex == ZetTweaksPlugin.equipDroneBodyIndex)
						{
							if (Util.CheckRoll(DroneEquipmentDropCfg.Value * 100f))
							{
								EquipmentIndex equipIndex = master.inventory.currentEquipmentIndex;
								if (equipIndex != EquipmentIndex.None)
								{
									PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(equipIndex);

									GenericPickupController.CreatePickupInfo pickupInfo = new GenericPickupController.CreatePickupInfo
									{
										position = body.corePosition,
										rotation = Quaternion.identity,
										pickupIndex = pickupIndex
									};

									GenericPickupController.CreatePickup(pickupInfo);

									master.inventory.SetEquipmentIndex(EquipmentIndex.None);
								}
							}
						}
					}
				}
			}
		}

		private static void SpawnDrone(SpawnCard spawnCard, CharacterBody body)
		{
			DirectorPlacementRule placementRule = new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
				position = body.corePosition,
			};

			GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0UL)));
			if (gameObject)
			{
				PurchaseInteraction purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
				if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money)
				{
					purchaseInteraction.Networkcost = Run.instance.GetDifficultyScaledCost(purchaseInteraction.cost);
				}

				if (body.transform) gameObject.transform.rotation = body.transform.rotation;
			}
		}
	}
}
