using System.Collections.ObjectModel;
using System.Linq;
using GameModel = Tapper.Model.Game;

namespace Tapper.View.ViewModel
{
	public class Game : ViewModelBase
	{
		public static Game FromModel(GameModel model) =>
			new Game
			{
				BarPatronPositions = new ObservableCollection<ObservableCollection<Patron>>(model.BarPatronPositions.Select(static b => new ObservableCollection<Patron>(b.Select(Patron.FromModel)))),
				BarGlassesPositions = new ObservableCollection<ObservableCollection<Glass>>(model.BarGlassesPositions.Select(static b => new ObservableCollection<Glass>(b.Select(Glass.FromModel)))),
				PlayerBar = model.PlayerBar,
				PlayerBarPosition = model.PlayerBarPosition
			};

		public ObservableCollection<ObservableCollection<Patron>> BarPatronPositions { get; init; } = new ObservableCollection<ObservableCollection<Patron>>();
		public ObservableCollection<ObservableCollection<Glass>> BarGlassesPositions { get; init; } = new ObservableCollection<ObservableCollection<Glass>>();

		private int playerBar = 0;
		public int PlayerBar
		{
			get => playerBar;
			set => ChangeProperty(ref playerBar, value);
		}

		private double playerBarPosition = GameModel.BarEnd;
		public double PlayerBarPosition
		{
			get => playerBarPosition;
			set => ChangeProperty(ref playerBarPosition, value);
		}

		public void UpdateFromModel(GameModel model)
		{
			var nextPatronIds = model.BarPatronPositions.SelectMany(static b => b.Select(static p => p)).ToDictionary(static p => p.Id);
			var currentPatronIds = BarPatronPositions.SelectMany(static b => b.Select(static p => p)).ToDictionary(static p => p.Id);
			for (int b = 0; b < BarPatronPositions.Count; b++)
			{
				var bar = BarPatronPositions[b];
				var mBar = model.BarPatronPositions[b];
				for (int p = 0; p < bar.Count; p++)
				{
					var patron = bar[p];
					if (nextPatronIds.TryGetValue(patron.Id, out var mp))
					{
						patron.UpdateFromModel(mp);
					}
					else
					{
						bar.RemoveAt(p--);
					}
				}
				for (int p = 0; p < mBar.Count; p++)
				{
					var patron = mBar[p];
					if (!currentPatronIds.ContainsKey(patron.Id))
					{
						bar.Add(Patron.FromModel(patron));
					}
				}
			}

			var nextGlassesIds = model.BarGlassesPositions.SelectMany(static b => b.Select(static p => p)).ToDictionary(static p => p.Id);
			var currentGlassesIds = BarGlassesPositions.SelectMany(static b => b.Select(static p => p)).ToDictionary(static p => p.Id);
			for (int b = 0; b < BarGlassesPositions.Count; b++)
			{
				var bar = BarGlassesPositions[b];
				var mBar = model.BarGlassesPositions[b];
				for (int g = 0; g < bar.Count; g++)
				{
					var glass = bar[g];
					if (nextGlassesIds.TryGetValue(glass.Id, out var mg))
					{
						glass.UpdateFromModel(mg);
					}
					else
					{
						bar.RemoveAt(g--);
					}
				}
				for (int g = 0; g < mBar.Count; g++)
				{
					var glass = mBar[g];
					if (!currentGlassesIds.ContainsKey(glass.Id))
					{
						bar.Add(Glass.FromModel(glass));
					}
				}
			}

			PlayerBar = model.PlayerBar;
			PlayerBarPosition = model.PlayerBarPosition;
		}
	}
}
