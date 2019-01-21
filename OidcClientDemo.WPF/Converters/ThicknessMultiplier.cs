using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace OidcClientDemo.WPF.Converters
{
    public class ThicknessMultiplier : DependencyObject ,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Thickness t) || targetType != typeof(Thickness) || Multiplier == null)
                return null;

            var result = new Thickness(
                t.Left * Multiplier.Left,
                t.Top * Multiplier.Top,
                t.Right * Multiplier.Right,
                t.Bottom * Multiplier.Bottom
            );
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        public Thickness Multiplier
        {
            get { return (Thickness)GetValue(MultiplierProperty); }
            set { SetValue(MultiplierProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Multiplier.  This enables animation styling binding etc...
        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(Thickness), typeof(ThicknessMultiplier), new PropertyMetadata(new Thickness(1)));
    }
}
