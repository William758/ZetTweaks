using System.Linq;
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
		public const string ModVer = "1.0.4";
		public const string ModName = "ZetTweaks";
		public const string ModGuid = "com.TPDespair.ZetTweaks";


		public static bool lateSetupCompleted = false;

		internal static ArtifactIndex EclipseArtifact = ArtifactIndex.None;



		internal static ConfigFile ConfigFile;

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
				FindIndexes();
				LateSetup();

				orig();
			};
		}

		private static void FindIndexes()
		{
			ArtifactIndex artifactIndex = ArtifactCatalog.FindArtifactIndex("ARTIFACT_ZETECLIFACT");
			if (artifactIndex != ArtifactIndex.None) EclipseArtifact = artifactIndex;
		}

		private static void LateSetup()
		{
			if (FixTeleShowCfg.Value && TeleShowFix.Enabled) TeleShowFix.Init();

			if (AutoCompatCfg.Value) SetupCompat();

			GameplayModule.LateInit();

			lateSetupCompleted = true;
		}

		private static void SetupCompat()
		{
			if (PluginLoaded("com.rob.VoidFieldsQoL")) Compat.VoidQuality = true;
			if (PluginLoaded("com.Borbo.BORBO")) Compat.DisableBossDropTweak = true;
			if (PluginLoaded("com.Wolfo.YellowPercent")) Compat.DisableBossDropTweak = true;
			if (PluginLoaded("com.Moffein.ReallyBigTeleporterRadius")) Compat.ReallyBigTeleporter = true;
			if (PluginLoaded("com.Cyro.NoLockedInteractables") && FixNoLockedCfg.Value) Compat.UnlockInteractables = true;
			if (PluginLoaded("com.TPDespair.CommandDropletFix")) Compat.DisableCommandDropletFix = true;
			if (PluginLoaded("com.xoxfaby.BetterGameplay"))
			{
				Compat.DisableTeleportLostDroplet = true;
				Compat.DisableBazaarGesture = true;
			}
			if (PluginLoaded("Withor.SavageHuntress")) Compat.DisableHuntressRange = true;
			if (PluginLoaded("HIFU.HuntressAutoaimFix")) Compat.DisableHuntressAimFix = true;

			// oudated mods ???
			if (PluginLoaded("com.TeaBoneJones.IncreaseHuntressRange")) Compat.DisableHuntressRange = true;
			if (PluginLoaded("com.FluffyMods.PocketMoney")) Compat.DisableStarterMoney = true;
			if (PluginLoaded("Rein.GeneralFixes")) Compat.DisableSelfDamageFix = true;
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}



	public static class Compat
	{
		public static bool DisableSelfDamageFix = false;
		public static bool DisableBazaarGesture = false;
		public static bool VoidQuality = false;
		public static bool DisableStarterMoney = false;
		public static bool DisableBossDropTweak = false;
		public static bool ReallyBigTeleporter = false;
		public static bool UnlockInteractables = false;
		public static bool DisableCommandDropletFix = false;
		public static bool DisableTeleportLostDroplet = false;
		public static bool DisableHuntressRange = false;
		public static bool DisableHuntressAimFix = false;
	}
}
