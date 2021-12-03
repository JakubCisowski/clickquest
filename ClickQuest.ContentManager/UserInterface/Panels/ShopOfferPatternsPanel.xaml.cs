﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class ShopOfferPatternsPanel : UserControl
	{
		private VendorPattern _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public ShopOfferPatternsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.ShopOffer.Select(x => x.Id.ToString());
		}

		public void RefreshStaticValuesPanel()
		{
			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			StackPanel panel = new StackPanel
			{
				Name = "StaticInfoPanel"
			};

			VendorPattern selectedVendorPattern = _dataContext;

			TextBox idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedVendorPattern.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			_controls.Add(idBox.Name, idBox);

			TextBox vendorIdBox = new TextBox
			{
				Name = "VendorIdBox",
				Text = selectedVendorPattern.VendorItemId.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			_controls.Add(vendorIdBox.Name, vendorIdBox);

			ComboBox nameBox = new ComboBox
			{
				Name = "NameBox",
				Margin = new Thickness(10)
			};
			nameBox.SelectionChanged += NameBox_SelectionChanged;
			_controls.Add(nameBox.Name, nameBox);

			ComboBox vendorTypeBox = new ComboBox
			{
				Name = "VendorTypeBox",
				ItemsSource = Enum.GetValues(typeof(RewardType)),
				Margin = new Thickness(10)
			};
			vendorTypeBox.SelectionChanged += RewardTypeBox_SelectionChanged;
			vendorTypeBox.SelectedValue = selectedVendorPattern.VendorItemType;
			_controls.Add(vendorTypeBox.Name, vendorTypeBox);

			TextBox valueBox = new TextBox
			{
				Name = "ValueBox",
				Text = selectedVendorPattern.Value.ToString(),
				Margin = new Thickness(10)
			};
			_controls.Add(valueBox.Name, valueBox);

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(vendorIdBox, "VendorItemId");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(vendorTypeBox, "VendorTypeBox");
			HintAssist.SetHint(valueBox, "Value");

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

		private void RewardTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_controls.ContainsKey("NameBox"))
			{
				return;
			}

			switch ((RewardType) Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString()))
			{
				case RewardType.Material:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Materials.Select(x => x.Name);
					break;

				case RewardType.Recipe:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Recipes.Select(x => x.Name);
					break;

				case RewardType.Artifact:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Artifacts.Select(x => x.Name);
					break;

				case RewardType.Blessing:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Blessings.Select(x => x.Name);
					break;

				case RewardType.Ingot:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Ingots.Select(x => x.Name);
					break;
			}

			_dataContext.VendorItemType = (RewardType) Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString());

			(_controls["NameBox"] as ComboBox).SelectedIndex = 0;
		}

		private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_controls.ContainsKey("IdBox"))
			{
				return;
			}

			ComboBox comboBox = sender as ComboBox;

			if (comboBox.SelectedValue is null)
			{
				return;
			}

			switch (_dataContext.VendorItemType)
			{
				case RewardType.Material:
					(_controls["VendorIdBox"] as TextBox).Text = GameContent.Materials.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Recipe:
					(_controls["VendorIdBox"] as TextBox).Text = GameContent.Recipes.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Artifact:
					(_controls["VendorIdBox"] as TextBox).Text = GameContent.Artifacts.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Blessing:
					(_controls["VendorIdBox"] as TextBox).Text = GameContent.Blessings.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Ingot:
					(_controls["VendorIdBox"] as TextBox).Text = GameContent.Ingots.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;
			}
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		public void Save()
		{
			VendorPattern vendorPattern = _dataContext;

			if (vendorPattern is null)
			{
				return;
			}

			vendorPattern.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			vendorPattern.VendorItemId = int.Parse((_controls["VendorIdBox"] as TextBox).Text);
			vendorPattern.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			vendorPattern.VendorItemType = (RewardType) Enum.Parse(typeof(RewardType), (_controls["VendorTypeBox"] as ComboBox).SelectedValue.ToString());

			// Check if this Id is already in the collection (modified).
			if (GameContent.ShopOffer.Select(x => x.Id).Contains(vendorPattern.Id))
			{
				int indexOfVendorPattern = GameContent.ShopOffer.FindIndex(x => x.Id == vendorPattern.Id);
				GameContent.ShopOffer[indexOfVendorPattern] = vendorPattern;
			}
			else
			{
				// If not, add it.
				GameContent.ShopOffer.Add(vendorPattern);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = (GameContent.ShopOffer.Max(x => x.Id as int?) ?? 0) + 1;
			_dataContext = new VendorPattern
			{
				Id = nextId
			};
			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();

			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			VendorPattern? objectToDelete = GameContent.ShopOffer.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete pattern of Id: {objectToDelete.Id}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.ShopOffer.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;
			_currentPanel.Children.Clear();
			DeleteObjectButton.Visibility = Visibility.Hidden;
			_dataContext = null;

			Application.Current.MainWindow.Close();
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedValue = (e.Source as ComboBox)?.SelectedValue?.ToString();

			if (selectedValue is null)
			{
				return;
			}

			int selectedId = int.Parse(selectedValue);

			if (_dataContext is not null)
			{
				Save();
			}

			_dataContext = GameContent.ShopOffer.FirstOrDefault(x => x.Id == selectedId);
			ContentSelectionBox.SelectedValue = _dataContext.Id.ToString();
			RefreshStaticValuesPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}
	}
}