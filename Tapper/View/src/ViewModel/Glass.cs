using System.Windows.Media;
using GlassModel = Tapper.Model.Glass;

namespace Tapper.View.ViewModel
{
	public class Glass : ViewModelBase
	{
		public static Glass FromModel(GlassModel model) =>
			new Glass(model.Id, model.Position);

		public int Id { get; }

		public Color Color { get; }

		private double position;
		public double Position
		{
			get => position;
			set => ChangeProperty(ref position, value);
		}

		public Glass(int id, double position) =>
			(Id, Color, Position) = (id, Tools.RandomColor(id), position);

		public void UpdateFromModel(GlassModel model) =>
			(Position) = (model.Position);
	}
}
