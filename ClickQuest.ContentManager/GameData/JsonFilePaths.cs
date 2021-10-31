﻿using System;
using System.IO;

namespace ClickQuest.ContentManager.GameData
{
	public static class JsonFilePaths
	{
		public static string ArtifactsFilePath { get; set; }
		public static string BlessingsFilePath { get; set; }
		public static string BossesFilePath { get; set; }
		public static string DungeonsFilePath { get; set; }
		public static string MaterialsFilePath { get; set; }
		public static string MonstersFilePath { get; set; }
		public static string QuestsFilePath { get; set; }
		public static string RecipesFilePath { get; set; }
		public static string RegionsFilePath { get; set; }
		public static string PriestOfferFilePath { get; set; }
		public static string ShopOfferFilePath { get; set; }
		public static string DungeonGroupsFilePath { get; set; }
		public static string DungeonKeysFilePath { get; set; }

		public static void CalculateGameAssetsFilePaths()
		{
			ArtifactsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Artifacts.json");
			BlessingsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Blessings.json");
			BossesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Bosses.json");
			DungeonsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Dungeons.json");
			DungeonGroupsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "DungeonGroups.json");
			DungeonKeysFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "DungeonKeys.json");
			MaterialsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Materials.json");
			MonstersFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Monsters.json");
			QuestsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Quests.json");
			RecipesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Recipes.json");
			RegionsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "Regions.json");
			PriestOfferFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "PriestOffer.json");
			ShopOfferFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"GameData\", @"GameAssets\", "ShopOffer.json");
		}
	}
}