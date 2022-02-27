﻿using System;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

public class ExplosiveArrows : ArtifactFunctionality, IConsumable
{
	private const int DamageDealtPerTick = 50;
	private const int TicksIntervalInSeconds = 1;

	private readonly DispatcherTimer _timer;

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

	public override void OnKill()
	{
		_timer.Stop();
	}

	public void Consume(Artifact ammunitionArtifact)
	{
		if (!_timer.IsEnabled)
		{
			ammunitionArtifact.RemoveItem(1);
			_timer.Start();
		}

		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnConsumed(ammunitionArtifact);
		}
	}

	public ExplosiveArrows()
	{
		Name = "Explosive Arrows";
		ArtifactSlotsRequired = 0;
		
		_timer = new DispatcherTimer()
		{
			Interval = new TimeSpan(0, 0, 0, TicksIntervalInSeconds)
		};
		_timer.Tick += BurningTimer_Tick;
	}
	
	private void BurningTimer_Tick(object source, EventArgs e)
	{
		CombatHelper.DealDamageToCurrentEnemy(DamageDealtPerTick, DamageType.Magic);
	}
}