using System.Linq;
using System.Windows;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.Extensions.Combat
{
	public static class CombatController
	{
		public static void HandleUserClickOnEnemy()
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

			if (isNoQuestActive)
			{
				CombatTimerController.StartPoisonTimer();

				var damageBaseAndCritInfo = User.Instance.CurrentHero.CalculateBaseAndCritClickDamage();
				int damageOnHit = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;

				DealDamageToEnemy(damageBaseAndCritInfo.Damage, damageBaseAndCritInfo.DamageType);
				DealDamageToEnemy(damageOnHit, DamageType.OnHit);

				// Invoke Artifacts with the "on-enemy-click" effect (if enemy isn't dead already).
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnEnemyClick();
				}

			}
			else
			{
				AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}

		public static void DealDamageToEnemy(int damage, DamageType damageType = DamageType.Normal)
		{
			// Make sure enemy is still alive before dealing damage.
			if (InterfaceController.CurrentEnemy?.CurrentHealth <= 0)
			{
				return;
			}

			// Trigger on dealing (any) damage artifact event.
			if (damageType != DamageType.Artifact)
			{
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnDealingDamage(ref damage);
				}
			}

			// Trigger damage type specific artifact events.
			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				switch (damageType)
				{
					case DamageType.Normal or DamageType.Critical:
						equippedArtifact.ArtifactFunctionality.OnDealingClickDamage(ref damage, damageType);
						break;

					case DamageType.Poison:
						equippedArtifact.ArtifactFunctionality.OnDealingPoisonDamage(ref damage);
						break;

					case DamageType.Aura:
						equippedArtifact.ArtifactFunctionality.OnDealingAuraDamage(ref damage);
						break;
				}
			}

			InterfaceController.CurrentEnemy.CurrentHealth -= damage;

			// todo: zrobic to lepiej potem
			// Invoke floating text method.
			if (InterfaceController.CurrentEnemy is Monster)
			{
				InterfaceController.CurrentMonsterButton.CreateFloatingTextPathAndStartAnimations(damage, damageType);
			}
			else if (InterfaceController.CurrentEnemy is Boss)
			{
				InterfaceController.CurrentBossPage.CreateFloatingTextPathAndStartAnimations(damage, damageType);
			}

			InterfaceController.CurrentEnemy.HandleEnemyDeathIfDefeated();
		}
	}
}