using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Navigation;
using System;
using UnityEngine;

namespace TPDespair.ZetTweaks
{
	public static class GameplayModule
	{
		public static ConfigEntry<bool> EnableModuleCfg { get; set; }
		public static ConfigEntry<bool> FixSelfDamageCfg { get; set; }
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
		public static ConfigEntry<float> MassRadiusCfg { get; set; }
		public static ConfigEntry<float> MassTimeCfg { get; set; }
		public static ConfigEntry<float> ShipRadiusCfg { get; set; }
		public static ConfigEntry<float> ShipTimeCfg { get; set; }
		public static ConfigEntry<int> MoonHoldoutZonesCfg { get; set; }
		public static ConfigEntry<bool> CommandDropletFixCfg { get; set; }
		public static ConfigEntry<bool> TeleportLostDropletCfg { get; set; }
		public static ConfigEntry<bool> ModifyHuntressRangeCfg { get; set; }
		public static ConfigEntry<float> BaseTargetingRangeCfg { get; set; }
		public static ConfigEntry<float> LevelTargetingRangeCfg { get; set; }
		public static ConfigEntry<bool> BazaarGestureCfg { get; set; }



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

			ModifyHuntressRangeCfg = Config.Bind(
				"2g-Gameplay - Skill", "modifyHuntressRange", true,
				"Enable or disable changing huntress targeting range."
			);
			BaseTargetingRangeCfg = Config.Bind(
				"2g-Gameplay - Skill", "baseHuntressTargetingRange", 60f,
				"Huntress base targeting range. Vanilla is 60"
			);
			LevelTargetingRangeCfg = Config.Bind(
				"2g-Gameplay - Skill", "levelHuntressTargetingRange", 2f,
				"Huntress level targeting range. Vanilla is 0"
			);

			BazaarGestureCfg = Config.Bind(
				"2h-Gameplay - Equipment", "disableBazaarGesture", true,
				"Prevent Gesture from firing equipment in Bazaar."
			);
		}

		internal static void Init()
		{
			if (EnableModuleCfg.Value)
			{
				if (DirectorStageLimitCfg.Value > 0) DirectorMoneyHook();
				if (MultitudeMoneyCfg.Value) GoldFromKillHook();

				MoonBatteryMissionController.onInstanceChangedGlobal += ChangeRequiredBatteries;
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

				if (MoneyStageLimitCfg.Value > 0 && !Compat.DisableStarterMoney) SceneDirector.onPostPopulateSceneServer += GiveMoney;

				if (BossDropTweakCfg.Value && !Compat.DisableBossDropTweak) BossDropHook();
				if (UnlockInteractablesCfg.Value || Compat.UnlockInteractables) UnlockInteractablesHook();

				if (ModifyHoldoutValueCfg.Value || EclipseHoldoutLimitCfg.Value) HoldoutZoneHook();

				if (CommandDropletFixCfg.Value && !Compat.DisableCommandDropletFix) CommandDropletFix();
				if (TeleportLostDropletCfg.Value && !Compat.DisableTeleportLostDroplet)
				{
					PickupBackgroundCollision();
					PickupTeleportHook();
				}

				if (ModifyHuntressRangeCfg.Value && !Compat.DisableHuntressRange) HuntressRangeBuff();

				if (BazaarGestureCfg.Value && !Compat.DisableBazaarGesture) BazaarGestureHook();
			}
		}



		private static void SelfCrowbarHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				ILLabel jumpTo = null;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(17),
					x => x.MatchLdcI4(0),
					x => x.MatchBle(out jumpTo)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<HealthComponent, CharacterBody, bool>>((healthComponent, attacker) =>
					{
						if (healthComponent.body == attacker) return true;

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
					x => x.MatchLdloc(18),
					x => x.MatchLdcI4(0),
					x => x.MatchBle(out jumpTo)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<HealthComponent, CharacterBody, bool>>((healthComponent, attacker) =>
					{
						if (healthComponent.body == attacker) return true;

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

		private static void GiveMoney(SceneDirector director)
		{
			if (Run.instance)
			{
				if (Run.instance.stageClearCount < MoneyStageLimitCfg.Value)
				{
					int scaledCost = Run.instance.GetDifficultyScaledCost(25);
					uint money = 1u + (uint)Mathf.Round(scaledCost * MoneyChestGivenCfg.Value);

					foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
					{
						pcmc.master.GiveMoney(money);
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
					c.Index += 2;

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
					c.Emit(OpCodes.Ldloc, 3);
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
			MoonBatteryMissionController.instance._numRequiredBatteries = Mathf.Clamp(MoonHoldoutZonesCfg.Value, 1, 4);
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

			if (sceneName == "arena" && !Compat.ReallyBigTeleporter)
			{
				if (self.baseRadius == 15f && self.baseChargeDuration == 60f)
				{
					self.baseRadius = VoidRadiusCfg.Value;
					self.baseChargeDuration = VoidTimeCfg.Value;
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
					if (collider.GetComponent<PickupDropletController>() || collider.GetComponent<GenericPickupController>())
					{
						SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
						spawnCard.hullSize = HullClassification.Human;
						spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
						spawnCard.prefab = Resources.Load<GameObject>("SpawnCards/HelperPrefab");

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

		private static void HuntressRangeBuff()
		{
			IL.RoR2.HuntressTracker.SearchForTarget += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<HuntressTracker>("maxTrackingDistance")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<HuntressTracker, float>>((tracker) =>
					{
						float level = tracker.GetComponent<CharacterBody>().level;
						return BaseTargetingRangeCfg.Value + (level - 1f) * LevelTargetingRangeCfg.Value;
					});
				}
				else
				{
					Debug.LogWarning("ZetTweaks [GameplayModule] - HuntressRangeBuff Failed!");
				}
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
	}
}
