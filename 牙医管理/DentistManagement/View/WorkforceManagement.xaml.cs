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
using System.Windows.Controls.Primitives;
using System.Globalization;
using DentistManagement.Model;
using DentistManagement.Model.ServiceClass;

namespace DentistManagement.View
{
    /// <summary>
    /// WorkforceManagement.xaml 的交互逻辑
    /// </summary>
    public partial class WorkforceManagement : Window
    {
        private CultureInfo localCultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.ToString());
        //private StackPanel selectedDatePanel;
        private UniformGrid dutyDisplayUniformGrid;

        public WorkforceManagement()
        {
            WindowBehaviorHelper windowBehaviorHelper = new WindowBehaviorHelper(this);
            windowBehaviorHelper.RepairBehavior();
            InitializeComponent();
            this.DataContext = new WorkforceManagementViewModel();
            WorkforceManagementViewModel.WorkforceManagement = this;
            //this.DutyListBox.SelectionChanged += SelectedOnDisplay;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayDuty();
            this.cbMonth.SelectedIndex = 0;
            
        }

        private void SelectedOnDisplay(Object sender, SelectionChangedEventArgs e)
        {
            //string valur = DutyListBox.SelectedValue.ToString();
            //ShiftsInformation ShiftsInformation = DutyListBox.SelectedItem as ShiftsInformation;
        }

        private void DisplayDuty()
        {
            //dutyDisplayUniformGrid = GetCalendarUniformGrid(DutyListBox);
            //for (int i = 0; i < ShiftsInforService.RetrieveShiftsInforList().Count; i++)
            //{
            //    Button mainDateLabel = new Button();
            //    mainDateLabel.FontSize = 15;
            //    mainDateLabel.Width = 80;
            //    mainDateLabel.Height = 30;
            //    mainDateLabel.Content = ShiftsInforService.RetrieveShiftsInforList()[i].WorkTime;
            //    //Compose the final displaying unit.
            //    StackPanel stackPanel = new StackPanel();
            //    stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            //    stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
            //    //stackPanel.Children.Add(hiddenLabel);
            //    stackPanel.Children.Add(mainDateLabel);
            //    DutyListBox.Items.Add(stackPanel);
                
            //}
        }

        public static UniformGrid GetCalendarUniformGrid(DependencyObject uniformGrid)
        {
            UniformGrid tempGrid = uniformGrid as UniformGrid;
            if (tempGrid != null)
            {
                return tempGrid;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(uniformGrid); i++)
            {
                DependencyObject gridReturn =
                    GetCalendarUniformGrid(VisualTreeHelper.GetChild(uniformGrid, i));
                if (gridReturn != null)
                {
                    return gridReturn as UniformGrid;
                }
            }
            return null;
        }

    }
}
