﻿using ClickQuest.ContentManager.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickQuest.ContentManager.UserInterface
{
	public partial class RegionsPanel : UserControl
	{
		private Region _dataContext;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;
		private List<MonsterSpawnPattern> _monsterSpawnPatterns;

		public RegionsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Regions.Select(x => x.Name);
		}

		public void RefreshMainInfoPanel()
		{
			// https://stackoverflow.com/questions/63834841/how-to-add-a-materialdesignhint-to-a-textbox-in-code

			// clear grid's first column to avoid duplicating the controls added below
			// how?
			// use the Dictionary

			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "MainInfoPanel" };

			var selectedRegion = _dataContext as Region;

			var idBox = new TextBox() { Name = "IdBox", Text = selectedRegion.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };
			var nameBox = new TextBox() { Name = "NameBox", Text = selectedRegion.Name, Margin = new Thickness(10) };
			var levelRequirementBox = new TextBox() { Name = "LevelRequirementBox", Text = selectedRegion.LevelRequirement.ToString(), Margin = new Thickness(10) };
			var backgroundBox = new TextBox() { Name = "BackgroundBox", Text = selectedRegion.Background, Margin = new Thickness(10) };
			var descriptionBox = new TextBox()
			{
				Name = "DescriptionBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 80,
				Text = selectedRegion.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(levelRequirementBox, "LevelRequirement");
			HintAssist.SetHint(backgroundBox, "Background");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(levelRequirementBox.Name, levelRequirementBox);
			_controls.Add(backgroundBox.Name, backgroundBox);
			_controls.Add(descriptionBox.Name, descriptionBox);

			foreach (var elem in _controls)
			{
				// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
				if (elem.Value is TextBox textBox)
				{
					textBox.Style = (Style)this.FindResource("MaterialDesignFloatingHintTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style)this.FindResource("MaterialDesignFloatingHintComboBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
				}

				panel.Children.Add(elem.Value);
			}

			Grid.SetColumn(panel, 1);

			_currentPanel = panel;

			MainGrid.Children.Add(panel);
		}

		private void MakeChangesButton_Click(object sender, RoutedEventArgs e)
		{
			var region = _dataContext as Region;

			region.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			region.Name = (_controls["NameBox"] as TextBox).Text;
			region.LevelRequirement = int.Parse((_controls["LevelRequirementBox"] as TextBox).Text);
			region.Background = (_controls["BackgroundBox"] as TextBox).Text;
			region.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Regions.Select(x => x.Id).Contains(region.Id))
			{
				int indexOfOldRegion = GameContent.Regions.FindIndex(x => x.Id == region.Id);
				GameContent.Regions[indexOfOldRegion] = region;
			}
			else
			{
				// If not, add it.
				GameContent.Regions.Add(region);
			}
		}

		private void AddNewButton_Click(object sender, RoutedEventArgs e)
		{
			int nextId = GameContent.Regions.Max(x => x.Id) + 1;
			_dataContext = new Region() { Id = nextId };
			RefreshMainInfoPanel();
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue.ToString();

			_dataContext = GameContent.Regions.FirstOrDefault(x => x.Name == selectedName);
			_monsterSpawnPatterns = _dataContext.Monsters;
			RefreshMainInfoPanel();
			RefreshAdditionalInfoPanel();
		}

		public void RefreshAdditionalInfoPanel()
		{
			AdditionalInfoPanel.Children.Clear();

			foreach (var pattern in _monsterSpawnPatterns)
			{
				var border = new Border
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = new SolidColorBrush(Colors.Gray),
					Padding = new Thickness(6),
					Margin = new Thickness(4),
					Tag = pattern
				};

				border.PreviewMouseUp += Edit;

				var grid = CreateSingleItemGrid(pattern);

				border.Child = grid;

				AdditionalInfoPanel.Children.Add(border);
			}
		}

		private Grid CreateSingleItemGrid(MonsterSpawnPattern pattern)
		{
			var grid = new Grid();

			var idBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(10, 0, 0, 0),
				FontStyle = FontStyles.Italic,
				Text = $"[{pattern.MonsterId}]"
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(80,0,0,0),
				Text = GameContent.Monsters.FirstOrDefault(x => x.Id == pattern.MonsterId).Name
			};

			var frequencyBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(480,0,0,0),
				Text = pattern.Frequency.ToString(CultureInfo.InvariantCulture)
			};

			var deleteButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5,0,50,0),
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

			deleteButton.Click += DeleteButton_Click;

			grid.Children.Add(idBlock);
			grid.Children.Add(nameBlock);
			grid.Children.Add(frequencyBlock);
			grid.Children.Add(deleteButton);

			return grid;
		}

		private void Edit(object sender, MouseButtonEventArgs e)
		{
			var monsterSpawnPattern = (sender as Border).Tag as MonsterSpawnPattern;

			var monsterSpawnPatternWindow = new MonsterSpawnPatternWindow(_dataContext, monsterSpawnPattern) { Owner = Application.Current.MainWindow };
			monsterSpawnPatternWindow.Show();
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			var pattern = (sender as Button).Tag as MonsterSpawnPattern;
			_dataContext.Monsters.Remove(_dataContext.Monsters.FirstOrDefault(x => x.MonsterId == pattern.MonsterId));

			RefreshAdditionalInfoPanel();
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			var newMonsterSpawnPattern = new MonsterSpawnPattern();
			_monsterSpawnPatterns.Add(newMonsterSpawnPattern);

			var tempBorder = new Border() { Tag = newMonsterSpawnPattern };
			Edit(tempBorder, null);
		}

	}
}