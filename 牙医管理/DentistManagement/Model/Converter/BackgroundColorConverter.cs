using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DentistManagement.Model.ServiceClass;

namespace DentistManagement.Model.Converter
{
    class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, System.Globalization.CultureInfo culture)
        {
            if (value==null)
            {
                return "White";
            }
            else
            {
               
                for (int i = 0; i < ShiftsInforService.RetrieveShiftsInforList().Count; i++)
                {
                    if (value.ToString()==ShiftsInforService.RetrieveShiftsInforList()[i].WorkTime)
                    {
                        return ShiftsInforService.RetrieveShiftsInforList()[i].Corlor;
                    }
                }
            }
            return "White";
        }
        public object ConvertBack(object value, Type typeTarget, object param, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}