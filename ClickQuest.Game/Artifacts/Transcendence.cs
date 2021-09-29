﻿using ClickQuest.Game.Enemies;
using ClickQuest.Game.Extensions.CombatManager;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Artifacts
{
	// Requires three artifact slots to equip. While on a Region, you don't deal any damage.
	// Instead, each click you make on a Monster will instantly defeat them.
	public class Transcendence : ArtifactFunctionality
	{
		public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
		{
			if (InterfaceController.CurrentEnemy is Monster)
			{
				clickDamage = 0;
			}
		}

		public override void OnDealingPoisonDamage(ref int poisonDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				poisonDamage = 0;
			}
		}

		public override void OnDealingAuraDamage(ref int auraDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				auraDamage = 0;
			}
		}

		public override void OnEnemyClick()
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				CombatController.DealDamageToEnemy(monster.CurrentHealth, DamageType.Artifact);
			}
		}

		public Transcendence()
		{
			Name = "Transcendence";
			ArtifactSlotsRequired = 3;
		}
	}
}