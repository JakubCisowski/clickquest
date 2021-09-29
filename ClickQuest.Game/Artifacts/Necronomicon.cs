﻿using System;
using System.Windows.Threading;
using ClickQuest.Game.Items;
using ClickQuest.Game.Player;

namespace ClickQuest.Game.Artifacts
{
	// After killing a monster, your poison damage is increased by 10 for 5 seconds. This effect does not stack.
	public class Necronomicon : ArtifactFunctionality
	{
		private const int PoisonDamageIncrease = 10;
		private const int StackDuration = 5;

		private readonly DispatcherTimer _timer;

		public override void OnKill()
		{
			if (_timer.IsEnabled)
			{
				PoisonDamageIncreaseTimer_Tick(null, null);
			}

			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
			_timer.Start();
		}

		public Necronomicon()
		{
			Name = "Necronomicon";
			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, StackDuration)
			};
			_timer.Tick += PoisonDamageIncreaseTimer_Tick;
		}

		private void PoisonDamageIncreaseTimer_Tick(object source, EventArgs e)
		{
			_timer.Stop();
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		}
	}
}