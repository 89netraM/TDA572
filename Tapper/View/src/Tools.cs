using System;
using System.Globalization;
using System.Windows.Data;
using MColor = System.Windows.Media.Color;

namespace Tapper.View
{
	static class Tools
	{
		public static MColor RandomColor(int? seed = null)
		{
			var r = seed is int s ? new Random(s) : new Random();
			var buffer = new byte[3];
			r.NextBytes(buffer);
			return MColor.FromRgb(buffer[0], buffer[1], buffer[2]);
		}
	}

	public class MultiplyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double v && double.TryParse(parameter as string, NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
			{
				return v * p;
			}
			else
			{
				throw new ArgumentException($"{nameof(value)} or {nameof(parameter)} wasn't a double");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double v && parameter is double p)
			{
				return v / p;
			}
			else
			{
				throw new ArgumentException($"{nameof(value)} or {nameof(parameter)} wasn't a double");
			}
		}
	}
}
