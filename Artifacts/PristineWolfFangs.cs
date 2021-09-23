﻿using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Increases your Critical Click Chance by 10%, and your Critical Click Damage by 15%.
	public class PristineWolfFangs : ArtifactFunctionality
	{
		private const double CritChanceIncrease = 0.10;
		private const double CritDamageIncrease = 0.15;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.CritChance += CritChanceIncrease;
			User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.CritChance -= CritChanceIncrease;
			User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
		}

		public PristineWolfFangs()
		{
			Name = "Pristine Wolf Fangs";
		}
	}
}