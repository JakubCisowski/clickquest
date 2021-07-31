﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ClickQuest.Adventures;
using ClickQuest.Data;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Heroes
{
	public class Hero : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private const int MAX_LEVEL = 100;
		private int _experience;

		private readonly Random _rng;

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[NotMapped]
		public int ExperienceToNextLvl { get; set; }

		[NotMapped]
		public int ExperienceToNextLvlTotal { get; set; }

		[NotMapped]
		public int ExperienceProgress { get; set; }

		[NotMapped]
		public DateTime SessionStartDate { get; set; }

		public int ClickDamagePerLevel { get; set; }
		public double CritChancePerLevel { get; set; }
		public int PoisonDamagePerLevel { get; set; }
		public string Name { get; set; }
		public HeroRace HeroRace { get; set; }
		public HeroClass HeroClass { get; set; }
		public int ClickDamage { get; set; }
		public double CritChance { get; set; }
		public int PoisonDamage { get; set; }
		public int Level { get; set; }
		public List<Material> Materials { get; set; }
		public List<Recipe> Recipes { get; set; }
		public List<Artifact> Artifacts { get; set; }
		public List<Quest> Quests { get; set; }
		public Blessing Blessing { get; set; }
		public Specialization Specialization { get; set; }
		public TimeSpan TimePlayed { get; set; }
		public double AuraDamage { get; set; }
		public double AuraAttackSpeed { get; set; }

		public int Experience
		{
			get
			{
				return _experience;
			}
			set
			{
				// Dirty code! Only when ExperienceToNextLvl is 0, we can be sure that we are loading Entity AND not killing any monster/boss.
				if (ExperienceToNextLvl != 0)
				{
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.ExperienceGained, value - _experience);
				}

				_experience = value;
				Heroes.Experience.CheckIfLeveledUpAndGrantBonuses(this);
				ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(this);
				ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
				ExperienceProgress = Heroes.Experience.CalculateXpProgress(this);
			}
		}

		public string ThisHeroClass
		{
			get
			{
				return HeroClass.ToString();
			}
		}

		public string ThisHeroRace
		{
			get
			{
				return HeroRace.ToString();
			}
		}

		public string CritChanceText
		{
			get
			{
				string critChanceText = string.Format("{0:P1}", CritChance > 1 ? 1 : CritChance);
				return critChanceText[critChanceText.Length - 2] == '0' ? critChanceText.Remove(critChanceText.Length - 3, 2) : critChanceText;
			}
		}

		public int LevelDamageBonus
		{
			get
			{
				return ClickDamagePerLevel * Level;
			}
		}

		public int LevelDamageBonusTotal
		{
			get
			{
				return ClickDamagePerLevel * Level + 2;
			}
		}

		public double LevelCritBonus
		{
			get
			{
				return CritChancePerLevel * Level * 100;
			}
		}

		public double LevelCritBonusTotal
		{
			get
			{
				return CritChancePerLevel * Level * 100 + 25;
			}
		}

		public int LevelPoisonBonus
		{
			get
			{
				return PoisonDamagePerLevel * Level;
			}
		}

		public int LevelPoisonBonusTotal
		{
			get
			{
				return PoisonDamagePerLevel * Level + 1;
			}
		}

		public string AuraDamageText
		{
			get
			{
				string auraDamageText = string.Format("{0:P1}", AuraDamage);
				return auraDamageText[auraDamageText.Length - 2] == '0' ? auraDamageText.Remove(auraDamageText.Length - 3, 2) : auraDamageText;
			}
		}

		public string AuraDps
		{
			get
			{
				string auraDps = string.Format("{0:P1}", Math.Round(AuraDamage * AuraAttackSpeed, 1));
				return auraDps[auraDps.Length - 2] == '0' ? auraDps.Remove(auraDps.Length - 3, 2) : auraDps;
			}
		}

		public double LevelAuraBonus
		{
			get
			{
				// AuraAttackSpeed per level * Level
				return 1 * Level;
			}
		}

		public double LevelAuraBonusTotal
		{
			get
			{
				// AuraAttackSpeed per level * Level + AuraBaseAttackSpeed
				return 1 * Level + 1;
			}
		}

		public Hero(HeroClass heroClass, HeroRace heroRace, string heroName)
		{
			Materials = new List<Material>();
			Recipes = new List<Recipe>();
			Artifacts = new List<Artifact>();
			Quests = new List<Quest>();

			Specialization = new Specialization();

			_rng = new Random();

			HeroClass = heroClass;
			HeroRace = heroRace;
			Experience = 0;
			Level = 0;
			Name = heroName;
			ClickDamagePerLevel = 1;
			AuraDamage = 0.1;
			AuraAttackSpeed = 1;

			SetClassSpecificValues();
			RefreshHeroExperience();
		}

		public Hero()
		{
			RefreshHeroExperience();

			_rng = new Random();
		}

		public void RefreshHeroExperience()
		{
			// Updates hero experience to make sure panels are updated at startup.
			ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(this);
			ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
			ExperienceProgress = Heroes.Experience.CalculateXpProgress(this);
		}

		public void UpdateTimePlayed()
		{
			if (SessionStartDate != default)
			{
				TimePlayed += DateTime.Now - SessionStartDate;
				SessionStartDate = default;
			}
		}

		public void GrantLevelUpBonuses()
		{
			if (Level < MAX_LEVEL)
			{
				// Class specific bonuses and hero stats panel update.
				switch (HeroClass)
				{
					case HeroClass.Slayer:
						ClickDamage += ClickDamagePerLevel;
						CritChance += CritChancePerLevel;

						break;

					case HeroClass.Venom:
						ClickDamage += ClickDamagePerLevel;
						PoisonDamage += PoisonDamagePerLevel;

						break;
				}
			}
		}

		public void SetClassSpecificValues()
		{
			switch (HeroClass)
			{
				case HeroClass.Slayer:
					ClickDamage = 2;
					CritChance = 0.25;
					CritChancePerLevel = 0.004;
					PoisonDamage = 0;
					PoisonDamagePerLevel = 0;
					break;

				case HeroClass.Venom:
					ClickDamage = 2;
					CritChance = 0;
					CritChancePerLevel = 0;
					PoisonDamage = 1;
					PoisonDamagePerLevel = 2;
					break;
			}
		}

		public void ResumeBlessing()
		{
			// Resume blessings (if there are any left) - used when user selects a hero.
			Blessing?.EnableBuff();
		}

		public void PauseBlessing()
		{
			// Pause current blessings - used when user exits the game or returns to main menu page.
			Blessing?.DisableBuff();
		}

		public void RemoveBlessing()
		{
			User.Instance.CurrentHero.Blessing?.DisableBuff();
			User.Instance.CurrentHero.Blessing = null;
		}

		public void ResumeQuest()
		{
			Quests.FirstOrDefault(x => x.EndDate != default)?.StartQuest();
		}

		public void LoadQuests()
		{
			// Clone hero's quests using those from Database - rewards are not stored in Entity.
			for (int i = 0; i < Quests.Count; i++)
			{
				var heroQuest = Quests[i];
				var databaseQuest = GameData.Quests.FirstOrDefault(x => x.Id == heroQuest.Id);

				Quests[i] = databaseQuest.CopyQuest();

				// CopyQuest sets EndDate from GameData, so we need to get EndDate from Entity instead.
				Quests[i].EndDate = heroQuest.EndDate;
			}
		}

		public (int Damage, bool IsCritical) CalculateClickDamage()
		{
			int damage = ClickDamage;
			bool isCritical = false;

			// Calculate crit (max 100%).
			double randomizedValue = _rng.Next(1, 101) / 100d;
			if (randomizedValue <= CritChance)
			{
				damage *= 2;
				isCritical = true;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.CritsAmount, 1);
			}

			damage += Specialization.SpecializationBuffs[SpecializationType.Clicking];

			return (damage, isCritical);
		}
	}
}