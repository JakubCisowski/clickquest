using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.Extensions.UserInterface.ToolTips;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class InfoPage : Page
	{
		private Page _previousPage;
		private string _previousLocationInfo;

		public InfoPage(Page previousPage, string previousLocationInfo)
		{
			InitializeComponent();

			_previousPage = previousPage;
			_previousLocationInfo = previousLocationInfo;

			RegionsListView.ItemsSource = GameAssets.Regions.Select(x => x.Name);
			DungeonsListView.ItemsSource = GameAssets.Dungeons.Select(x => x.Name);
			GameMechanicsListView.ItemsSource = GameAssets.GameMechanicsTabs.Select(x => x.Name);
		}

		private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(_previousPage, _previousLocationInfo);
		}

		private void RegionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
			{
				return;
			}

			var region = GameAssets.Regions.FirstOrDefault(x => x.Name == e.AddedItems[0].ToString());

			if (region != null)
			{
				GenerateRegionInfoPanel(region);

				DungeonsListView.SelectedItems.Clear();
			}
		}

		private void DungeonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
			{
				return;
			}

			var dungeon = GameAssets.Dungeons.FirstOrDefault(x => x.Name == e.AddedItems[0].ToString());

			if (dungeon != null)
			{
				GenerateDungeonInfoPanel(dungeon);

				RegionsListView.SelectedItems.Clear();
			}
		}

		private void GenerateRegionInfoPanel(Region region)
		{
			InfoPanel.Children.Clear();

			var regionNameBlock = new TextBlock()
			{
				Text=region.Name,
				FontSize = 26,
				FontFamily = (FontFamily)this.FindResource("FontFancy"),
				TextAlignment=TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			var levelRequirementBlock = new TextBlock()
			{
				Text="Level Requirement: " + region.LevelRequirement,
				FontSize = 20,
				TextAlignment=TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			var descriptionBlock = new TextBlock()
			{
				Text = region.Description,
				FontSize = 18,
				TextAlignment=TextAlignment.Justify,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				TextWrapping = TextWrapping.Wrap,
				FontFamily = (FontFamily)this.FindResource("FontFancy")
			};

			var separator = new Separator()
			{
				Height = 2,
				Width = 400,
				Margin = new Thickness(30)
			};

			InfoPanel.Children.Add(regionNameBlock);
			InfoPanel.Children.Add(levelRequirementBlock);
			InfoPanel.Children.Add(descriptionBlock);
			InfoPanel.Children.Add(separator);

			var sortedMonsterPatterns = region.MonsterSpawnPatterns.OrderByDescending(x => x.Monster.BestiaryDiscovered).ThenByDescending(y => y.Frequency);

			foreach (var monsterPattern in sortedMonsterPatterns)
			{
				var monster = monsterPattern.Monster;
				
				var monsterNameBlock = new TextBlock()
				{
					FontSize = 22,
					FontFamily = (FontFamily)this.FindResource("FontFancy"),
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5)
				};

				if (!monster.BestiaryDiscovered)
				{
					monsterNameBlock.Text = "Unknown Monster";

					GeneralToolTipController.SetToolTipDelayAndDuration(monsterNameBlock);
					monsterNameBlock.ToolTip = GeneralToolTipController.GenerateUndiscoveredEnemyToolTip();
				}
				else
				{
					monsterNameBlock.Text = monster.Name;
				}

				InfoPanel.Children.Add(monsterNameBlock);

				if (!monster.BestiaryDiscovered)
				{
					var monsterSeparator = new Separator()
					{
						Height = 2,
						Width = 200,
						Margin = new Thickness(10)
					};

					InfoPanel.Children.Add(monsterSeparator);

					continue;
				}

				var monsterHealthBlock = new TextBlock()
				{
					Text="Health: " + monster.Health,
					FontSize = 16,
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5)
				};

				InfoPanel.Children.Add(monsterHealthBlock);

				var monsterDescriptionBlock = new TextBlock()
				{
					Text=monster.Description,
					FontSize = 16,
					TextAlignment=TextAlignment.Justify,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5),
					TextWrapping = TextWrapping.Wrap
				};

				InfoPanel.Children.Add(monsterDescriptionBlock);

				var lootLabelBlock = new TextBlock()
				{
					Text="Loot: ",
					FontSize = 18,
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5),
				};

				InfoPanel.Children.Add(lootLabelBlock);

				var sortedLootPatterns = monster.MonsterLootPatterns.OrderByDescending(x => x.BestiaryDiscovered).ThenBy(y => y.Item.Rarity);

				foreach (var lootPattern in sortedLootPatterns)
				{
					var item = lootPattern.Item;

					var itemNameBlock = new TextBlock()
					{
						TextAlignment=TextAlignment.Center,
						HorizontalAlignment = HorizontalAlignment.Center,
						Margin = new Thickness(5),
						Foreground = ColorsController.GetRarityColor(item.Rarity),
						FontSize = 16
					};

					GeneralToolTipController.SetToolTipDelayAndDuration(itemNameBlock);

					if (!lootPattern.BestiaryDiscovered)
					{
						itemNameBlock.Text = "Unknown " + item.RarityString + " " + lootPattern.MonsterLootType;
						itemNameBlock.FontFamily = (FontFamily)this.FindResource("FontRegularItalic");
						itemNameBlock.ToolTip = ItemToolTipController.GenerateUndiscoveredItemToolTip();
					}
					else
					{
						itemNameBlock.Text = item.Name;
						itemNameBlock.FontFamily = (FontFamily)this.FindResource("FontRegularBold");
						itemNameBlock.ToolTip = ItemToolTipController.GenerateItemToolTip(item);
					}

					InfoPanel.Children.Add(itemNameBlock);
				}

				var itemSeparator = new Separator()
				{
					Height = 2,
					Width = 200,
					Margin = new Thickness(15)
				};

				InfoPanel.Children.Add(itemSeparator);
			}
		}

		private void GenerateDungeonInfoPanel(Dungeon dungeon)
		{
			InfoPanel.Children.Clear();

			var dungeonNameBlock = new TextBlock()
			{
				Text=dungeon.Name,
				FontSize = 26,
				FontFamily = (FontFamily)this.FindResource("FontFancy"),
				TextAlignment=TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			var dungeonGroupBlock = new TextBlock()
			{
				Text=dungeon.DungeonGroup.Name,
				FontSize = 20,
				TextAlignment=TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				Foreground = ColorsController.GetRarityColor((Rarity)dungeon.DungeonGroupId)
			};

			var descriptionBlock = new TextBlock()
			{
				Text = dungeon.Description,
				FontSize = 18,
				TextAlignment=TextAlignment.Justify,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				TextWrapping = TextWrapping.Wrap,
				FontFamily = (FontFamily)this.FindResource("FontFancy")
			};

			var separator = new Separator()
			{
				Height = 2,
				Width = 400,
				Margin = new Thickness(30)
			};

			InfoPanel.Children.Add(dungeonNameBlock);
			InfoPanel.Children.Add(dungeonGroupBlock);
			InfoPanel.Children.Add(descriptionBlock);
			InfoPanel.Children.Add(separator);

			var sortedBosses = GameAssets.Bosses.Where(x => dungeon.BossIds.Contains(x.Id)).OrderByDescending(y => y.BestiaryDiscovered);

			foreach (var boss in sortedBosses)
			{				
				var bossNameBlock = new TextBlock()
				{
					FontSize = 22,
					FontFamily = (FontFamily)this.FindResource("FontFancy"),
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5)
				};

				if (!boss.BestiaryDiscovered)
				{
					bossNameBlock.Text = "Unknown Boss";

					GeneralToolTipController.SetToolTipDelayAndDuration(bossNameBlock);
					bossNameBlock.ToolTip = GeneralToolTipController.GenerateUndiscoveredEnemyToolTip();
				}
				else
				{
					bossNameBlock.Text = boss.Name;
				}

				InfoPanel.Children.Add(bossNameBlock);

				if (!boss.BestiaryDiscovered)
				{
					var bossSeparator = new Separator()
					{
						Height = 2,
						Width = 200,
						Margin = new Thickness(10)
					};

					InfoPanel.Children.Add(bossSeparator);

					continue;
				}

				var bossAffixesStrings = new List<string>();

				foreach (var affix in boss.Affixes)
				{
					var	affixString = string.Concat(affix.ToString().Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
					bossAffixesStrings.Add(affixString);
				}

				var bossAffixBlock = new TextBlock()
				{
					Text="Affixes: " + string.Join(" / ", bossAffixesStrings),
					FontSize = 16,
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5)
				};

				InfoPanel.Children.Add(bossAffixBlock);

				var bossHealthBlock = new TextBlock()
				{
					Text="Health: " + boss.Health,
					FontSize = 16,
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5)
				};

				InfoPanel.Children.Add(bossHealthBlock);

				var bossDescriptionBlock = new TextBlock()
				{
					Text=boss.Description,
					FontSize = 16,
					TextAlignment=TextAlignment.Justify,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5),
					TextWrapping = TextWrapping.Wrap
				};

				InfoPanel.Children.Add(bossDescriptionBlock);

				var lootLabelBlock = new TextBlock()
				{
					Text="Loot: ",
					FontSize = 18,
					TextAlignment=TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5),
				};

				InfoPanel.Children.Add(lootLabelBlock);

				var sortedLootPatterns = boss.BossLootPatterns.OrderByDescending(x => x.BestiaryDiscovered).ThenBy(y => y.Item.Rarity);

				foreach (var lootPattern in sortedLootPatterns)
				{
					var item = lootPattern.Item;

					var itemNameBlock = new TextBlock()
					{
						TextAlignment=TextAlignment.Center,
						HorizontalAlignment = HorizontalAlignment.Center,
						Margin = new Thickness(5),
						Foreground = ColorsController.GetRarityColor(item.Rarity),
						FontSize = 16
					};

					GeneralToolTipController.SetToolTipDelayAndDuration(itemNameBlock);

					if (!lootPattern.BestiaryDiscovered)
					{
						itemNameBlock.Text = "Unknown " + item.RarityString + " " + lootPattern.BossLootType;
						itemNameBlock.FontFamily = (FontFamily)this.FindResource("FontRegularItalic");
						itemNameBlock.ToolTip = ItemToolTipController.GenerateUndiscoveredItemToolTip();
					}
					else
					{
						itemNameBlock.Text = item.Name;
						itemNameBlock.FontFamily = (FontFamily)this.FindResource("FontRegularBold");
						
						if (lootPattern.BossLootType == Core.Items.Types.RewardType.Blessing)
						{
							itemNameBlock.ToolTip = ItemToolTipController.GenerateBlessingToolTip(GameAssets.Blessings.FirstOrDefault(x=>x.Id==lootPattern.BossLootId));
						}
						else
						{
							itemNameBlock.ToolTip = ItemToolTipController.GenerateItemToolTip(item);
						}
					}

					InfoPanel.Children.Add(itemNameBlock);
				}

				var itemSeparator = new Separator()
				{
					Height = 2,
					Width = 200,
					Margin = new Thickness(15)
				};

				InfoPanel.Children.Add(itemSeparator);
			}
		}

		private void GameMechanicsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			InfoPanel.Children.Clear();

			var selectedName = e.AddedItems[0].ToString();

			var description = GameAssets.GameMechanicsTabs.FirstOrDefault(x => x.Name == selectedName).Description;

			var nameTextBlock = new TextBlock()
			{
				Text=selectedName,
				FontSize = 24,
				FontFamily = (FontFamily)this.FindResource("FontFancy"),
				TextAlignment=TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			var separator = new Separator()
			{
				Height = 2,
				Width = 400,
				Margin = new Thickness(10)
			};

			var descriptionTextBlock = new TextBlock()
			{
				FontSize = 18,
				TextAlignment=TextAlignment.Justify,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				TextWrapping = TextWrapping.Wrap
			};

			descriptionTextBlock.Inlines.AddRange(DescriptionsController.GenerateDescriptionRuns(description));

			InfoPanel.Children.Add(nameTextBlock);
			InfoPanel.Children.Add(separator);
			InfoPanel.Children.Add(descriptionTextBlock);
		}
	}
}