using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tapper.View.ViewModel
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void ChangeProperty<T>(ref T property, T newValue, [CallerMemberName] string? propertyName = default)
		{
			if (property?.Equals(newValue) != true)
			{
				property = newValue;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
