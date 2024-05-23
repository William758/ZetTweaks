using BepInEx;
using BepInEx.Bootstrap;
using System;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetTweaks
{
    internal static class ShareSuiteCompat
    {
		private static readonly string GUID = "com.funkfrog_sipondo.sharesuite";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type MoneySharingType;
		private static MethodInfo ExternalMoneyMethod;

		internal static bool moneyMethodFound = false;



		private static int State = -1;
		public static bool Enabled
		{
			get
			{
				if (State == -1)
				{
					if (Chainloader.PluginInfos.ContainsKey(GUID)) State = 1;
					else State = 0;
				}
				return State == 1;
			}
		}



		internal static void Init()
		{
			if (!Chainloader.PluginInfos.ContainsKey(GUID)) return;

			Plugin = Chainloader.PluginInfos[GUID].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				GatherInfos();
			}
			else
			{
				Debug.LogWarning("ZetTweaks [ShareSuiteCompat] - Could Not Find ShareSuite Assembly");
			}

			if (ExternalMoneyMethod != null) moneyMethodFound = true;
		}



		private static void GatherInfos()
		{
			
			Type type;

			type = Type.GetType("ShareSuite.MoneySharingHooks, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				MoneySharingType = type;

				ExternalMoneyMethod = type.GetMethod("AddMoneyExternal", Flags);
				if (ExternalMoneyMethod == null) Debug.LogWarning("ZetTweaks [ShareSuiteCompat] - Could Not Find Method : MoneySharingHooks.AddMoneyExternal");
			}
			else
			{
				Debug.LogWarning("ZetTweaks [ShareSuiteCompat] - Could Not Find Type : ShareSuite.MoneySharingHooks");
			}
		}



		internal static void AddMoneyShareSuite(int money)
		{
			if (ExternalMoneyMethod != null)
			{
				ExternalMoneyMethod.Invoke(MoneySharingType, new object[] { money });
				Debug.LogWarning("ZetTweaks [ShareSuiteCompat] - AddMoneyShareSuite : " + money);
			}
		}
	}
}
