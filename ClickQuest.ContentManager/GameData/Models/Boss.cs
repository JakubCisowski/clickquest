﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class BossLootPattern
	{
		public RewardType BossLootType { get; set; }
		public int BossLootId { get; set; }
		public List<double> Frequencies { get; set; }

		[JsonIgnore]
		public Item Item
		{
			get
			{
				Item item = null;

				switch (BossLootType)
				{
					case RewardType.Material:
						item = GameContent.Materials.FirstOrDefault(x => x.Id == BossLootId);
						break;

					case RewardType.Recipe:
						item = GameContent.Recipes.FirstOrDefault(x => x.Id == BossLootId);
						break;

					case RewardType.Artifact:
						item = GameContent.Artifacts.FirstOrDefault(x => x.Id == BossLootId);
						break;
				}

				return item;
			}
		}

		public BossLootPattern()
		{
			
		}
	}
	
	public class Boss : IIdentifiable
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Health { get; set; }
		public string Image { get; set; }
		public string Description { get; set; }
		public List<BossLootPattern> BossLootPatterns { get; set; }

		public Boss()
		{
			
		}
	}
}