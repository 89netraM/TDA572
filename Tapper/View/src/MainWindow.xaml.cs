using System.Linq;
using System.Windows;
using Tapper.View.ViewModel;
using GameModel = Tapper.Model.Game;

namespace Tapper.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private GameModel game;

		public MainWindow()
		{
			InitializeComponent();
			game = new GameModel();
			while (game.BarPatronPositions.Sum(static b => b.Count) < 5)
			{
				game.Tick(Model.PlayerAction.None);
			}
			DataContext = Game.FromModel(game);
		}
	}
}
