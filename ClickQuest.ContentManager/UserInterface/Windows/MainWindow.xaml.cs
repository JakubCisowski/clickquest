﻿using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.UserInterface.Panels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class MainWindow : Window
	{
		private UserControl _currentPanel;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl tabControl)
			{
				SaveObjectChanges();

				var currentTabName = (tabControl.SelectedItem as TabItem).Header.ToString();
				var currentTabNameAsContentType = (ContentType)Enum.Parse(typeof(ContentType), currentTabName.Replace(" ", ""));

				switch (currentTabNameAsContentType)
				{
					case ContentType.Artifacts:
						var artifactsPanel = new ArtifactsPanel();
						_currentPanel = artifactsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(artifactsPanel);
						break;

					case ContentType.Regions:
						var regionsPanel = new RegionsPanel();
						_currentPanel = regionsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(regionsPanel);
						break;

					case ContentType.Materials:
						var materialsPanel = new MaterialsPanel();
						_currentPanel = materialsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(materialsPanel);
						break;

					case ContentType.Recipes:
						var recipesPanel = new RecipesPanel();
						_currentPanel = recipesPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(recipesPanel);
						break;

					case ContentType.Blessings:
						var blessingsPanel = new BlessingsPanel();
						_currentPanel = blessingsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(blessingsPanel);
						break;

					case ContentType.Bosses:
						var bossesPanel = new BossesPanel();
						_currentPanel = bossesPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(bossesPanel);
						break;

					case ContentType.DngGroups:
						var dngGroupsPanel = new DungeonGroupsPanel();
						_currentPanel = dngGroupsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dngGroupsPanel);
						break;

					case ContentType.DngKeys:
						var dngKeysPanel = new DungeonKeysPanel();
						_currentPanel = dngKeysPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dngKeysPanel);
						break;

					case ContentType.Dungeons:
						var dungeonsPanel = new DungeonsPanel();
						_currentPanel = dungeonsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dungeonsPanel);
						break;
				}
			}
		}

		private void SaveObjectChanges()
		{
			if (_currentPanel is null)
			{
				return;
			}

			switch (_currentPanel)
			{
				case ArtifactsPanel artifactsPanel:
					artifactsPanel.Save();
					break;

				case RegionsPanel regionsPanel:
					regionsPanel.Save();
					break;

				case MaterialsPanel materialsPanel:
					materialsPanel.Save();
					break;

				case RecipesPanel recipesPanel:
					recipesPanel.Save();
					break;

				case BlessingsPanel blessingsPanel:
					blessingsPanel.Save();
					break;

				case BossesPanel bossesPanel:
					bossesPanel.Save();
					break;

				case DungeonGroupsPanel dungeonGroupsPanel:
					dungeonGroupsPanel.Save();
					break;

				case DungeonKeysPanel dungeonKeysPanel:
					dungeonKeysPanel.Save();
					break;
			}
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			SaveObjectChanges();

			ContentSaver.SaveAllContent();
		}
	}
}