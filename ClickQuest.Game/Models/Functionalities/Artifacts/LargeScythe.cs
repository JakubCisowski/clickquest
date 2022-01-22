﻿using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// All non-artifact damage dealt by you is increased by 15% against Monsters (excluding Bosses).
public class LargeScythe : ArtifactFunctionality
{
	private const double DamageIncreasePercent = 0.15;

	public override void OnDealingDamage(ref int baseDamage)
	{
		if (InterfaceHelper.CurrentEnemy is Monster)
		{
			var bonusDamage = (int)(DamageIncreasePercent * baseDamage);

			baseDamage += bonusDamage;
		}
	}

	public LargeScythe()
	{
		Name = "Large Scythe";
	}
}