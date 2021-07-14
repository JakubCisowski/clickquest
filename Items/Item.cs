using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Interfaces;

namespace ClickQuest.Items
{
	public abstract partial class Item : INotifyPropertyChanged, IIdentifiable
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields

		private int _id;
		private string _name;
		private int _value;
		private Rarity _rarity;
		private int _quantity;
		private string _description;

		#endregion Private Fields

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DbKey { get; set; }

		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				OnPropertyChanged();
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				OnPropertyChanged();
			}
		}

		public Rarity Rarity
		{
			get
			{
				return _rarity;
			}
			set
			{
				_rarity = value;
				OnPropertyChanged();
			}
		}

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
					Player.User.Instance.CurrentHero?.Recipes.Remove(this as Recipe);
					Player.User.Instance.CurrentHero?.Materials.Remove(this as Material);
					Player.User.Instance.CurrentHero?.Artifacts.Remove(this as Artifact);
				}

				OnPropertyChanged();
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
				OnPropertyChanged();
			}
		}

		public string RarityString
		{
			get
			{
				return Rarity.ToString();
			}
		}

		#endregion Properties

		public Item()
		{

		}

		public abstract Item CopyItem();
		public abstract void AddAchievementProgress();
	}
}