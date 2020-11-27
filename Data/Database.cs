using ClickQuest.Enemies;
using ClickQuest.Items;
using ClickQuest.Places;
using ClickQuest.Pages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ClickQuest.Data
{
	public static partial class Database
	{
		public static List<Material> Materials { get; set; }
		public static List<Monster> Monsters { get; set; }
		public static List<Region> Regions { get; set; }
		public static Dictionary<string,Page> Pages{ get; set;}

        public static void Load()
		{
			Materials = new List<Material>();
			Monsters = new List<Monster>();
			Regions = new List<Region>();
            Pages = new Dictionary<string, Page>();

            LoadMaterials();
			LoadMonsters();
			LoadRegions();
			LoadPages();
		}

		public static void LoadMaterials()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Materials.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Materials"];

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Materials"][i]["Id"].ToString());
				var name = parsedObject["Materials"][i]["Name"].ToString();
                var rarity = (Rarity)int.Parse(parsedObject["Materials"][i]["Rarity"].ToString());
                var value = int.Parse(parsedObject["Materials"][i]["Value"].ToString());

				var newMaterial = new Material(id, name, rarity, value);
				Materials.Add(newMaterial);
			}
		}
		public static void LoadMonsters()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Monsters.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Monsters"];

			// Error check
			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Monsters"][i]["Id"].ToString());
				var name = parsedObject["Monsters"][i]["Name"].ToString();
				var health = int.Parse(parsedObject["Monsters"][i]["Health"].ToString());
				var image = parsedObject["Monsters"][i]["Image"].ToString();

				var typesTemp = new List<MonsterType>();
				var typeArray = (JArray)parsedObject["Monsters"][i]["Types"];

				for (int j = 0; j < typeArray.Count; j++)
				{
					var monsterType = (MonsterType)Enum.Parse(typeof(MonsterType), parsedObject["Monsters"][i]["Types"][j].ToString());
					typesTemp.Add(monsterType);
				}

				var lootTemp = new List<(Material, double)>();
				var lootArray = (JArray)parsedObject["Monsters"][i]["Loot"];

				// Error check
				double frequencySum = 0;

				for (int j = 0; j < lootArray.Count; j++)
				{
					var materialId = int.Parse(parsedObject["Monsters"][i]["Loot"][j]["Id"].ToString());
					var material = Materials.Where(x => x.Id == materialId).FirstOrDefault();
					var frequency = Double.Parse(parsedObject["Monsters"][i]["Loot"][j]["Frequency"].ToString());

					frequencySum+=frequency;

					lootTemp.Add((material, frequency));
				}

				if(frequencySum != 1)
				{
					errorLog.Add($"Error: {name} - loot frequency sums up to {frequencySum} instead of 1.");
				}

				var newMonster = new Monster(id, name, health, image, typesTemp, lootTemp);
				Monsters.Add(newMonster);
			}

			if (errorLog.Count>0)
			{
				Logger.Log(errorLog);
			}
		}
		public static void LoadRegions()
		{
			var path = Path.Combine(Environment.CurrentDirectory, @"Data\", "Regions.json");
			var parsedObject = JObject.Parse(File.ReadAllText(path));
			var jArray = (JArray)parsedObject["Regions"];

			// Error check
			var errorLog = new List<string>();

			for (var i = 0; i < jArray.Count; i++)
			{
				var id = int.Parse(parsedObject["Regions"][i]["Id"].ToString());
				var name = parsedObject["Regions"][i]["Name"].ToString();
				var background = parsedObject["Regions"][i]["Background"].ToString();

				var monstersTemp = new List<(Monster, Double)>();
				var monstersArray = (JArray)parsedObject["Regions"][i]["Monsters"];

				// Error check
				double frequencySum = 0;

				for (int j = 0; j < monstersArray.Count; j++)
				{
					var monsterId = int.Parse(parsedObject["Regions"][i]["Monsters"][j]["Id"].ToString());
					var monster = Monsters.Where(x => x.Id == monsterId).FirstOrDefault();

					var frequency = Double.Parse(parsedObject["Regions"][i]["Monsters"][j]["Frequency"].ToString());

                    frequencySum += frequency;

                    monstersTemp.Add((monster, frequency));
				}
				
				if(frequencySum != 1)
				{
					errorLog.Add($"Error: {name} - monster frequency sums up to {frequencySum} instead of 1.");
				}

				var newRegion = new Region(id, name, background, monstersTemp);
				Regions.Add(newRegion);
			}

			if (errorLog.Count>0)
			{
				Logger.Log(errorLog);
			}
		}

		public static void LoadPages()
		{
            // Town
            Pages.Add("Town", new TownPage());
			// Regions
			foreach (var region in Regions)
			{
                Pages.Add(region.Name, new RegionPage(region));
            }
        }
	}
}