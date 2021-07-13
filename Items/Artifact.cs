using ClickQuest.Player;

namespace ClickQuest.Items
{
	public partial class Artifact : Item
	{
		public Artifact() : base()
		{
			
		}

		public override Artifact CopyItem()
		{
			Artifact copy = new Artifact();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = 1;

			return copy;
		}

		public override void AddAchievementProgress()
		{
			NumericAchievementType achievementType = 0;
			// Increase achievement amount.
			switch(this.Rarity)
			{
				case Rarity.General:
					achievementType=NumericAchievementType.GeneralArtifactsGained;
					break;
				case Rarity.Fine:
					achievementType=NumericAchievementType.FineArtifactsGained;
					break;
				case Rarity.Superior:
					achievementType=NumericAchievementType.SuperiorArtifactsGained;
					break;
				case Rarity.Exceptional:
					achievementType=NumericAchievementType.ExceptionalArtifactsGained;
					break;
				case Rarity.Mythic:
					achievementType=NumericAchievementType.MythicArtifactsGained;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkArtifactsGained;
					break;
			}
			User.Instance.Achievements.IncreaseAchievementValue(achievementType, 1);
		}
	}
}