﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;
using static ClickQuest.Game.Extensions.UserInterface.ToolTips.GeneralToolTipController;
using Colors = ClickQuest.Game.Extensions.UserInterface.ColorsController;

namespace ClickQuest.Game.Extensions.UserInterface.ToolTips
{
	public static class ItemToolTipController
	{
		const string TagOpeningStart = "<";
		const string TagClosingStart = "</";
		const string TagEnd = ">";

		public static ToolTip GenerateItemToolTip<T>(T itemToGenerateToolTipFor) where T : Item
		{
			var toolTip = new ToolTip()
			{
				BorderBrush = Colors.GetRarityColor(itemToGenerateToolTipFor.Rarity)
			};

			var toolTipBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
			};

			switch (itemToGenerateToolTipFor)
			{
				case Material material:
					{
						toolTipBlock.Inlines.Add(new Run($"{material.Name}"){FontSize=(double)Application.Current.FindResource("FontSizeToolTipName")});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"*{material.RarityString}*")
						{
							Foreground = Colors.GetRarityColor(material.Rarity),
							FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
						});
						
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());

						toolTipBlock.Inlines.Add(new Run($"{material.Description}") {FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"), Foreground=(SolidColorBrush)Application.Current.FindResource("BrushGame0")});

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());
						
						toolTipBlock.Inlines.Add(new Run("Value: "));
						toolTipBlock.Inlines.Add(new Run($"{material.Value} gold") {Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGold")});
					}
					break;

				case Artifact artifact:
					{
						toolTipBlock.Inlines.Add(new Run($"{artifact.Name}"){FontSize=(double)Application.Current.FindResource("FontSizeToolTipName")});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"*{artifact.RarityString}*")
						{
							Foreground = Colors.GetRarityColor(artifact.Rarity),
							FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
						});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{artifact.ArtifactType.ToString()}"));

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());

						var listOfDescriptionRuns = GenerateArtifactDescriptionRuns(artifact.Description);
						foreach (var run in listOfDescriptionRuns)
						{
							toolTipBlock.Inlines.Add(run);
						}
						
						if (!string.IsNullOrWhiteSpace(artifact.ExtraInfo))
						{
							toolTipBlock.Inlines.Add(new LineBreak());
							toolTipBlock.Inlines.Add(new LineBreak());
							toolTipBlock.Inlines.Add(new Run($"{artifact.ExtraInfo}") {FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"), Foreground=(SolidColorBrush)Application.Current.FindResource("BrushGame1")});
						}

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());

						toolTipBlock.Inlines.Add(new Run($"{artifact.Lore}") {FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"), Foreground=(SolidColorBrush)Application.Current.FindResource("BrushGame0")});

						if (artifact.Rarity == Rarity.Mythic)
						{
							toolTipBlock.Inlines.Add(new LineBreak());
							toolTipBlock.Inlines.Add(new LineBreak());
							toolTipBlock.Inlines.Add(new Run($"{artifact.MythicTag}") { FontFamily = (FontFamily)Application.Current.FindResource("FontFancy"), Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGame2")});
						}
					}
					break;

				case Recipe recipe:
					{
						toolTipBlock.Inlines.Add(new Run($"{recipe.Name}") {FontSize=(double)Application.Current.FindResource("FontSizeToolTipName")});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"*{recipe.RarityString}*")
						{
							Foreground = Colors.GetRarityColor(recipe.Rarity),
							FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
						});

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());

						var listOfDescriptionRuns = GenerateArtifactDescriptionRuns(recipe.Description);
						foreach (var run in listOfDescriptionRuns)
						{
							toolTipBlock.Inlines.Add(run);
						}

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());

						var listOfIngredientRuns = GenerateRecipeIngredientsRuns(recipe);
						foreach (var run in listOfIngredientRuns)
						{
							toolTipBlock.Inlines.Add(run);
						}
						
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(GenerateTextSeparator());
						toolTipBlock.Inlines.Add(new LineBreak());
						
						toolTipBlock.Inlines.Add(new Run("Value: "));
						toolTipBlock.Inlines.Add(new Run($"{recipe.Value} gold") {Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGold")});
					}
					break;
			}

			toolTip.Content = toolTipBlock;

			return toolTip;
		}

		public static ToolTip GenerateCurrencyToolTip<T>(int rarityValue) where T : Item
		{
			var currencyToolTip = new ToolTip()
			{
				Style = (Style)Application.Current.FindResource("ToolTipSimple")
			};

			var currencyToolTipTextBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
				FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
			};

			var currencyColor = Colors.GetRarityColor((Rarity)rarityValue);
			currencyToolTipTextBlock.Foreground = currencyColor;

			currencyToolTipTextBlock.Text = (Rarity)rarityValue + " " + (typeof(T) == typeof(Ingot) ? "Ingots" : "Dungeon Keys");

			currencyToolTip.Content = currencyToolTipTextBlock;

			return currencyToolTip;
		}

		public static ToolTip GenerateBlessingToolTip(Blessing blessing)
		{
			var blessingToolTip = new ToolTip();

			var blessingToolTipTextBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
			};
			
			blessingToolTipTextBlock.Inlines.Add(new Run($"{blessing.Name}"));
			blessingToolTipTextBlock.Inlines.Add(new LineBreak());
			blessingToolTipTextBlock.Inlines.Add(new Run($"*{blessing.RarityString}*")
			{
				Foreground = Colors.GetRarityColor((Rarity)blessing.Rarity),
				FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
			});
			blessingToolTipTextBlock.Inlines.Add(new LineBreak());
			blessingToolTipTextBlock.Inlines.Add(new LineBreak());
			blessingToolTipTextBlock.Inlines.Add(new Run($"{blessing.Description}"));
			blessingToolTipTextBlock.Inlines.Add(new LineBreak());
			blessingToolTipTextBlock.Inlines.Add(new Run($"Duration: {blessing.Duration}s"));

			blessingToolTip.Content = blessingToolTipTextBlock;

			return blessingToolTip;
		}

		public static List<Run> GenerateArtifactDescriptionRuns(string description)
		{
			var artifactDescriptionRuns = new List<Run>();

			// If it's called before loading descriptions.
			if (description is null)
			{
				return artifactDescriptionRuns;
			}

			while (true)
			{
				int indexOfTagOpeningStart = description.IndexOf(TagOpeningStart);

				if (indexOfTagOpeningStart == -1)
				{
					// Tag opening not found - create a normal Run with the remainder of description.
					artifactDescriptionRuns.Add(new Run(description));
					break;
				}
				else
				{
					if (indexOfTagOpeningStart != 0)
					{
						// If tag opening index is not zero, first create a normal Run with that part of description.
						string taglessPart = description.Substring(0, indexOfTagOpeningStart);
						artifactDescriptionRuns.Add(new Run(taglessPart));
						description = description.Remove(0, indexOfTagOpeningStart);
						indexOfTagOpeningStart = 0;
					}
					
					// Find closing tag.
					int indexOfTagOpeningEnd = description.IndexOf(TagEnd);
					int indexOfTagClosingStart = description.IndexOf(TagClosingStart);

					string tagType = description.Substring(1, indexOfTagOpeningEnd - indexOfTagOpeningStart - 1).ToUpper();
					
					string taggedPart = description.Substring(indexOfTagOpeningEnd + 1, indexOfTagClosingStart - indexOfTagOpeningEnd - 1);

					var coloredRun = new Run(taggedPart);

					switch (tagType)
					{
						// Damage type
						case "NORMAL":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Normal);
							break;
						case "CRITICAL":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Critical);
							break;
						case "POISON":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Poison);
							break;
						case "AURA":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Aura);
							break;
						case "ONHIT":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.OnHit);
							break;
						case "ARTIFACT":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Artifact);
							break;
							
						// Class 
						case "SLAYER":
							coloredRun.Foreground = ColorsController.GetHeroClassColor(HeroClass.Slayer);
							break;
						case "VENOM":
							coloredRun.Foreground = ColorsController.GetHeroClassColor(HeroClass.Venom);
							break;

						// Rarity
						case "GENERAL":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.General);
							break;
						case "FINE":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Fine);
							break;
						case "SUPERIOR":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Superior);
							break;
						case "EXCEPTIONAL":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Exceptional);
							break;
						case "MASTERWORK":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Masterwork);
							break;
						case "MYTHIC":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Mythic);
							break;
					}

					artifactDescriptionRuns.Add(coloredRun);

					// Remove opening tag, tagged part and closing tag from description.
					description = description.Remove(0, indexOfTagClosingStart);
					int indexOfTagClosingEnd = description.IndexOf(TagEnd);
					description = description.Remove(0, indexOfTagClosingEnd + 1);
				}
			}

			return artifactDescriptionRuns;
		}
	
		public static List<Run> GenerateRecipeIngredientsRuns(Recipe recipe)
		{
			var ingredientRuns = new List<Run>();

			ingredientRuns.Add(new Run("Ingredients:\n") { FontSize = (double)Application.Current.FindResource("FontSizeToolTipIngredientText")});

			foreach (var ingredient in recipe.IngredientPatterns)
			{
				var relatedMaterial = ingredient.RelatedMaterial;
				ingredientRuns.Add(new Run($"{ingredient.Quantity}x "));
				ingredientRuns.Add(new Run($"{relatedMaterial.Name}"){Foreground=Colors.GetRarityColor(relatedMaterial.Rarity)});
				ingredientRuns.Add(new Run("\n"));
			}

			ingredientRuns.RemoveAt(ingredientRuns.Count - 1);

			return ingredientRuns;
		}
	}
}
