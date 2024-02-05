using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL_Calculator.Converters
{
    public class PolygonPointsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Point> obsPoints = value as ObservableCollection<Point>;
            if (obsPoints is not null)
            {
                PointCollection points = new();
                foreach (var point in obsPoints)
                    points.Add(point);
                return points;
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
