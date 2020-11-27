using ClickQuest.Controls;
using ClickQuest.Places;
using ClickQuest.Data;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class RegionPage : Page
	{
		private Region _region;

		public RegionPage(Region currentRegion)
		{
			InitializeComponent();

			_region = currentRegion;
			this.DataContext = _region;

			CreateMonsterButton();
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}

		public void CreateMonsterButton()
		{
			var button = new MonsterButton(_region, this);
			this.RegionPanel.Children.Insert(1, button);
		}
	}
}