using System.Windows.Media;
using PatronModel = Tapper.Model.Patron;

namespace Tapper.View.ViewModel
{
	public class Patron : ViewModelBase
	{
		public static Patron FromModel(PatronModel model) =>
			new Patron(model.Id, model.Position);

		public int Id { get; }

		public Color Color { get; }

		private double position;
		public double Position
		{
			get => position;
			set => ChangeProperty(ref position, value);
		}

		public Patron(int id, double position) =>
			(Id, Color, Position) = (id, Tools.RandomColor(id), position);

		public void UpdateFromModel(PatronModel model) =>
			(Position) = (model.Position);
	}
}
