using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Items
{
	public partial class Recipe : Item
	{
		public Recipe(int id, string name, Rarity rarity, int value) : base(id,name,rarity,value)
		{
		}
	}
}