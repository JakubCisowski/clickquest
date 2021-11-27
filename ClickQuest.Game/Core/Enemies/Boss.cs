using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Windows;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Enemies
{
	public class Boss : Enemy
	{
		public List<BossLootPattern> BossLootPatterns { get; set; }

		[JsonIgnore]
		public List<AffixFunctionality> AffixFunctionalities { get; set; }

		public List<Affix> Affixes { get; set; }

		[JsonIgnore]
		public override int CurrentHealth
		{
			get
			{
				return _currentHealth;
			}
			set
			{
				// value - new current health
				if (value == Health)
				{
					_currentHealth = value;
				}
				else if (value <= 0)
				{
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth);
					_currentHealth = 0;
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);
				}
				else
				{
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth - value);
					_currentHealth = value;
				}

				CurrentHealthProgress = CalculateCurrentHealthProgress();
			}
		}

		public override Boss CopyEnemy()
		{
			var copy = new Boss();

			copy.Id = Id;
			copy.Name = Name;
			copy.Health = Health;
			copy.CurrentHealth = Health;
			copy.Description = Description;
			copy.CurrentHealthProgress = CurrentHealthProgress;
			copy.BossLootPatterns = BossLootPatterns;
			copy.AffixFunctionalities = AffixFunctionalities;
			copy.Affixes = Affixes;

			return copy;
		}

		public override void HandleEnemyDeathIfDefeated()
		{
			if (CurrentHealth <= 0)
			{
				CombatTimerController.StopPoisonTimer();
				CombatTimerController.BossFightTimer.Stop();

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

				GrantVictoryBonuses();
				
				// Invoke Artifacts with the "on-death" effect.
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnKill();
				}
				
				InterfaceController.CurrentBossPage.HandleInterfaceAfterBossDeath();
			}
		}

		public override void GrantVictoryBonuses()
		{
			int damageDealtToBoss = Health - CurrentHealth;
			// [PRERELEASE]
			int experienceGained = 10;
			User.Instance.CurrentHero.GainExperience(experienceGained);

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - (int)Math.Ceiling(CurrentHealth / (Health / 5.0));
			// 2. Iterate through every possible loot.

			// Loot animation delay, incremented after each animation.
			int animationDelay = 1;

			foreach (var loot in BossLootPatterns)
			{
				int itemIntegerCount = (int)loot.Frequencies[threshold];

				double randomizedValue = RNG.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold] - itemIntegerCount)
				{
					itemIntegerCount++;
				}

				// Grant loot after checking if it's not empty.
				if (loot.BossLootType == RewardType.Blessing)
				{
					Blessing.AskUserAndSwapBlessing(loot.BossLootId);

					if (!GameAssets.BestiaryEntries.Any(x=>x.EntryType == BestiaryEntryType.BossLoot && x.LootType == RewardType.Blessing && x.Id==loot.BossLootId))
					{
						GameAssets.BestiaryEntries.Add(new BestiaryEntry() { Id = loot.BossLootId, LootType = RewardType.Blessing, EntryType = BestiaryEntryType.BossLoot });
					}
					
					// Start blessing animation.
					(Application.Current.MainWindow as GameWindow).CreateFloatingTextBlessing(GameAssets.Blessings.FirstOrDefault(x=>x.Id==loot.BossLootId));

					continue;
				}

				(loot.Item as Artifact)?.CreateMythicTag(Name);

				if (itemIntegerCount > 0)
				{
					loot.Item.AddItem(itemIntegerCount);

					if (!GameAssets.BestiaryEntries.Any(x=>x.EntryType == BestiaryEntryType.BossLoot && x.LootType == loot.BossLootType && x.Id==loot.BossLootId))
					{
						GameAssets.BestiaryEntries.Add(new BestiaryEntry() { Id = loot.BossLootId, LootType = loot.BossLootType, EntryType = BestiaryEntryType.BossLoot });
					}

					// Start loot animation.
					(Application.Current.MainWindow as GameWindow).CreateFloatingTextLoot(loot.Item, itemIntegerCount, animationDelay++);
				}
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();

			GameController.UpdateSpecializationAmountAndUI(SpecializationType.Dungeon);

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}
	}
}