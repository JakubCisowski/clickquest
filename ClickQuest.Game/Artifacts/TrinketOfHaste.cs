﻿using ClickQuest.Game.Adventures;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Artifacts
{
	// Quests take 25% less time to complete.
	public class TrinketOfHaste : ArtifactFunctionality
	{
		private const double QuestTimeReduced = 0.25;

		public override void OnQuestStarted(Quest quest)
		{
			quest.Duration -= (int) (quest.Duration * QuestTimeReduced);
		}

		public TrinketOfHaste()
		{
			Name = "Trinket of Haste";
		}
	}
}