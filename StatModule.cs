using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace TPDespair.ZetTweaks
{
	public static class StatModule
	{
		public static ConfigEntry<bool> EnableModuleCfg { get; set; }
		public static ConfigEntry<bool> UtilityChangesCfg { get; set; }
		public static ConfigEntry<float> BaseMinCritCfg { get; set; }
		public static ConfigEntry<int> ExtraJumpCfg { get; set; }
		public static ConfigEntry<float> ExtraMovespeedCfg { get; set; }
		public static ConfigEntry<bool> HealthChangesCfg { get; set; }
		public static ConfigEntry<bool> BaseHealthLimiterCfg { get; set; }
		public static ConfigEntry<float> BaseHealthRatioCfg { get; set; }
		public static ConfigEntry<float> MinBaseHealthCfg { get; set; }
		public static ConfigEntry<float> CeilBaseHealthCfg { get; set; }
		public static ConfigEntry<float> FactorLevelHealthCfg { get; set; }
		public static ConfigEntry<bool> RegenChangesCfg { get; set; }
		public static ConfigEntry<float> MultBaseRegenCfg { get; set; }
		public static ConfigEntry<float> MultLevelRegenCfg { get; set; }
		public static ConfigEntry<float> ScaleRegenMultCfg { get; set; }
		public static ConfigEntry<float> BurningRegenMultCfg { get; set; }
		public static ConfigEntry<bool> BarrierChangesCfg { get; set; }
		public static ConfigEntry<bool> DynamicBarrierCfg { get; set; }
		public static ConfigEntry<bool> BarrierSlowCfg { get; set; }
		public static ConfigEntry<float> BarrierSlowStopCfg { get; set; }
		public static ConfigEntry<float> AegisSlowMultCfg { get; set; }
		public static ConfigEntry<float> IroncladSlowMultPlayerCfg { get; set; }
		public static ConfigEntry<float> IroncladSlowMultMonsterCfg { get; set; }



		internal static void SetupConfig()
		{
			ConfigFile Config = ZetTweaksPlugin.ConfigFile;

			EnableModuleCfg = Config.Bind(
				"1a-Stat - Enable", "enableStatModule", true,
				"Enable Stat Module."
			);

			UtilityChangesCfg = Config.Bind(
				"1b-Stat - Utility", "utilityChanges", true,
				"Enable or disable utility changes."
			);
			BaseMinCritCfg = Config.Bind(
				"1b-Stat - Utility", "baseCritChance", 5f,
				"Set minimum base critical strike chance for all entities. Vanilla is 1"
			);
			ExtraJumpCfg = Config.Bind(
				"1b-Stat - Utility", "baseExtraJump", 1,
				"Extra jumps for players."
			);
			ExtraMovespeedCfg = Config.Bind(
				"1b-Stat - Utility", "baseExtraMovespeed", 0.2f,
				"Increase movement speed for all entities. 0.2 = +20%."
			);

			HealthChangesCfg = Config.Bind(
				"1c-Stat - Health", "healthChanges", true,
				"Enable or disable health changes."
			);
			BaseHealthLimiterCfg = Config.Bind(
				"1c-Stat - Health", "baseHealthLimiter", true,
				"Prevent base health from being reduced."
			);
			BaseHealthRatioCfg = Config.Bind(
				"1c-Stat - Health", "baseHealthRatio", 0.35f,
				"Controls how much base health and level health influence new base health. 0 : baseHealth = baseHealth. 1 : baseHealth = levelHealth * 3.33333."
			);
			MinBaseHealthCfg = Config.Bind(
				"1c-Stat - Health", "baseHealthMinimum", 120f,
				"Minimum player base health."
			);
			CeilBaseHealthCfg = Config.Bind(
				"1c-Stat - Health", "baseHealthCeiling", 15f,
				"Round player base health up to nearest multiple."
			);
			FactorLevelHealthCfg = Config.Bind(
				"1c-Stat - Health", "levelHealthFactor", 0.333333f,
				"Player level health derived from base health. Vanilla is 0.3"
			);

			RegenChangesCfg = Config.Bind(
				"1d-Stat - Regeneration", "regenChanges", true,
				"Enable or disable regen changes."
			);
			MultBaseRegenCfg = Config.Bind(
				"1d-Stat - Regeneration", "baseRegenMult", 2f,
				"Player base regen multiplier."
			);
			MultLevelRegenCfg = Config.Bind(
				"1d-Stat - Regeneration", "levelRegenMult", 2.5f,
				"Player level regen multiplier."
			);
			ScaleRegenMultCfg = Config.Bind(
				"1d-Stat - Regeneration", "scaleRegenFromLevel", 0.1f,
				"Set player regen increase from levels. Vanilla is 0.2 = +100% every 5 levels."
			);
			BurningRegenMultCfg = Config.Bind(
				"1d-Stat - Regeneration", "burningRegenMult", 0.5f,
				"Burning regen multiplier. Vanilla is 0"
			);

			BarrierChangesCfg = Config.Bind(
				"1e-Stat - Barrier", "barrierChanges", true,
				"Enable or disable barrier changes."
			);
			DynamicBarrierCfg = Config.Bind(
				"1e-Stat - Barrier", "dynamicBarrierDecay", true,
				"Barrier decays based off current barrier. 2x when full. 0.5x at 25%."
			);
			BarrierSlowCfg = Config.Bind(
				"1e-Stat - Barrier", "barrierSlow", true,
				"Allows items and buffs to slow barrier decay."
			);
			BarrierSlowStopCfg = Config.Bind(
				"1e-Stat - Barrier", "barrierSlowedStop", 0.1f,
				"Barrier stops decaying at barrier fraction if decay is slowed."
			);
			AegisSlowMultCfg = Config.Bind(
				"1e-Stat - Barrier", "aegisBarrierSlow", 0.3f,
				"Slow barrier decay when entity has Aegis."
			);
			IroncladSlowMultPlayerCfg = Config.Bind(
				"1e-Stat - Barrier", "ironcladBarrierSlowPlayer", 0.3f,
				"Slow barrier decay when player has Ironclad Affix."
			);
			IroncladSlowMultMonsterCfg = Config.Bind(
				"1e-Stat - Barrier", "ironcladBarrierSlowMonster", 0.65f,
				"Slow barrier decay when monster has Ironclad Affix."
			);
		}

		internal static void Init()
		{
			if (EnableModuleCfg.Value)
			{
				if (UtilityChangesCfg.Value)
				{
					BaseCritHook();
					ExtraJumpHook();
					ExtraMovespeedHook();
				}

				if (HealthChangesCfg.Value)
				{
					BaseHealthHook();
				}

				if (RegenChangesCfg.Value)
				{
					BaseRegenHook();
					RegenScalingHook();
					BurningRegenHook();
				}
			}
		}

		internal static void LateInit()
		{
			if (EnableModuleCfg.Value)
			{
				if (BarrierChangesCfg.Value && !Compat.DisableBarrierChanges)
				{
					if (DynamicBarrierCfg.Value && !Compat.DisableDynamicBarrier) DynamicBarrierDecayHook();
					if (BarrierSlowCfg.Value) SlowBarrierDecayHook();
				}
			}
		}



		private static void BaseCritHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				self.baseCrit = Mathf.Max(self.baseCrit, BaseMinCritCfg.Value);

				orig(self);
			};
		}

		private static void ExtraJumpHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("baseJumpCount"),
					x => x.MatchLdloc(8)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 8);
					c.EmitDelegate<Func<CharacterBody, int, int>>((self, value) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player) value += ExtraJumpCfg.Value;

						return value;
					});
					c.Emit(OpCodes.Stloc, 8);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - ExtraJumpHook Failed!");
				}
			};
		}

		private static void ExtraMovespeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(62),
					x => x.MatchLdloc(63),
					x => x.MatchLdloc(64),
					x => x.MatchDiv(),
					x => x.MatchMul(),
					x => x.MatchStloc(62)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);

					c.Emit(OpCodes.Ldloc, 63);
					c.EmitDelegate<Func<float, float>>((value) =>
					{
						value += ExtraMovespeedCfg.Value;

						return value;
					});
					c.Emit(OpCodes.Stloc, 63);

					c.Emit(OpCodes.Ldloc, 62);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - ExtraMovespeedHook Failed!");
				}
			};
		}

		private static float StepCeil(float value, float step)
		{
			return Mathf.Ceil((value - 0.1f) / step) * step;
		}

		private static void BaseHealthHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(50),
					x => x.MatchLdloc(51),
					x => x.MatchMul(),
					x => x.MatchStloc(50)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 50);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (!self.isPlayerControlled && self.teamComponent.teamIndex != TeamIndex.Player) return value;

						float addedHealth = 0f;
						float targetBaseHealth;
						float targetLevelHealth;

						if (BaseHealthRatioCfg.Value <= 0f)
						{
							targetBaseHealth = self.baseMaxHealth;
						}
						else if (BaseHealthRatioCfg.Value >= 1f)
						{
							targetBaseHealth = self.levelMaxHealth * 3.33333f;
						}
						else
						{
							targetBaseHealth = Mathf.Lerp(self.baseMaxHealth, self.levelMaxHealth * 3.33333f, BaseHealthRatioCfg.Value);
						}

						if (BaseHealthLimiterCfg.Value) targetBaseHealth = Mathf.Max(self.baseMaxHealth, targetBaseHealth);
						targetBaseHealth = Mathf.Max(MinBaseHealthCfg.Value, targetBaseHealth);
						targetBaseHealth = StepCeil(targetBaseHealth, CeilBaseHealthCfg.Value);
						addedHealth += targetBaseHealth - self.baseMaxHealth;

						targetLevelHealth = targetBaseHealth * FactorLevelHealthCfg.Value;
						addedHealth += (targetLevelHealth - self.levelMaxHealth) * (self.level - 1f);

						return value + Mathf.Round(addedHealth);
					});
					c.Emit(OpCodes.Stloc, 50);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - BaseHealthHook Failed!");
				}
			};
		}

		private static void BaseRegenHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(1f),
					x => x.MatchStloc(60)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 55);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (!self.isPlayerControlled && self.teamComponent.teamIndex != TeamIndex.Player) return value;

						float addedRegen = 0f;

						if (self.baseRegen > 0f) addedRegen += self.baseRegen * (MultBaseRegenCfg.Value - 1f);
						if (self.levelRegen > 0f) addedRegen += self.levelRegen * (MultLevelRegenCfg.Value - 1f) * (self.level - 1f);

						return value + addedRegen;
					});
					c.Emit(OpCodes.Stloc, 55);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - BaseRegenHook Failed!");
				}
			};
		}

		private static void RegenScalingHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(1f),
					x => x.MatchLdloc(41),
					x => x.MatchLdcR4(0.2f),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(54)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<CharacterBody, float>>((self) =>
					{
						if (!self.isPlayerControlled && self.teamComponent.teamIndex != TeamIndex.Player) return 0.2f;
						return ScaleRegenMultCfg.Value;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - RegenScalingHook Failed!");
				}
			};
		}

		private static void BurningRegenHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("OnFire")),
					x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff"),
					x => x.MatchBrfalse(out _),
					x => x.MatchLdcR4(0f),
					x => x.MatchStloc(60)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 60);
					c.Emit(OpCodes.Ldc_R4, BurningRegenMultCfg.Value);
					c.Emit(OpCodes.Mul);
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - BurningRegenHook Failed!");
				}
			};
		}

		public static float GetBurnRegenMult()
		{
			if (!RegenChangesCfg.Value) return 0f;
			return BurningRegenMultCfg.Value;
		}

		private static void DynamicBarrierDecayHook()
		{
			On.RoR2.CharacterBody.FixedUpdate += (orig, self) =>
			{
				orig(self);

				float min = self.maxBarrier / 60f;
				self.barrierDecayRate = Mathf.Max(min, self.healthComponent.barrier / 15f);
			};
		}

		private static void SlowBarrierDecayHook()
		{
			IL.RoR2.HealthComponent.ServerFixedUpdate += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchCallvirt<CharacterBody>("get_barrierDecayRate")
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((rate, self) =>
					{
						if (self.body) rate *= GetBodyBarrierDecayMult(self.body);

						return rate;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - SlowBarrierDecayHook:DecayMult Failed!");
				}

				found = c.TryGotoNext(
					x => x.MatchCall<HealthComponent>("set_Networkbarrier")
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((decayed, self) =>
					{
						if (self.body && GetBodyBarrierDecayMult(self.body) <= 0.99f)
						{
							float limit = self.fullBarrier * BarrierSlowStopCfg.Value;

							if (self.barrier > limit)
							{
								return Mathf.Max(limit, decayed);
							}
							else
							{
								return self.barrier;
							}
						}

						return decayed;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [StatModule] - SlowBarrierDecayHook:DecayStop Failed!");
				}
			};
		}

		public static float GetBodyBarrierDecayMult(CharacterBody body)
		{
			float mult = 1f;

			Inventory inventory = body.inventory;
			if (inventory)
			{
				if (inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal) > 0) mult *= 1f - AegisSlowMultCfg.Value;
			}

			if (body.HasBuff(ZetTweaksPlugin.AffixArmored))
			{
				if (body.teamComponent.teamIndex == TeamIndex.Player)
				{
					mult *= 1f - IroncladSlowMultPlayerCfg.Value;
				}
				else
				{
					mult *= 1f - IroncladSlowMultMonsterCfg.Value;
				}
			}

			return mult;
		}
	}
}
