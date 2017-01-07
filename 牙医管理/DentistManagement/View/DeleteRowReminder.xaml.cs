using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DentistManagement.ViewModel;

namespace DentistManagement.View
{
    /// <summary>
    /// DeleteRowReminder.xaml 的交互逻辑
    /// </summary>
    public partial class DeleteRowReminder : Window
    {
        public DeleteRowReminder()
        {
            InitializeComponent();
            this.DataContext = new DeleteRowReminderViewModel();
            DeleteRowReminderViewModel.DeleteRowReminder = this;
        }
    }
}
