﻿using System;
using System.IO;

namespace ClickQuest.ContentManager.Logic.Helpers;

public static class FilePathHelper
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
	public static string IngotsFilePath { get; set; }
	public static string GameMechanicsPath { get; set; }

	public static void CalculateGameAssetsFilePaths()
	{
		ArtifactsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Artifacts.aes");
		BlessingsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Blessings.aes");
		BossesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Bosses.aes");
		DungeonsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Dungeons.aes");
		DungeonGroupsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "DungeonGroups.aes");
		DungeonKeysFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "DungeonKeys.aes");
		MaterialsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Materials.aes");
		MonstersFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Monsters.aes");
		QuestsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Quests.aes");
		RecipesFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Recipes.aes");
		RegionsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Regions.aes");
		PriestOfferFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "PriestOffer.aes");
		ShopOfferFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "ShopOffer.aes");
		IngotsFilePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "Ingots.aes");
		GameMechanicsPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, @"ClickQuest.Game\", @"Data", @"GameAssets\", "GameMechanics.aes");
	}
}