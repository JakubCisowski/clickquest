using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using ClickQuest.Player;

namespace ClickQuest.Controls
{
	public partial class MonsterButton : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly Random _rng = new Random();
		private readonly RegionPage _regionPage;
		private DispatcherTimer _poisonTimer;
		private DispatcherTimer _auraTimer;
		private int _poisonTicks;

		public Monster Monster { get; set; }

		public Region Region
		{
			get
			{
				return _regionPage.Region;
			}
		}

		public int AuraTickDamage
		{
			get
			{
				return (int) Math.Ceiling(User.Instance.CurrentHero.AuraDamage * Monster.Health);
			}
		}

		public double AuraTickInterval
		{
			get
			{
				return 1d / User.Instance.CurrentHero.AuraAttackSpeed;
			}
		}

		public MonsterButton(RegionPage regionPage)
		{
			InitializeComponent();

			_regionPage = regionPage;

			SetupPoisonTimer();
			SetupAuraTimer();
			SpawnMonster();
		}

		public void SpawnMonster()
		{
			var frequencyList = Region.Monsters.Select(x => x.Frequency).ToList();
			int position = RandomizeFreqenciesListPosition(frequencyList);
			Monster = Region.Monsters[position].Monster.CopyEnemy();

			DataContext = Monster;

			CombatController.StartAuraTimerOnCurrentRegion();
		}

		private void HandleMonsterDeathIfDefeated()
		{
			if (Monster.CurrentHealth <= 0)
			{
				StopPoisonTimer();
				GrantVictoryBonuses();
				SpawnMonster();
			}
		}

		public void GrantVictoryBonuses()
		{
			int experienceGained = Experience.CalculateMonsterXpReward(Monster.Health);
			User.Instance.CurrentHero.Experience += experienceGained;

			var frequencyList = Monster.Loot.Select(x => x.Frequency).ToList();
			int position = RandomizeFreqenciesListPosition(frequencyList);
			var selectedLoot = Monster.Loot[position].Item;

			if (selectedLoot.Id != 0)
			{
				selectedLoot.AddItem();
			}

			// [PRERELEASE] Display exp and loot for testing purposes.
			_regionPage.TestRewardsBlock.Text = "Loot: " + selectedLoot.Name + ", Exp: " + experienceGained;

			CheckForDungeonKeyDrop();

			_regionPage.StatsFrame.Refresh();
			UpdateAuraAttackSpeed();
		}

		private int RandomizeFreqenciesListPosition(List<double> frequencies)
		{
			double randomizedValue = _rng.Next(1, 10001) / 10000d;
			int i = 0;

			while (randomizedValue > frequencies[i])
			{
				randomizedValue -= frequencies[i];
				i++;
			}

			return i;
		}

		private void CheckForDungeonKeyDrop()
		{
			var DungeonKeyRarityChances = DungeonKey.CreateRarityChancesList(Monster.Health);

			int position = RandomizeFreqenciesListPosition(DungeonKeyRarityChances);

			// Grant dungeon key after if algorithm didn't roll empty loot.
			if (position != 0)
			{
				var dungeonKey = User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity) (position - 1));
				dungeonKey.AddItem();

				// [PRERELEASE] Display dungeon key drop.
				_regionPage.TestRewardsBlock.Text += $". You've got a {(Rarity) (position - 1)} Dungeon Key!";
			}
		}

		private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

			if (isNoQuestActive)
			{
				StartPoisonTimer();

				var damageBaseAndCritInfo = User.Instance.CurrentHero.CalculateClickDamage();
				var damageOnHit = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];
				var damageTotal = damageBaseAndCritInfo.Damage + damageOnHit;
				Monster.CurrentHealth -= damageTotal;

				var damageType = damageBaseAndCritInfo.IsCritical ? DamageType.Critical : DamageType.Normal;
				CreateFloatingTextPathAndStartAnimations(damageBaseAndCritInfo.Damage, damageType);
				CreateFloatingTextPathAndStartAnimations(damageOnHit, DamageType.OnHit);

				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;

				HandleMonsterDeathIfDefeated();

				_regionPage.StatsFrame.Refresh();
			}
			else
			{
				AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}

		private void SetupAuraTimer()
		{
			_auraTimer = new DispatcherTimer();
			_auraTimer.Tick += AuraTimer_Tick;
		}

		private void SetupPoisonTimer()
		{
			int poisonIntervalMs = 500;
			_poisonTimer = new DispatcherTimer();
			_poisonTimer.Interval = new TimeSpan(0, 0, 0, 0, poisonIntervalMs);
			_poisonTimer.Tick += PoisonTimer_Tick;
			_poisonTicks = 0;
		}

		public void StartAuraTimer()
		{
			if (User.Instance.CurrentHero != null)
			{
				// ex.: 1.50 aura attack speed = 1.5 aura ticks per second
				_auraTimer.Interval = TimeSpan.FromSeconds(1d / User.Instance.CurrentHero.AuraAttackSpeed);

				_auraTimer.Start();
			}
		}

		private void StartPoisonTimer()
		{
			if (User.Instance.CurrentHero.PoisonDamage > 0)
			{
				_poisonTicks = 0;
				_poisonTimer.Start();
			}
		}

		public void StopCombatTimers()
		{
			StopPoisonTimer();
			_auraTimer.Stop();
		}

		public void StopPoisonTimer()
		{
			_poisonTimer.Stop();
			_poisonTicks = 0;
		}

		private void PoisonTimer_Tick(object source, EventArgs e)
		{
			int poisonTicksMax = 5;

			if (_poisonTicks >= poisonTicksMax)
			{
				_poisonTimer.Stop();
			}
			else
			{
				int poisonDamage = User.Instance.CurrentHero.PoisonDamage;
				Monster.CurrentHealth -= poisonDamage;

				CreateFloatingTextPathAndStartAnimations(poisonDamage, DamageType.Poison);

				_poisonTicks++;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				HandleMonsterDeathIfDefeated();
			}
		}

		private void AuraTimer_Tick(object source, EventArgs e)
		{
			if (User.Instance.CurrentHero != null)
			{
				Monster.CurrentHealth -= AuraTickDamage;

				CreateFloatingTextPathAndStartAnimations(AuraTickDamage, DamageType.Aura);

				HandleMonsterDeathIfDefeated();
			}
		}

		private void CreateFloatingTextPathAndStartAnimations(int damage, DamageType damageType)
		{
			if (damage == 0)
			{
				return;
			}

			int animationDuration = 1;
			int maximumPositionOffset = 50;
			var mousePosition = Mouse.GetPosition(DamageTextCanvas);
			
			var panel = FloatingTextController.CreateFloatingTextPanel(damage, damageType);
			
			var randomizedPositions = FloatingTextController.RandomizeFloatingTextPathPosition(mousePosition, DamageTextCanvas.ActualWidth, DamageTextCanvas.ActualHeight, maximumPositionOffset);

			Canvas.SetLeft(panel, randomizedPositions.X);
			Canvas.SetTop(panel, randomizedPositions.Y);

			DamageTextCanvas.Children.Add(panel);

			var textOpacityAnimation = FloatingTextController.CreateTextOpacityAnimation(animationDuration);
			textOpacityAnimation.Completed += FloatingTextAnimation_Completed;
			panel.BeginAnimation(OpacityProperty, textOpacityAnimation);

			var transform = new ScaleTransform(1, 1);
			panel.LayoutTransform = transform;
			var animationX = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
			transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
			var animationY = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
			transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
		}

		private void FloatingTextAnimation_Completed(object sender, EventArgs e)
		{
			// Remove invisible paths.
			DamageTextCanvas.Children.Remove(DamageTextCanvas.Children.OfType<Border>().FirstOrDefault(x => x.Opacity == 0));
		}

		private void UpdateAuraAttackSpeed()
		{
			_auraTimer.Stop();
			_auraTimer.Interval = TimeSpan.FromSeconds(AuraTickInterval);
			_auraTimer.Start();
		}
	}
}