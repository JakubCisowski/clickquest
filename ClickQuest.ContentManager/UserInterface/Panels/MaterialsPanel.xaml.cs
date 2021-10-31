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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class MaterialsPanel : UserControl
	{
		private Material _dataContext;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public MaterialsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Materials.Select(x => x.Name);
		}

		public void RefreshStaticValuesPanel()
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

			var selectedMaterial = _dataContext;

			var idBox = new TextBox() { Name = "IdBox", Text = selectedMaterial.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };
			var nameBox = new TextBox() { Name = "NameBox", Text = selectedMaterial.Name, Margin = new Thickness(10) };
			var valueBox = new TextBox() { Name = "ValueBox", Text = selectedMaterial.Value.ToString(), Margin = new Thickness(10) };
			var rarityBox = new ComboBox() { Name = "RarityBox", ItemsSource = Enum.GetValues(typeof(Rarity)), SelectedIndex = (int)selectedMaterial.Rarity, Margin = new Thickness(10) };
			var descriptionBox = new TextBox()
			{
				Name = "DescriptionBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedMaterial.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(valueBox, "Value");
			HintAssist.SetHint(rarityBox, "Rarity");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(valueBox.Name, valueBox);
			_controls.Add(rarityBox.Name, rarityBox);
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

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var material = _dataContext as Material;

			material.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			material.Name = (_controls["NameBox"] as TextBox).Text;
			material.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			material.Rarity = (Rarity)Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
			material.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Materials.Select(x => x.Id).Contains(material.Id))
			{
				int indexOfOldMaterial = GameContent.Materials.FindIndex(x => x.Id == material.Id);
				GameContent.Materials[indexOfOldMaterial] = material;
			}
			else
			{
				// If not, add it.
				GameContent.Materials.Add(material);
			}

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedValue = _dataContext.Name;
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			int nextId = GameContent.Materials.Max(x => x.Id) + 1;
			_dataContext = new Material() { Id = nextId };
			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			var objectToDelete = GameContent.DungeonKeys.FirstOrDefault(x=>x.Id==int.Parse((_controls["IdBox"] as TextBox).Text));

			if (objectToDelete is null)
			{
				_currentPanel?.Children.Clear();
				DeleteObjectButton.Visibility=Visibility.Hidden;
				SaveButton.Visibility=Visibility.Hidden;
				return;
			}

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.DungeonKeys.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;
			_currentPanel.Children.Clear();
			DeleteObjectButton.Visibility=Visibility.Hidden;
			SaveButton.Visibility=Visibility.Hidden;
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue?.ToString();

			if (selectedName is null)
			{
				return;
			}

			_dataContext = GameContent.Materials.FirstOrDefault(x => x.Name == selectedName);
			RefreshStaticValuesPanel();
			DeleteObjectButton.Visibility=Visibility.Visible;
			SaveButton.Visibility = Visibility.Visible;
		}
	}
}