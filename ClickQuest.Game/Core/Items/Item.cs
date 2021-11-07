using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Items
{
	public abstract class Item : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private int _quantity;

		public int Id { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
		public Rarity Rarity { get; set; }
		public virtual string Description { get; set; }
		
		[JsonIgnore]
		public ToolTip ToolTip => TooltipController.GenerateEquipmentItemTooltip(this);

		public int Quantity
		{
			get
			{
				return _quantity;
			}
			set
			{
				_quantity = value;

				// If we set quantity to 0 or lower, then remove it from user's equipment
				if (_quantity <= 0)
				{
					User.Instance.CurrentHero?.Recipes.Remove(this as Recipe);
					User.Instance.CurrentHero?.Materials.Remove(this as Material);
					User.Instance.CurrentHero?.Artifacts.Remove(this as Artifact);
				}
			}
		}

		[JsonIgnore]
		public string RarityString
		{
			get
			{
				return Rarity.ToString();
			}
		}

		public abstract Item CopyItem(int quantity);
		public abstract void AddAchievementProgress(int amount = 1);

		public abstract void AddItem(int amount = 1);
		public abstract void RemoveItem(int amount = 1);
	}
}