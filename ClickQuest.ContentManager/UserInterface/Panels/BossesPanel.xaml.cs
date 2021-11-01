﻿using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class BossesPanel : UserControl
	{
		private Boss _dataContext;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;
		private List<BossLootPattern> _bossLootPatterns;

		public BossesPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Bosses.Select(x => x.Name);
		}

		public void RefreshStaticValuesPanel()
		{
			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "StaticInfoPanel" };

			var selectedBoss = _dataContext;

			var idBox = new TextBox() { Name = "IdBox", Text = selectedBoss.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };
			var nameBox = new TextBox() { Name = "NameBox", Text = selectedBoss.Name, Margin = new Thickness(10) };
			var healthBox = new TextBox() { Name = "HealthBox", Text = selectedBoss.Health.ToString(), Margin = new Thickness(10) };
			var imageBox = new TextBox() { Name = "ImageBox", Text = selectedBoss.Image, Margin = new Thickness(10) };
			var descriptionBox = new TextBox()
			{
				Name = "DescriptionBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedBoss.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(healthBox, "Health");
			HintAssist.SetHint(imageBox, "Image");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(healthBox.Name, healthBox);
			_controls.Add(imageBox.Name, imageBox);
			_controls.Add(descriptionBox.Name, descriptionBox);

			foreach (var elem in _controls)
			{
				// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
				if (elem.Value is TextBox textBox)
				{
					textBox.Style = (Style)this.FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
					textBox.GotFocus += TextBox_GotFocus;
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style)this.FindResource("MaterialDesignOutlinedComboBox");
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
			var boss = _dataContext;

			if (boss is null)
			{
				return;
			}

			boss.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			boss.Name = (_controls["NameBox"] as TextBox).Text;
			boss.Health = int.Parse((_controls["HealthBox"] as TextBox).Text);
			boss.Image = (_controls["ImageBox"] as TextBox).Text;
			boss.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Bosses.Select(x => x.Id).Contains(boss.Id))
			{
				int indexOfOldBoss = GameContent.Bosses.FindIndex(x => x.Id == boss.Id);
				GameContent.Bosses[indexOfOldBoss] = boss;
			}
			else
			{
				// If not, add it.
				GameContent.Bosses.Add(boss);
			}

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedValue = _dataContext.Name;
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = GameContent.Bosses.Max(x => x.Id) + 1;

			_dataContext = new Boss() { Id = nextId, BossLootPatterns = new List<BossLootPattern>() };
			_bossLootPatterns = _dataContext.BossLootPatterns;

			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();
			DynamicValuesPanel.Children.Clear();

			DeleteObjectButton.Visibility=Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			var objectToDelete = GameContent.Bosses.FirstOrDefault(x=>x.Id==int.Parse((_controls["IdBox"] as TextBox).Text));

			if (objectToDelete is null)
			{
				_currentPanel?.Children.Clear();
				DeleteObjectButton.Visibility=Visibility.Hidden;
				return;
			}

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.Bosses.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;

			_currentPanel.Children.Clear();
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Hidden;
			DeleteObjectButton.Visibility=Visibility.Hidden;
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

			_dataContext = GameContent.Bosses.FirstOrDefault(x => x.Name == selectedName);
			_bossLootPatterns = _dataContext.BossLootPatterns;
			RefreshStaticValuesPanel();
			RefreshDynamicValuesPanel();
			DeleteObjectButton.Visibility=Visibility.Visible;
		}

		public void RefreshDynamicValuesPanel()
		{
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Visible;

			foreach (var pattern in _bossLootPatterns)
			{
				var border = new Border
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = new SolidColorBrush(Colors.Gray),
					Padding = new Thickness(6),
					Margin = new Thickness(4)
				};

				var grid = CreateDynamicValueGrid(pattern);

				border.Child = grid;

				DynamicValuesPanel.Children.Add(border);
			}
		}

		private Grid CreateDynamicValueGrid(BossLootPattern pattern)
		{
			var grid = new Grid();

			var idBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(10, 0, 0, 0),
				FontStyle = FontStyles.Italic,
				Text = $"[{pattern.BossLootId}]"
			};

			var itemTypeBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(80, 0, 0, 0),
				Text = pattern.BossLootType.ToString()
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(180, 0, 0, 0),
			};

			switch (pattern.BossLootType)
			{
				case RewardType.Material:
					nameBlock.Text = GameContent.Materials.FirstOrDefault(x => x.Id == pattern.BossLootId).Name;
					break;

				case RewardType.Recipe:
					nameBlock.Text = GameContent.Recipes.FirstOrDefault(x => x.Id == pattern.BossLootId).Name;
					break;

				case RewardType.Artifact:
					nameBlock.Text = GameContent.Artifacts.FirstOrDefault(x => x.Id == pattern.BossLootId).Name;
					break;

				case RewardType.Blessing:
					nameBlock.Text = GameContent.Blessings.FirstOrDefault(x => x.Id == pattern.BossLootId).Name;
					break;

				case RewardType.Ingot:
					nameBlock.Text = GameContent.Ingots.FirstOrDefault(x=>x.Id == pattern.BossLootId).Name;
					break;
			}

			var frequenciesBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(460, 0, 0, 0),
				Text = string.Join(' ', pattern.Frequencies.Select(x => x.ToString()))
			};

			var editButton = new Button
			{	
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 60, 0),
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right,
				Tag = pattern
			};

			var editIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.Edit,
				Foreground = new SolidColorBrush(Colors.Gray)
			};

			editButton.Content = editIcon;

			editButton.Click += EditDynamicValue_Click;

			var deleteButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 20, 0),
				Tag = pattern,
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right
			};

			var deleteIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.DeleteForever,
				Foreground = new SolidColorBrush(Colors.Gray)
			};

			deleteButton.Content = deleteIcon;

			deleteButton.Click += DeleteDynamicValue_Click;

			grid.Children.Add(idBlock);
			grid.Children.Add(itemTypeBlock);
			grid.Children.Add(nameBlock);
			grid.Children.Add(frequenciesBlock);
			grid.Children.Add(editButton);
			grid.Children.Add(deleteButton);

			return grid;
		}

		private void EditDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			var bossLootPattern = (sender as Button).Tag as BossLootPattern;

			var bossLootPatternWindow = new BossLootPatternWindow(_dataContext, bossLootPattern) { Owner = Application.Current.MainWindow };
			bossLootPatternWindow.ShowDialog();

			RefreshDynamicValuesPanel();
		}

		private void DeleteDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			var pattern = (sender as Button).Tag as BossLootPattern;

			var result = MessageBox.Show($"Are you sure you want to delete pattern of Id: {pattern.BossLootId}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			_dataContext.BossLootPatterns.Remove(_dataContext.BossLootPatterns.FirstOrDefault(x => x.BossLootId == pattern.BossLootId));

			RefreshDynamicValuesPanel();
		}

		private void CreateDynamicValueButton_Click(object sender, RoutedEventArgs e)
		{
			var newBossLootPattern = new BossLootPattern();
			newBossLootPattern.Frequencies = new List<double>()
			{
				0,0,0,0,0,0
			};
			_bossLootPatterns.Add(newBossLootPattern);

			var tempButton = new Button() { Tag = newBossLootPattern };
			EditDynamicValue_Click(tempButton, null);
		}

	}
}