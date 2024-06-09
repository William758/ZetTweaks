using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using EntityStates.Missions.Arena.NullWard;
using System.Reflection;
using BepInEx;

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
		public static ConfigEntry<int> RecolorLunarEquipmentCfg { get; set; }
		public static ConfigEntry<float> MoneyChestGivenCfg { get; set; }
		public static ConfigEntry<int> MoneyStageLimitCfg { get; set; }
		public static ConfigEntry<float> BloodShrineScaleCfg { get; set; }
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
		public static ConfigEntry<int> VoidBossHoldoutZonesCfg { get; set; }
		public static ConfigEntry<bool> CommandDropletFixCfg { get; set; }
		public static ConfigEntry<bool> CleanPickerOptionsCfg { get; set; }
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
		public static ConfigEntry<bool> ModifyChanceShrineCfg { get; set; }
		public static ConfigEntry<int> ChanceShrineCountCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineTimerCfg { get; set; }
		public static ConfigEntry<int> ChanceShrineCostCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineCostMultCfg { get; set; }
		public static ConfigEntry<int> ChanceShrineMaxFailCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineFailureCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineFailureMultCfg { get; set; }
		public static ConfigEntry<bool> ChanceShrineHackedLockCfg { get; set; }
		public static ConfigEntry<bool> ChanceShrineBypassDropTableCfg { get; set; }
		public static ConfigEntry<bool> ChanceShrineLunarConversionCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineEquipmentCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineEquipmentMultCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineCommonCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineCommonMultCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineUncommonCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineUncommonMultCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineLegendaryCfg { get; set; }
		public static ConfigEntry<float> ChanceShrineLegendaryMultCfg { get; set; }



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

		internal static GameObject LunarEquipmentDropletPrefab;



		private static bool AspectsResolved = false;
		internal static bool AspectCommandGroupItems = false;
		internal static bool AspectCommandGroupEquip = false;
		internal static List<ItemIndex> aspectItemIndexes = new List<ItemIndex>();
		internal static List<EquipmentIndex> aspectEquipIndexes = new List<EquipmentIndex>();



		internal static void SetupConfig()
		{
			ConfigFile Config = ZetTweaksPlugin.ConfigFile;

			EnableModuleCfg = Config.Bind(
				"2a-Gameplay - Enable", "enableGameplayModule", true,
				"Enable Gameplay Module."
			);

			FixSelfDamageCfg = Config.Bind(
				"2b-Gameplay - Fixes", "fixSelfDamage", true,
				"Prevent Focus Crystal, Crowbar and DelicateWatch from increasing self damage."
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

			RecolorLunarEquipmentCfg = Config.Bind(
				"2b-Gameplay - Tweak", "recolorLunarEquipment", 1,
				"Change the color of lunar equipment droplets. 0 = Disabled, 1 = AutoCompat, 2 = Force Enabled"
			);

			MoneyChestGivenCfg = Config.Bind(
				"2c-Gameplay - Economy", "startingMoney", 1.25f,
				"Money given at stage start (measured in chests)."
			);
			MoneyStageLimitCfg = Config.Bind(
				"2c-Gameplay - Economy", "stageMoneyLimit", 5,
				"Last stage to give start money. 0 to disable."
			);
			BloodShrineScaleCfg = Config.Bind(
				"2c-Gameplay - Economy", "bloodShrineScale", 2f,
				"Minimum money given (measured in chests) when blood shrine takes 100% health. 0 to disable."
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
				"2e-Gameplay - HoldoutZone", "voidBossHoldoutRadius", 30f,
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
			VoidBossHoldoutZonesCfg = Config.Bind(
				"2e-Gameplay - HoldoutZone", "voidBossHoldoutZones", 3,
				"Pillars required to fight Voidling. Range : 1 - 4"
			);

			CommandDropletFixCfg = Config.Bind(
				"2f-Gameplay - Droplet", "commandDropletFix", true,
				"No command droplet for items that don't give choices."
			);
			CleanPickerOptionsCfg = Config.Bind(
				"2f-Gameplay - Droplet", "cleanPickerOptions", true,
				"Remove disabled-unobtainable items from command options."
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

			ModifyChanceShrineCfg = Config.Bind(
				"2i-Gameplay - Shrine", "modifyChanceShrine", true,
				"Modify chance shrine behavior."
			);
			ChanceShrineCountCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineCount", 2,
				"Maximum successful purchases from a chance shrine."
			);
			ChanceShrineTimerCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineTimer", 1f,
				"Time between chance shrine uses. Vanilla is 2"
			);
			ChanceShrineCostCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineCost", 17,
				"Base cost of chance shrine. Vanilla is 17"
			);
			ChanceShrineCostMultCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineCostMult", 1.4f,
				"Cost multiplier every time chance shrine is used. Vanilla is 1.4"
			);
			ChanceShrineMaxFailCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineMaxFail", 5,
				"Consecutive failure effects dont scale past this amount."
			);
			ChanceShrineFailureCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineFailure", 0.45f,
				"Base fail chance of chance shrine. Vanilla is 0.45 = 45%"
			);
			ChanceShrineFailureMultCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineFailureMult", 0.9f,
				"Fail chance multiplier for each consecutive chance shrine failure."
			);
			ChanceShrineHackedLockCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineHackedLock", true,
				"Lock consecutive failure count if price is zero."
			);
			ChanceShrineBypassDropTableCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineBypassDropTable", true,
				"Use custom item tier weights to roll items instead of the default drop table."
			);
			ChanceShrineLunarConversionCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineLunarConversion", true,
				"Allow Eulogy Zero to convert rewards to lunar when using custom weights."
			);
			ChanceShrineEquipmentCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineEquipment", 9f,
				"Base equipment drop weight from shrine reward. vanilla is 9"
			);
			ChanceShrineEquipmentMultCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineEquipmentMult", 0.75f,
				"Equipment drop weight multiplier for each consecutive chance shrine failure."
			);
			ChanceShrineCommonCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineCommon", 36f,
				"Base common drop weight from shrine reward. vanilla is 36"
			);
			ChanceShrineCommonMultCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineCommonMult", 0.75f,
				"Common drop weight multiplier for each consecutive chance shrine failure."
			);
			ChanceShrineUncommonCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineUncommon", 9f,
				"Base uncommon drop weight from shrine reward. vanilla is 9"
			);
			ChanceShrineUncommonMultCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineUncommonMult", 1.5f,
				"Uncommon drop weight multiplier for each consecutive chance shrine failure."
			);
			ChanceShrineLegendaryCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineLegendary", 1f,
				"Base legendary drop weight from shrine reward. vanilla is 1"
			);
			ChanceShrineLegendaryMultCfg = Config.Bind(
				"2i-Gameplay - Shrine", "chanceShrineLegendaryMult", 2f,
				"Legendary drop weight multiplier for each consecutive chance shrine failure."
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
				On.RoR2.VoidStageMissionController.Start += ChangeRequiredDeepVoidCells;

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
					if (!Compat.Risky) SelfFocusHook();
					SelfWatchHook();
				}

				if (BazaarGestureCfg.Value && !Compat.DisableBazaarGesture) BazaarGestureHook();
				if (BazaarPreventKickoutCfg.Value && !Compat.DisableBazaarPreventKickout) BazaarPreventKickoutHook();

				if (RecolorLunarEquipmentCfg.Value == 2 || (RecolorLunarEquipmentCfg.Value == 1 && !Compat.WolfoQol))
				{
					RecolorLunarEquipment();
				}

				if (MoneyStageLimitCfg.Value > 0 && !Compat.DisableStarterMoney) SceneDirector.onPostPopulateSceneServer += GiveMoney;

				if (BloodShrineScaleCfg.Value > 0f && !Compat.DisableBloodShrineScale)
				{
					ModifyBloodShrineReward();
				}

				if (BossDropTweakCfg.Value && !Compat.DisableBossDropTweak) BossRewardChanceHook();
				if (UnlockInteractablesCfg.Value || Compat.UnlockInteractables) UnlockInteractablesHook();

				if (ModifyHoldoutValueCfg.Value || EclipseHoldoutLimitCfg.Value)
				{
					VoidCellHook();
					HoldoutZoneHook();
				}

				if (CommandDropletFixCfg.Value && !Compat.DisableCommandDropletFix) CommandDropletFix();

				if (CleanPickerOptionsCfg.Value) CleanPickerOptionsHook();
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
						Debug.LogWarning("ZetTweaks - Modifying Drone AI");
						ModifyDroneAI();
					}

					HandleDroneDeathHook();
				}

				if (!Compat.DisableChanceShrine)
				{
					if (ModifyChanceShrineCfg.Value)
					{
						ChanceShrineAwakeHook();
						ChanceShrineTimerHook();
						ChanceShrineDropHook();
					}
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
					x => x.MatchLdloc(19),
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
					Debug.LogWarning("ZetTweaks - SelfCrowbarHook Failed!");
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
					x => x.MatchLdloc(24),
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
					Debug.LogWarning("ZetTweaks - SelfFocusHook Failed!");
				}
			};
		}

		private static void SelfWatchHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				ILLabel jumpTo = null;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(25),
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
					Debug.LogWarning("ZetTweaks - SelfWatchHook Failed!");
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
					else Debug.LogWarning("ZetTweaks - interactablespawncard/" + cardName + " could not be found!");
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
					Debug.LogWarning("ZetTweaks - BazaarGestureHook Failed!");
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

		private static void RecolorLunarEquipment()
		{
			CreateLunarEquipmentPrefab();

			foreach (EquipmentIndex equipIndex in EquipmentCatalog.equipmentList)
			{
				EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(equipIndex);
				if (equipDef && equipDef.isLunar)
				{
					if (!(Compat.ZetAspects && equipDef == RoR2Content.Equipment.AffixLunar))
					{
						ColorEquipmentDroplet(equipDef);
					}
				}
			}
		}

		private static void CreateLunarEquipmentPrefab()
		{
			LunarEquipmentDropletPrefab = ZetTweaksPlugin.ClonePrefab(LegacyResourcesAPI.Load<GameObject>("Prefabs/itempickups/LunarOrb"), "ZT_LunarEquipmentOrb");

			TrailRenderer trail = LunarEquipmentDropletPrefab.transform.GetComponentInChildren<TrailRenderer>();
			trail.startColor = new Color(0.3f, 0.45f, 0.9f, 0f);
			trail.endColor = new Color(0.2f, 0.3f, 0.9f);
		}

		private static void ColorEquipmentDroplet(EquipmentDef equipDef)
		{
			if (equipDef)
			{
				PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));
				if (LunarEquipmentDropletPrefab) pickupDef.dropletDisplayPrefab = LunarEquipmentDropletPrefab;
				pickupDef.baseColor = new Color(0.45f, 0.6f, 0.9f);
				pickupDef.darkColor = new Color(0.45f, 0.6f, 0.9f);
			}
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
					Debug.LogWarning("ZetTweaks - DirectorMoneyHook Failed!");
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
							float mult = Mathf.Max(1f, multiplier - 1f);
							int maxStages = multiplier + 2;
							int stageClearCount = Run.instance.stageClearCount;
							float factor = 0.5f + (0.5f / mult) * Math.Min(mult, realPlayerCount - 1);
							factor *= Math.Max(0, maxStages - stageClearCount) / (float)maxStages;

							uint newReward = (uint)(reward * (mult * factor + 1));

							if (newReward > reward)
							{
								return newReward;
							}
						}

						return reward;
					});
					c.Emit(OpCodes.Stloc, 2);
				}
				else
				{
					Debug.LogWarning("ZetTweaks - GoldFromKillHook Failed!");
				}
			};
		}

		private static void ModifyBloodShrineReward()
		{
			IL.RoR2.ShrineBloodBehavior.AddShrineStack += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(1)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<uint, ShrineBloodBehavior, uint>>((money, shrine) =>
					{
						if (Run.instance)
						{
							uint minReward = (uint)(Run.instance.GetDifficultyScaledCost(25) * (shrine.purchaseInteraction.cost / 100f) * BloodShrineScaleCfg.Value);

							if (minReward > money) return minReward;
						}

						return money;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks - ModifyBloodShrineReward Failed!");
				}
			};
		}

		private static void BossRewardChanceHook()
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
					Debug.LogWarning("ZetTweaks - BossRewardChanceHook Failed!");
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

		private static void ChangeRequiredDeepVoidCells(On.RoR2.VoidStageMissionController.orig_Start orig, VoidStageMissionController self)
		{
			if (self && NetworkServer.active)
			{
				self.batteryCount = Mathf.Clamp(VoidBossHoldoutZonesCfg.Value, 1, 4);
			}

			orig(self);
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
			IL.RoR2.PickupDropletController.CreatePickup += (il) =>
			{
				ILCursor c = new ILCursor(il);

				MethodInfo methodInfo = typeof(PickupCatalog).GetMethod("GetPickupDef");

				bool found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt(methodInfo),
					x => x.MatchStloc(out _),
					x => x.MatchLdloc(out _)
				);

				if (found)
				{
					c.Index += 3;

					c.EmitDelegate<Func<PickupDef, PickupDef>>((pickupDef) =>
					{
						if (Compat.ZetAspects && !AspectsResolved) ResolveAspects();

						if (pickupDef.itemIndex != ItemIndex.None && (!Compat.ZetAspects || !AspectCommandGroupItems || !aspectItemIndexes.Contains(pickupDef.itemIndex)))
						{
							ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
							if (itemDef.ContainsTag(ItemTag.WorldUnique)) return null;
						}

						if (pickupDef.equipmentIndex != EquipmentIndex.None && (!Compat.ZetAspects || !AspectCommandGroupEquip || !aspectEquipIndexes.Contains(pickupDef.equipmentIndex)))
						{
							EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex);
							if (!equipDef.canDrop) return null;
						}

						return pickupDef;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks - CommandDropletFix Failed!");
				}
			};
		}

		private static void ResolveAspects()
		{
			BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

			BaseUnityPlugin Plugin = BepInEx.Bootstrap.Chainloader.PluginInfos["com.TPDespair.ZetAspects"].Instance;
			Assembly PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			Type type = Type.GetType("TPDespair.ZetAspects.Configuration, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				MethodInfo methodInfo = type.GetMethod("get_AspectCommandGroupItems", Flags);
				if (methodInfo != null)
				{
					try
					{
						ConfigEntry<bool> config = (ConfigEntry<bool>)methodInfo.Invoke(null, null);
						AspectCommandGroupItems = config.Value;
					}
					catch (Exception e)
					{
						Debug.LogError(e);
					}
				}
				else
				{
					Debug.LogWarning("[ZetTweaks - ResolveAspects] - Could Not Find Method : Configuration.get_AspectCommandGroupItems");
				}

				methodInfo = type.GetMethod("get_AspectCommandGroupEquip", Flags);
				if (methodInfo != null)
				{
					try
					{
						ConfigEntry<bool> config = (ConfigEntry<bool>)methodInfo.Invoke(null, null);
						AspectCommandGroupEquip = config.Value;
					}
					catch (Exception e)
					{
						Debug.LogError(e);
					}
				}
				else
				{
					Debug.LogWarning("[ZetTweaks - ResolveAspects] - Could Not Find Method : Configuration.get_AspectCommandGroupEquip");
				}
			}
			else
			{
				Debug.LogWarning("[ZetTweaks - ResolveAspects] Could Not Find Type : TPDespair.ZetAspects.Configuration");
			}



			if (AspectCommandGroupItems || AspectCommandGroupEquip)
			{
				type = Type.GetType("TPDespair.ZetAspects.Catalog, " + PluginAssembly.FullName, false);
				if (type != null)
				{
					if (AspectCommandGroupItems)
					{
						FieldInfo fieldInfo = type.GetField("aspectItemIndexes", Flags);
						if (fieldInfo != null)
						{
							try
							{
								aspectItemIndexes = (List<ItemIndex>)fieldInfo.GetValue(null);
							}
							catch (Exception e)
							{
								Debug.LogError(e);
							}
						}
						else
						{
							Debug.LogWarning("[ZetTweaks - ResolveAspects] - Could Not Find Field : Catalog.aspectItemIndexes");
						}
					}

					if (AspectCommandGroupEquip)
					{
						FieldInfo fieldInfo = type.GetField("aspectEquipIndexes", Flags);
						if (fieldInfo != null)
						{
							try
							{
								aspectEquipIndexes = (List<EquipmentIndex>)fieldInfo.GetValue(null);
							}
							catch (Exception e)
							{
								Debug.LogError(e);
							}
						}
						else
						{
							Debug.LogWarning("[ZetTweaks - ResolveAspects] - Could Not Find Field : Catalog.aspectEquipIndexes");
						}
					}
				}
				else
				{
					Debug.LogWarning("[ZetTweaks - ResolveAspects] Could Not Find Type : TPDespair.ZetAspects.Catalog");
				}
			}

			AspectsResolved = true;
		}

		private static void CleanPickerOptionsHook()
		{
			On.RoR2.PickupPickerController.GetOptionsFromPickupIndex += (orig, pickupIndex) =>
			{
				PickupPickerController.Option[] options = orig(pickupIndex);

				return CleanupOptions(options);
			};
		}

		private static PickupPickerController.Option[] CleanupOptions(PickupPickerController.Option[] options)
		{
			List<PickupPickerController.Option> optionList = new List<PickupPickerController.Option>();

			for (int i = 0; i < options.Length; i++)
			{
				bool valid = true;
				PickupPickerController.Option option = options[i];

				PickupDef pickupDef = PickupCatalog.GetPickupDef(option.pickupIndex);
				if (pickupDef.itemIndex != ItemIndex.None)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);

					if (itemDef.tier == ItemTier.NoTier && itemDef.ContainsTag(ItemTag.WorldUnique)) valid = false;
				}
				else if (pickupDef.equipmentIndex != EquipmentIndex.None)
				{
					EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex);

					if (equipDef.canDrop == false && equipDef.appearsInSinglePlayer == false && equipDef.appearsInMultiPlayer == false) valid = false;
				}

				if (valid) optionList.Add(option);
			}

			return optionList.ToArray();
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

		private static bool ColliderPickup(Collider collider)
		{
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
					Debug.LogWarning("ZetTweaks - HuntressTargetFix Failed!");
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
					Debug.LogWarning("ZetTweaks - HuntressRangeBuff Failed!");
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

		private static void ModifyDroneAI()
		{
			ModifyAI(Turret1Master);
			ModifyAI(EngiTurretMaster);
			ModifyAI(EngiWalkerTurretMaster);
			ModifyAI(EngiBeamTurretMaster);
			ModifyAI(Drone1Master);
			ModifyAI(MegaDroneMaster);
			ModifyAI(MissileDroneMaster);
			ModifyAI(BackupDroneMaster);
			ModifyAI(FlameDroneMaster);
			ModifyAI(EquipmentDroneMaster);
			ModifyAI(BeetleGuardAllyMaster);
			ModifyAI(RoboBallGreenBuddyMaster);
			ModifyAI(RoboBallRedBuddyMaster);
		}

		public static void ModifyAI(GameObject masterObject)
		{
			AimAtEnemy(masterObject.GetComponents<AISkillDriver>());
			DontRetaliate(masterObject.GetComponents<BaseAI>());
		}

		public static void AimAtEnemy(AISkillDriver[] skillDrivers)
		{
			foreach (var skillDriver in skillDrivers) skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
		}

		public static void DontRetaliate(BaseAI[] baseAIs)
		{
			foreach (var baseAI in baseAIs) baseAI.neverRetaliateFriendlies = true;
		}
		/*
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
		*/
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



		private static void ChanceShrineAwakeHook()
		{
			On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
			{
				bool isRedShrine = self.maxPurchaseCount == 1 && self.failureChance >= 0.945f;

				if (!isRedShrine)
				{
					self.maxPurchaseCount = ChanceShrineCountCfg.Value;
					self.costMultiplierPerPurchase = ChanceShrineCostMultCfg.Value;
					self.refreshTimer = ChanceShrineTimerCfg.Value;
				}

				orig(self);

				if (!isRedShrine)
				{
					PurchaseInteraction interaction = self.purchaseInteraction;
					if (interaction)
					{
						interaction.cost = ChanceShrineCostCfg.Value;
					}
					else
					{
						Debug.LogWarning("ZetTweaks - ChanceShrineAwakeHook : Could not set base cost!");
					}
				}
			};
		}

		private static void ChanceShrineTimerHook()
		{
			IL.RoR2.ShrineChanceBehavior.AddShrineStack += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStfld<ShrineChanceBehavior>("refreshTimer")
				);

				if (found)
				{
					c.Emit(OpCodes.Pop);
					c.EmitDelegate<Func<float>>(() =>
					{
						return ChanceShrineTimerCfg.Value;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks - ChanceShrineTimerHook Failed!");
				}
			};
		}

		private static void ChanceShrineDropHook()
		{
			IL.RoR2.ShrineChanceBehavior.AddShrineStack += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(0),
					x => x.MatchLdsfld<PickupIndex>("none"),
					x => x.MatchCall(out _),
					x => x.MatchStloc(1)
				);

				if (found)
				{
					c.Index += 4;

					ILLabel bypassDropGen = c.MarkLabel();

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<ShrineChanceBehavior, PickupIndex>>((self) =>
					{
						return GenerateDrop(self);
					});
					c.Emit(OpCodes.Stloc, 0);

					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<PickupIndex, bool>>((pickupIndex) =>
					{
						return pickupIndex == PickupIndex.none;
					});
					c.Emit(OpCodes.Stloc, 1);

					c.Index = 0;

					found = c.TryGotoNext(
						x => x.MatchLdsfld<PickupIndex>("none"),
						x => x.MatchStloc(0)
					);
					
					if (found)
					{
						if (bypassDropGen != null)
						{
							c.Index += 2;

							c.Emit(OpCodes.Br, bypassDropGen.Target);
						}
						else
						{
							Debug.LogWarning("ZetTweaks - ChanceShrineDropHook:Illabel is null!");
						}
					}
					else
					{
						Debug.LogWarning("ZetTweaks - ChanceShrineDropHook:Bypass Failed!");
					}
					
				}
				else
				{
					Debug.LogWarning("ZetTweaks - ChanceShrineDropHook:BypassSetup Failed!");
				}
			};
		}

		private static PickupIndex GenerateDrop(ShrineChanceBehavior self)
		{
			ChanceShrineTracker tracker = self.GetComponent<ChanceShrineTracker>();
			if (!tracker)
			{
				tracker = self.gameObject.AddComponent<ChanceShrineTracker>();
			}

			bool isRedShrine = self.maxPurchaseCount == 1 && self.failureChance >= 0.945f;
			bool lockFailCount = ChanceShrineHackedLockCfg.Value && self.purchaseInteraction.Networkcost == 0;

			int failCount = tracker ? tracker.consecutiveFailure : 0;
			failCount = Mathf.Max(0, Mathf.Min(failCount, ChanceShrineMaxFailCfg.Value));

			float successChance = Mathf.Max(1f - (ChanceShrineFailureCfg.Value * Mathf.Pow(ChanceShrineFailureMultCfg.Value, failCount)), 0.05f);
			if (isRedShrine) successChance = 1f - self.failureChance;

			if (self.rng.nextNormalizedFloat > successChance)
			{
				// - Failure

				if (!isRedShrine)
				{
					if (tracker && !lockFailCount)
					{
						tracker.consecutiveFailure++;
						//Debug.LogWarning("ZetTweaks [ChanceShrineTracker] - ConFail : " + tracker.consecutiveFailure + " FailChance : " + failChance);
					}

					self.shrineColor = new Color(0.35f, 0.35f, 0.35f);
				}

				return PickupIndex.none;
			}



			PickupIndex pickupIndex = PickupIndex.none;

			if ((!ChanceShrineBypassDropTableCfg.Value || isRedShrine) && self.dropTable)
			{
				pickupIndex = self.dropTable.GenerateDrop(self.rng);
			}
			else
			{
				float equipmentWeight = ChanceShrineEquipmentCfg.Value * Mathf.Pow(ChanceShrineEquipmentMultCfg.Value, failCount);
				float commonWeight = ChanceShrineCommonCfg.Value * Mathf.Pow(ChanceShrineCommonMultCfg.Value, failCount);
				float uncommonWeight = ChanceShrineUncommonCfg.Value * Mathf.Pow(ChanceShrineUncommonMultCfg.Value, failCount);
				float legendaryWeight = ChanceShrineLegendaryCfg.Value * Mathf.Pow(ChanceShrineLegendaryMultCfg.Value, failCount);

				PickupIndex equipment = self.rng.NextElementUniform(Run.instance.availableEquipmentDropList);
				PickupIndex common = self.rng.NextElementUniform(Run.instance.availableTier1DropList);
				PickupIndex uncommon = self.rng.NextElementUniform(Run.instance.availableTier2DropList);
				PickupIndex legendary = self.rng.NextElementUniform(Run.instance.availableTier3DropList);

				WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>();
				weightedSelection.AddChoice(equipment, equipmentWeight);
				weightedSelection.AddChoice(common, commonWeight);
				weightedSelection.AddChoice(uncommon, uncommonWeight);
				weightedSelection.AddChoice(legendary, legendaryWeight);
				/*
				ConvertToChance(ref equipmentWeight, ref commonWeight, ref uncommonWeight, ref legendaryWeight);
				Debug.LogWarning("ZetTweaks [ChanceShrine] - Chance : " + $"Equip : {equipmentWeight:0.#}% , " + $"T1 : {commonWeight:0.#}% , " + $"T2 : {uncommonWeight:0.#}% , " + $"T3 : {legendaryWeight:0.#}%");
				*/
				pickupIndex = weightedSelection.Evaluate(self.rng.nextNormalizedFloat);

				if (ChanceShrineLunarConversionCfg.Value)
				{
					pickupIndex = RoR2.Items.RandomlyLunarUtils.CheckForLunarReplacement(pickupIndex, self.rng);
				}
			}

			if (!isRedShrine)
			{
				Color color = new Color(0.35f, 0.35f, 0.35f);

				if (pickupIndex != PickupIndex.none)
				{
					if (tracker && !lockFailCount)
					{
						tracker.consecutiveFailure = 0;
						//Debug.LogWarning("ZetTweaks [ChanceShrineTracker] - ConFail : 0");
					}

					PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
					if (pickupDef != null)
					{
						if (pickupDef.equipmentIndex != EquipmentIndex.None)
						{
							if (pickupDef.isLunar)
							{
								color = new Color(0f, 0.5f, 1f);
							}
							else
							{
								color = new Color(1f, 0.5f, 0f);
							}
						}
						else
						{
							ItemTier itemTier = pickupDef.itemTier;
							switch (itemTier)
							{
								case ItemTier.Tier1:
									color = Color.white;
									break;
								case ItemTier.Tier2:
									color = Color.green;
									break;
								case ItemTier.Tier3:
									color = Color.red;
									break;
								case ItemTier.Lunar:
									color = new Color(0f, 0.5f, 1f);
									break;
								default:
									color = new Color(0.35f, 0.35f, 0.35f);
									break;
							}
						}
					}
				}

				self.shrineColor = color;
			}

			return pickupIndex;
		}
		/*
		private static void ConvertToChance(ref float num1, ref float num2, ref float num3, ref float num4)
		{
			float divisor = (num1 + num2 + num3 + num4) * 0.01f;

			num1 /= divisor;
			num2 /= divisor;
			num3 /= divisor;
			num4 /= divisor;
		}
		*/
	}



	public class ChanceShrineTracker : MonoBehaviour
	{
		public int consecutiveFailure = 0;
	}
}
