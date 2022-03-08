using System;
using System.Linq;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using RoR2;

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetTweaks
{
	[BepInPlugin(ModGuid, ModName, ModVer)]

	public class ZetTweaksPlugin : BaseUnityPlugin
	{
		public const string ModVer = "1.1.2";
		public const string ModName = "ZetTweaks";
		public const string ModGuid = "com.TPDespair.ZetTweaks";


		private static bool lateSetupAttempted = false;
		public static bool lateSetupCompleted = false;

		internal static ArtifactIndex EclipseArtifact = ArtifactIndex.None;

		internal static BodyIndex turret1BodyIndex = BodyIndex.None;
		internal static BodyIndex megaDroneBodyIndex = BodyIndex.None;
		internal static BodyIndex equipDroneBodyIndex = BodyIndex.None;



		public static ConfigFile ConfigFile;

		public static ConfigEntry<bool> AutoCompatCfg { get; set; }
		public static ConfigEntry<bool> FixTeleShowCfg { get; set; }
		public static ConfigEntry<bool> FixNoLockedCfg { get; set; }



		public void Awake()
		{
			RoR2Application.isModded = true;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

			ConfigFile = Config;
			SetupConfig();
			GameplayModule.SetupConfig();

			GameplayModule.Init();
			OnLogBookControllerReady();
			OnMainMenuEnter();
		}

		public void Update()
		{
			if (lateSetupCompleted)
			{
				if (FixTeleShowCfg.Value && TeleShowFix.Enabled) TeleShowFix.SetShouldShow();
			}
		}



		private static void SetupConfig()
		{
			ConfigFile Config = ConfigFile;

			AutoCompatCfg = Config.Bind(
				"0a-Core - Compatibility", "enableAutoCompat", true,
				"Enable Automatic Compatibility. Changes settings based on other installed mods."
			);

			// 0b - Fixes

			FixTeleShowCfg = Config.Bind(
				"0c-Core - ModFixes", "fixTeleShow", true,
				"Fix nullref spam when local player is dead."
			);
			FixNoLockedCfg = Config.Bind(
				"0c-Core - ModFixes", "fixNoLockedInteractables", true,
				"Fix nullref spam when teleporter event is active."
			);
		}



		private static void OnLogBookControllerReady()
		{
			On.RoR2.UI.LogBook.LogBookController.Init += (orig) =>
			{
				try
				{
					if (!lateSetupAttempted)
					{
						lateSetupAttempted = true;

						FindIndexes();
						LateSetup();
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

				orig();
			};
		}

		private static void OnMainMenuEnter()
		{
			On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += (orig, self, controller) =>
			{
				orig(self, controller);

				try
				{
					if (!lateSetupAttempted)
					{
						lateSetupAttempted = true;

						FindIndexes();
						LateSetup();
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			};
		}

		private static void FindIndexes()
		{
			ArtifactIndex artifactIndex = ArtifactCatalog.FindArtifactIndex("ARTIFACT_ZETECLIFACT");
			if (artifactIndex != ArtifactIndex.None) EclipseArtifact = artifactIndex;

			BodyIndex bodyIndex = BodyCatalog.FindBodyIndex("Turret1Body");
			if (bodyIndex != BodyIndex.None) turret1BodyIndex = bodyIndex;
			bodyIndex = BodyCatalog.FindBodyIndex("MegaDroneBody");
			if (bodyIndex != BodyIndex.None) megaDroneBodyIndex = bodyIndex;
			bodyIndex = BodyCatalog.FindBodyIndex("EquipmentDroneBody");
			if (bodyIndex != BodyIndex.None) equipDroneBodyIndex = bodyIndex;
		}

		private static void LateSetup()
		{
			if (FixTeleShowCfg.Value && TeleShowFix.Enabled) TeleShowFix.Init();

			if (GameplayModule.MoneyStageLimitCfg.Value > 0 && !Compat.DisableStarterMoney && ShareSuiteCompat.Enabled) ShareSuiteCompat.Init();

			if (AutoCompatCfg.Value) SetupCompat();

			GameplayModule.LateInit();

			Debug.LogWarning("ZetTweaks - LateSetup Complete!");
			lateSetupCompleted = true;
		}

		private static void SetupCompat()
		{
			if (PluginLoaded("com.Moffein.ReallyBigTeleporterRadius")) Compat.ReallyBigTeleporter = true;
			if (PluginLoaded("com.Cyro.NoLockedInteractables") && FixNoLockedCfg.Value) Compat.UnlockInteractables = true;
			if (PluginLoaded("com.Chen.ChensGradiusMod")) Compat.ChenGradius = true;

			if (PluginLoaded("com.xoxfaby.BetterGameplay"))
			{
				Compat.DisableBazaarGesture = true;
				Compat.DisableTeleportLostDroplet = true;
			}
			if (PluginLoaded("com.Moffein.NoBazaarKickout")) Compat.DisableBazaarPreventKickout = true;
			if (PluginLoaded("com.rob.VoidFieldsQoL")) Compat.DisableVoidHealthHeal = true;
			if (PluginLoaded("com.Anreol.VoidQoL")) Compat.DisableVoidHealthHeal = true;
			if (PluginLoaded("KevinPione.CellVentHeal")) Compat.DisableVoidHealthHeal = true;
			if (PluginLoaded("com.Borbo.BORBO")) Compat.DisableBossDropTweak = true;
			if (PluginLoaded("com.Wolfo.YellowPercent")) Compat.DisableBossDropTweak = true;
			if (PluginLoaded("com.TPDespair.CommandDropletFix")) Compat.DisableCommandDropletFix = true;
			if (PluginLoaded("Withor.SavageHuntress")) Compat.DisableHuntressRange = true;
			if (PluginLoaded("HIFU.HuntressAutoaimFix")) Compat.DisableHuntressAimFix = true;

			// oudated mods ???
			if (PluginLoaded("Rein.GeneralFixes")) Compat.DisableSelfDamageFix = true;
			if (PluginLoaded("_Simon.NoBazaarKickOut")) Compat.DisableBazaarPreventKickout = true;
			if (PluginLoaded("com.FluffyMods.PocketMoney")) Compat.DisableStarterMoney = true;
			if (PluginLoaded("com.TeaBoneJones.IncreaseHuntressRange")) Compat.DisableHuntressRange = true;
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}



	public static class Compat
	{
		public static bool ReallyBigTeleporter = false;
		public static bool UnlockInteractables = false;
		public static bool ChenGradius = false;

		public static bool DisableSelfDamageFix = false;
		public static bool DisableBazaarGesture = false;
		public static bool DisableBazaarPreventKickout = false;
		public static bool DisableVoidHealthHeal = false;
		public static bool DisableStarterMoney = false;
		public static bool DisableBossDropTweak = false;
		public static bool DisableCommandDropletFix = false;
		public static bool DisableTeleportLostDroplet = false;
		public static bool DisableHuntressRange = false;
		public static bool DisableHuntressAimFix = false;
	}
}
