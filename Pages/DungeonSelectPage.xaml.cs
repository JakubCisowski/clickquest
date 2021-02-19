using ClickQuest.Account;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Items;
using ClickQuest.Places;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class DungeonSelectPage : Page
	{
		#region Private Fields
		private DungeonGroup _dungeonGroupSelected;
		private Dungeon _dungeonSelected;
		private Monster _bossSelected;

		#endregion

		public DungeonSelectPage()
		{
			InitializeComponent();

			// Initially, display dungeon groups.
			LoadDungeonGroupSelection();
		}

		public void LoadDungeonGroupSelection()
		{
			UndoButton.Visibility = Visibility.Hidden;

			DungeonSelectPanel.Children.Clear();

			UndoButton.Click -= UndoButtonGroup_Click;
			UndoButton.Click -= UndoButtonDungeon_Click;

			// Create buttons for selecting dungeon groups.
			for (int i = 0; i < Database.DungeonGroups.Count; i++)
			{
				var button = new Button()
				{
					Name = "DungeonGroup" + Database.DungeonGroups[i].Id.ToString(),
					Width = 250,
					Height = 80,
				};

				var block = new TextBlock()
				{
					FontSize = 22,
					Text = Database.DungeonGroups[i].Name + "\n" + Database.DungeonGroups[i].Description
				};

				button.Content = block;

				button.Click += DungeonGroupButton_Click;

				DungeonSelectPanel.Children.Insert(i, button);
			}
		}

		public void LoadDungeonSelection()
		{
			DungeonSelectPanel.Children.Clear();

			UndoButton.Visibility = Visibility.Visible;

			UndoButton.Click += UndoButtonGroup_Click;

			var dungeonsOfThisGroup = Database.Dungeons.Where(x => x.DungeonGroup == _dungeonGroupSelected).ToList();

			// Create buttons for selecting dungeon groups.
			for (int i = 0; i < dungeonsOfThisGroup.Count; i++)
			{
				var button = new Button()
				{
					Name = "Dungeon" + dungeonsOfThisGroup[i].Id.ToString(),
					Width = 250,
					Height = 80
				};

				var block = new TextBlock()
				{
					FontSize = 22,
					Text = dungeonsOfThisGroup[i].Name + "\n" + dungeonsOfThisGroup[i].Description
				};

				button.Content = block;

				button.Click += DungeonButton_Click;

				DungeonSelectPanel.Children.Insert(i, button);
			}
		}

		public void LoadBossSelection()
		{
			DungeonSelectPanel.Children.Clear();

			UndoButton.Click -= UndoButtonGroup_Click;
			UndoButton.Click += UndoButtonDungeon_Click;

			// Create buttons for selecting dungeon groups.
			for (int i = 0; i < _dungeonSelected.Bosses.Count; i++)
			{
				var button = new Button()
				{
					Name = "Boss" + _dungeonSelected.Bosses[i].Id.ToString(),
					Width = 250,
					Height = 80
				};

				var block = new TextBlock()
				{
					FontSize = 22,
					Text = _dungeonSelected.Bosses[i].Name + "\n" + _dungeonSelected.Bosses[i].Description
				};

				button.Content = block;

				button.Click += BossButton_Click;

				DungeonSelectPanel.Children.Insert(i, button);
			}
		}

		#region Events
		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Come back to town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(Database.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(Database.Pages["Town"] as TownPage).StatsFrame.Refresh();

			// Reset selection.
			LoadDungeonGroupSelection();
		}
		private void DungeonGroupButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if any quest is currently assigned to this hero (if so, hero can't enter the dungeon).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
			{
				// Select dungeon group.
				_dungeonGroupSelected = Database.DungeonGroups.FirstOrDefault(x => x.Id == int.Parse((sender as Button).Name.Substring(12)));

				// Now let user select dungeon in that group.
				LoadDungeonSelection();

				// Change info bar
				(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon";
			}
		}

		private void DungeonButton_Click(object sender, RoutedEventArgs e)
		{
			// Select dungeon.
			_dungeonSelected = Database.Dungeons.FirstOrDefault(x => x.Id == int.Parse((sender as Button).Name.Substring(7)));

			// Now let user select boss in that dungeon.
			LoadBossSelection();

			// Change info bar
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting boss";
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			// Select boss.
			_bossSelected = _dungeonSelected.Bosses.FirstOrDefault(x => x.Id == int.Parse((sender as Button).Name.Substring(4)));

			// Check if user has enough dungoen keys to enter boss fight.
			var counts = _dungeonGroupSelected.KeyRequirements.GroupBy(x => x).ToDictionary(k => k.Key, v => v.Count());
			foreach (var pair in counts)
			{
				if (User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)pair.Key).Quantity < pair.Value)
				{
					// Display error - not enough dungeon keys.
					AlertBox.Show($"Not enough {(Rarity)pair.Key} dungeon keys to enter.\nTry to get them by completing quests and killing monsters!", MessageBoxButton.OK);
					return;
				}
			}
			// Remove dungeon keys from account.
			foreach (var pair in counts)
			{
				User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)pair.Key).Quantity -= pair.Value;
			}

			// Start boss fight.
			(Database.Pages["DungeonBoss"] as DungeonBossPage).TownButton.Visibility = Visibility.Hidden;
			(Database.Pages["DungeonBoss"] as DungeonBossPage).StartBossFight(new Monster(_bossSelected));
			// Navigate to boss fight page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["DungeonBoss"]);
			(Database.Pages["DungeonBoss"] as DungeonBossPage).EquipmentFrame.Refresh();
			// Change info bar
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Boss fight";

			// Reset selections (for future use).
			_bossSelected = null;
			_dungeonGroupSelected = null;
			_dungeonSelected = null;
			// Hide undo button.
			UndoButton.Visibility = Visibility.Hidden;
			// Reset selection page.
			LoadDungeonGroupSelection();
		}

		private void UndoButtonGroup_Click(object sender, RoutedEventArgs e)
		{
			_dungeonGroupSelected = null;
			LoadDungeonGroupSelection();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon group";
		}

		private void UndoButtonDungeon_Click(object sender, RoutedEventArgs e)
		{
			_dungeonSelected = null;
			LoadDungeonSelection();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon";
		}

		#endregion Events
	}
}