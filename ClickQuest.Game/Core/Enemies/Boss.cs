using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using System;
using System.Collections.Generic;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Enemies
{
	public class Boss : Enemy
	{
		public List<BossLootPattern> BossLootPatterns { get; set; }

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
			int threshold = 5 - (int)Math.Ceiling((double)CurrentHealth / (Health / 5));
			// 2. Iterate through every possible loot.
			string lootText = "Experience gained: " + experienceGained + " \n" + "Loot: \n";

			foreach (var loot in BossLootPatterns)
			{
				int itemIntegerCount = (int)loot.Frequencies[threshold];

				double randomizedValue = RNG.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold] - itemIntegerCount)
				{
					// Grant loot after checking if it's not empty.
					if (loot.BossLootType == RewardType.Blessing)
					{
						bool hasBlessingActive = User.Instance.CurrentHero.Blessing != null;

						if (hasBlessingActive)
						{
							bool doesUserWantToSwap = Blessing.AskUserAndSwapBlessing(loot.BossLootId);

							if (doesUserWantToSwap == false)
							{
							}
						}
						else
						{
							Blessing.AddOrReplaceBlessing(loot.BossLootId);
						}

						continue;
					}

					itemIntegerCount++;
				}

				(loot.Item as Artifact)?.CreateMythicTag(Name);

				loot.Item.AddItem(itemIntegerCount);
				lootText += "- " + $"{itemIntegerCount}x " + loot.Item.Name + " (" + loot.BossLootType + ")\n";
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();

			// Grant gold reward.
			int goldReward = 2137; // (change value later)
			User.Instance.Gold += goldReward;
			lootText += "- " + goldReward + " (gold)\n";

			// [PRERELEASE] Display exp and loot for testing purposes.
			InterfaceController.CurrentBossPage.TestRewardsBlock.Text = lootText;

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Dungeon]++;

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}
	}
}