using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Globalization;

namespace DentistManagement
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //DentistManagement.Properties.Resources.Culture = new System.Globalization.CultureInfo("zh-CN");
            CultureInfo ctrDate = new CultureInfo("en-US", false);
            ctrDate.DateTimeFormat.ShortDatePattern = "yyyy.MM.dd";
            System.Threading.Thread.CurrentThread.CurrentCulture = ctrDate;
        }
        
    }
}
