﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class DungeonsPanel : UserControl
	{
		private Dungeon _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;
		private List<int> _bossIds;

		public DungeonsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Dungeons.Select(x => x.Name);
		}

		public void RefreshStaticValuesPanel()
		{
			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			StackPanel panel = new StackPanel
			{
				Name = "StaticInfoPanel"
			};

			Dungeon selectedDungeon = _dataContext;

			TextBox idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedDungeon.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			ComboBox dungeonGroupBox = new ComboBox
			{
				Name = "DungeonGroupBox",
				ItemsSource = GameContent.DungeonGroups.Select(x => x.Name),
				SelectedValue = GameContent.DungeonGroups.FirstOrDefault(x => x.Id == selectedDungeon.DungeonGroupId)?.Name,
				Margin = new Thickness(10)
			};
			TextBox nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedDungeon.Name,
				Margin = new Thickness(10)
			};
			TextBox backgroundBox = new TextBox
			{
				Name = "BackgroundBox",
				Text = selectedDungeon.Background,
				Margin = new Thickness(10)
			};
			TextBox descriptionBox = new TextBox
			{
				Name = "DescriptionBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedDungeon.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(dungeonGroupBox, "DungeonGroup");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(backgroundBox, "Background");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(dungeonGroupBox.Name, dungeonGroupBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(backgroundBox.Name, backgroundBox);
			_controls.Add(descriptionBox.Name, descriptionBox);

			foreach (var elem in _controls)
			{
				// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
				if (elem.Value is TextBox textBox)
				{
					textBox.Style = (Style) FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
					textBox.GotFocus += TextBox_GotFocus;
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style) FindResource("MaterialDesignOutlinedComboBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
				}

				panel.Children.Add(elem.Value);
			}

			Grid.SetColumn(panel, 1);

			_currentPanel = panel;

			MainGrid.Children.Add(panel);
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		public void Save()
		{
			Dungeon dungeon = _dataContext;

			if (dungeon is null)
			{
				return;
			}

			dungeon.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			dungeon.DungeonGroupId = GameContent.DungeonGroups.FirstOrDefault(x => x.Name == (_controls["DungeonGroupBox"] as ComboBox).SelectedValue.ToString()).Id;
			dungeon.Name = (_controls["NameBox"] as TextBox).Text;
			dungeon.Background = (_controls["BackgroundBox"] as TextBox).Text;
			dungeon.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Dungeons.Select(x => x.Id).Contains(dungeon.Id))
			{
				int indexOfOldDungeon = GameContent.Dungeons.FindIndex(x => x.Id == dungeon.Id);
				GameContent.Dungeons[indexOfOldDungeon] = dungeon;
			}
			else
			{
				// If not, add it.
				GameContent.Dungeons.Add(dungeon);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = (GameContent.Dungeons.Max(x => x.Id as int?) ?? 0) + 1;

			_dataContext = new Dungeon
			{
				Id = nextId,
				BossIds = new List<int>()
			};
			_bossIds = _dataContext.BossIds;

			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();
			DynamicValuesPanel.Children.Clear();

			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			Dungeon? objectToDelete = GameContent.Dungeons.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.Dungeons.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;

			_currentPanel.Children.Clear();
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Hidden;
			DeleteObjectButton.Visibility = Visibility.Hidden;
			_dataContext = null;

			Application.Current.MainWindow.Close();
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue?.ToString();

			if (selectedName is null)
			{
				return;
			}

			if (_dataContext is not null)
			{
				Save();
			}

			_dataContext = GameContent.Dungeons.FirstOrDefault(x => x.Name == selectedName);
			_bossIds = _dataContext.BossIds;
			RefreshStaticValuesPanel();
			RefreshDynamicValuesPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		public void RefreshDynamicValuesPanel()
		{
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Visible;

			foreach (int id in _bossIds)
			{
				Border border = new Border
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = (SolidColorBrush) FindResource("BrushGray2"),
					Padding = new Thickness(6),
					Margin = new Thickness(4)
				};

				Grid grid = CreateDynamicValueGrid(id);

				border.Child = grid;

				DynamicValuesPanel.Children.Add(border);
			}
		}

		private Grid CreateDynamicValueGrid(int id)
		{
			Grid grid = new Grid();

			TextBlock idBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(10, 0, 0, 0),
				FontStyle = FontStyles.Italic,
				Text = $"[{id}]"
			};

			TextBlock nameBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(80, 0, 0, 0),
				Text = GameContent.Bosses.FirstOrDefault(x => x.Id == id).Name
			};

			Button editButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 90, 0),
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right,
				Tag = id
			};

			PackIcon editIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.Edit,
				Foreground = (SolidColorBrush) FindResource("BrushGray2")
			};

			editButton.Content = editIcon;

			editButton.Click += EditDynamicValue_Click;

			Button deleteButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 50, 0),
				Tag = id,
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right
			};

			PackIcon deleteIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.DeleteForever,
				Foreground = (SolidColorBrush) FindResource("BrushGray2")
			};

			deleteButton.Content = deleteIcon;

			deleteButton.Click += DeleteDynamicValue_Click;

			grid.Children.Add(idBlock);
			grid.Children.Add(nameBlock);
			grid.Children.Add(editButton);
			grid.Children.Add(deleteButton);

			return grid;
		}

		private void EditDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			int bossId = int.Parse((sender as Button).Tag.ToString());

			BossIdWindow bossIdWindow = new BossIdWindow(_dataContext, bossId)
			{
				Owner = Application.Current.MainWindow
			};
			bossIdWindow.ShowDialog();

			RefreshDynamicValuesPanel();
		}

		private void DeleteDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			int bossId = int.Parse((sender as Button).Tag.ToString());

			MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete pattern of Id: {bossId}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			_dataContext.BossIds.Remove(bossId);

			RefreshDynamicValuesPanel();
		}

		private void CreateDynamicValueButton_Click(object sender, RoutedEventArgs e)
		{
			var bossId = 0;
			_bossIds.Add(bossId);

			Button tempButton = new Button
			{
				Tag = bossId
			};
			EditDynamicValue_Click(tempButton, null);
		}
	}
}