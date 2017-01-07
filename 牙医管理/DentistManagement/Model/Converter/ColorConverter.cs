using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DentistManagement.Model.Converter
{
    class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "Black";
            }

            if (value.ToString() == "六" || value.ToString() == "日")
            {
                return "Red";
            }
            return "Black";
        }
        public object ConvertBack(object value, Type typeTarget, object param, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
