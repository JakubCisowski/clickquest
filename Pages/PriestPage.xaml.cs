using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Items;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class PriestPage : Page
	{
		public PriestPage()
		{
			InitializeComponent();

			UpdatePriest();
		}

		public void UpdatePriest()
		{
			ItemsListViewBuy.ItemsSource = Database.PriestOffer;
			ItemsListViewBuy.Items.Refresh();
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var blessingBlueprint = b.CommandParameter as Blessing;
			// Remove gold, start blessing

			if (User.Instance.Gold >= blessingBlueprint.Value)
			{
				User.Instance.Gold -= blessingBlueprint.Value;
				// Create a new Blessing.
				var blessing = new Blessing(blessingBlueprint);
				User.Instance.Blessings.Add(blessing);
				blessing.ChangeBuffStatus(true);
			}

			UpdatePriest();
		}
	}
}