﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickQuest.ContentManager.GameData.Models;
using static ClickQuest.ContentManager.GameData.JsonFilePaths;

namespace ClickQuest.ContentManager.GameData
{
	public static class ContentLoader
	{
		public static void LoadAllContent()
		{
			GameContent.Artifacts = LoadContent<Artifact>(ArtifactsFilePath);
			GameContent.Blessings = LoadContent<Blessing>(BlessingsFilePath);
			GameContent.Bosses = LoadContent<Boss>(BossesFilePath);
			GameContent.Dungeons = LoadContent<Dungeon>(DungeonsFilePath);
			GameContent.DungeonGroups = LoadContent<DungeonGroup>(DungeonGroupsFilePath);
			GameContent.DungeonKeys = LoadContent<DungeonKey>(DungeonKeysFilePath);
			GameContent.Ingots = LoadContent<Ingot>(IngotsFilePath);
			GameContent.Materials = LoadContent<Material>(MaterialsFilePath);
			GameContent.Monsters = LoadContent<Monster>(MonstersFilePath);
			GameContent.Quests = LoadContent<Quest>(QuestsFilePath);
			GameContent.Recipes = LoadContent<Recipe>(RecipesFilePath);
			GameContent.Regions = LoadContent<Region>(RegionsFilePath);
			GameContent.PriestOffer = LoadContent<VendorPattern>(PriestOfferFilePath);
			GameContent.ShopOffer = LoadContent<VendorPattern>(ShopOfferFilePath);
			GameContent.GameMechanicsTabs = LoadContent<GameMechanicsTab>(GameMechanicsPath);
		}

		public static List<T> LoadContent<T>(string jsonFilePath)
		{
			string json = File.ReadAllText(jsonFilePath);
			var options = new JsonSerializerOptions
			{
			};

			var objects = JsonSerializer.Deserialize<List<T>>(json, options);

			return objects;
		}
	}
}