using BepInEx;
using BepInEx.Bootstrap;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetTweaks
{
	internal static class TeleShowFix
    {
		private static BaseUnityPlugin TeleShowPlugin;
		private static FieldInfo ShouldShowField;
		private static bool TargetValue = false;
		private static bool CurrentValue = false;

		private static int State = -1;
		public static bool Enabled
		{
			get
			{
				if (State == -1)
				{
					if (Chainloader.PluginInfos.ContainsKey("com.Pickleses.TeleShow")) State = 1;
					else State = 0;
				}
				return State == 1;
			}
		}

		internal static void Init()
		{
			On.RoR2.TeleporterInteraction.Awake += delegate (On.RoR2.TeleporterInteraction.orig_Awake orig, TeleporterInteraction self)
			{
				orig(self);
				TargetValue = true;
				CurrentValue = true;
			};
			On.RoR2.TeleporterInteraction.OnInteractionBegin += delegate (On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
			{
				TargetValue = false;
				CurrentValue = false;
				orig(self, activator);
			};

			TeleShowPlugin = Chainloader.PluginInfos["com.Pickleses.TeleShow"].Instance;
			ShouldShowField = TeleShowPlugin.GetType().GetField("shouldShow", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal static void SetShouldShow()
		{
			LocalUser firstLocalUser = LocalUserManager.GetFirstLocalUser();

			if (firstLocalUser == null || !firstLocalUser.cachedBody)
			{
				if (CurrentValue != false)
				{
					CurrentValue = false;
					if (ShouldShowField != null)
					{
						Debug.LogWarning("TeleShowFix shouldShow SetValue : " + CurrentValue);
						ShouldShowField.SetValue(TeleShowPlugin, CurrentValue);
					}
				}
			}
			else
			{
				if (CurrentValue != TargetValue)
				{
					CurrentValue = TargetValue;
					if (ShouldShowField != null)
					{
						Debug.LogWarning("TeleShowFix shouldShow SetValue : " + CurrentValue);
						ShouldShowField.SetValue(TeleShowPlugin, CurrentValue);
					}
				}
			}
		}
	}
}
