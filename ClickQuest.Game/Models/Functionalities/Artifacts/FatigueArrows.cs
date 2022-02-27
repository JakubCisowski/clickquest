﻿using System;
using System.Linq;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

public class FatigueArrows : ArtifactFunctionality, IConsumable
{
	private const int BaseDamageDealt = 500;
	private const double BonusDamageDealtPerHealthPercentLost = 0.025;
	
	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		var rangedWeaponArtifact = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactType == ArtifactType.RangedWeapon);

		if (rangedWeaponArtifact is null)
		{
			return;
		}

		var ammunitionArtifact = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactFunctionality == this);

		if (ammunitionArtifact is null)
		{
			return;
		}

		Consume(ammunitionArtifact);
	}

	public void Consume(Artifact ammunitionArtifact)
	{
		ammunitionArtifact.RemoveItem(1);

		var damageDealt = BaseDamageDealt;
		var bonusDamageModifier = Math.Min((100 - InterfaceHelper.CurrentEnemy.CurrentHealthProgress) * BonusDamageDealtPerHealthPercentLost, 2.0) + 1.0;

		damageDealt = (int)Math.Ceiling(damageDealt * bonusDamageModifier);
		
		CombatHelper.DealDamageToCurrentEnemy(damageDealt, DamageType.Magic);
		
		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnConsumed(ammunitionArtifact);
		}
	}

	public FatigueArrows()
	{
		Name = "Fatigue Arrows";
		ArtifactSlotsRequired = 0;
	}
}