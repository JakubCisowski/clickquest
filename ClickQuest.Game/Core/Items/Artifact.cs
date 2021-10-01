using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Interface;

namespace ClickQuest.Game.Core.Items
{
	public class Artifact : Item, IMeltable
	{
		[JsonIgnore]
		public ArtifactFunctionality ArtifactFunctionality { get; set; }

		public int BaseIngotBonus
		{
			get
			{
				return 100;
			}
		}

		public override Artifact CopyItem(int quantity)
		{
			var copy = new Artifact();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = quantity;
			copy.ArtifactFunctionality = ArtifactFunctionality;

			return copy;
		}

		public override void AddAchievementProgress(int amount = 1)
		{
			NumericAchievementType achievementType = 0;
			// Increase achievement amount.
			switch (Rarity)
			{
				case Rarity.General:
					achievementType = NumericAchievementType.GeneralArtifactsGained;
					break;
				case Rarity.Fine:
					achievementType = NumericAchievementType.FineArtifactsGained;
					break;
				case Rarity.Superior:
					achievementType = NumericAchievementType.SuperiorArtifactsGained;
					break;
				case Rarity.Exceptional:
					achievementType = NumericAchievementType.ExceptionalArtifactsGained;
					break;
				case Rarity.Mythic:
					achievementType = NumericAchievementType.MythicArtifactsGained;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkArtifactsGained;
					break;
			}

			User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}

		public override void AddItem(int amount = 1)
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Artifacts);

			AddAchievementProgress();
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Materials);
		}
	}
}