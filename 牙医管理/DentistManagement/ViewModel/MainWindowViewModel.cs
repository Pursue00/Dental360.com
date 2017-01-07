using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DentistManagement.Model;
using DentistManagement.View;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace DentistManagement.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public static MainWindow MainWindow { get; set; }
        private ICalendar mainCalendar;
        private ICalendar subsidiaryCalendar;

        // Specified year, month and day
        private int year;
        private int month;
        private int day;

        private CultureInfo localCultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.ToString());
        private StackPanel selectedDatePanel;
        private UniformGrid calendarDisplayUniformGrid;

        //Display a date with three labels, and 
        //The first label is used to contain the hidden festival text,
        //the second label is used to contain the Gregorian text,
        //the third label is used to contain the Lunar text.
        private static int hiddenLabelIndex = 0;
        private static int mainLabelIndex = 1;
        private static int subsidiaryLabelIndex = 2;
       
        private string[] WeekDays = new string[]{ 
            DentistManagement.Properties.Resources.ChineseNumberText,
            DentistManagement.Properties.Resources.Monday,
            DentistManagement.Properties.Resources.Tuesday, 
            DentistManagement.Properties.Resources.Wednesday,
            DentistManagement.Properties.Resources.Thursday, 
            DentistManagement.Properties.Resources.Friday,
            DentistManagement.Properties.Resources.Saturday 
        };
        #region 属性
        //左边日历显示几月几日
        private string _dateLeft;
        public string DateLeft
        {
            get
            {
                return this._dateLeft;
            }
            set
            {
                if (this._dateLeft!=value)
                {
                    this._dateLeft = value;
                    OnPropertyChanged("DateLeft");
                }
            }
        }
        //左边日历显示的生肖和阴历
        private string _dateDetailLeft;
        public string DateDetailLeft
        {
            get
            {
                return this._dateDetailLeft;
            }
            set
            {
                if (this._dateDetailLeft != value)
                {
                    this._dateDetailLeft = value;
                    OnPropertyChanged("DateDetailLeft");
                }
            }
        }
        //右边日期的详细显示
        private string _dateRight;
        public string DateRight
        {
            get
            {
                return this._dateRight;
            }
            set
            {
                if (this._dateRight != value)
                {
                    this._dateRight = value;
                    OnPropertyChanged("DateRight");
                }
            }
        }
        //天中的时间
        private string _dayTime;
        public string DayTime
        {
            get
            {
                return this._dayTime;
            }
            set
            {
                if (this._dayTime != value)
                {
                    this._dayTime = value;
                    OnPropertyChanged("DayTime");
                }
            }
        }
        //周中的时间
        private string _weekTime;
        public string WeekTime
        {
            get
            {
                return this._weekTime;
            }
            set
            {
                if (this._weekTime != value)
                {
                    this._weekTime = value;
                    OnPropertyChanged("WeekTime");
                }
            }
        }
        #endregion
        #region 命令以及命令对应的方法
        #region 主窗体
        //关闭
        public DelegateCommand CloseCommand { get; set; }

        public void CloseCommandExecute(object obj)
        {
            try
            {
                MainWindow.Close();
                LogHelper.WriteLog(typeof(MainWindowViewModel), "窗体关闭成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(MainWindowViewModel), "窗体关闭失败" + ex.Message);
                LogHelper.WriteLog(typeof(MainWindowViewModel), "窗体关闭失败详细信息" + ex.StackTrace);
            }

        }

        //最小化
        public DelegateCommand MinCommand { get; set; }

        public void MinCommandExecute(object obj)
        {
            try
            {
                MainWindow.WindowState = System.Windows.WindowState.Minimized;
                LogHelper.WriteLog(typeof(MainWindowViewModel), "窗体最小化");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(MainWindowViewModel), "窗体最小化失败" + ex.Message);
                LogHelper.WriteLog(typeof(MainWindowViewModel), "窗体最小化失败详细信息" + ex.StackTrace);
            }
        }

        #endregion
        #region 其它
        //排班
        public DelegateCommand ShiftsCommand { get; set; }

        public void ShiftsCommandExecute(object obj)
        {
            WorkforceManagement WorkforceManagement = new WorkforceManagement();
            WorkforceManagement.ShowInTaskbar = false;
            WorkforceManagement.Owner = MainWindow;
            WorkforceManagement.ShowDialog();
        }
        //前一个时间
        public DelegateCommand PreviousCommand { get; set; }

        public void PreviousCommandExecute(object obj)
        {
            try
            {
                if (month == 1 && year == 1902)
                {
                    return;
                }

                month -= 1;
                if (month == 0)
                {
                    month = 12;
                    year--;
                }
                UpdateMonth();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        //当天
        public DelegateCommand CurrentCommand { get; set; }

        public void CurrentCommandExecute(object obj)
        {
            try
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;

                UpdateMonth();
                HighlightCurrentDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //后一个时间
        public DelegateCommand NextCommand { get; set; }

        public void NextCommandExecute(object obj)
        {
            try
            {
                if (month == 12 && year == 2100)
                {
                    return;
                }

                month += 1;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
                UpdateMonth();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            this.CloseCommand = new DelegateCommand(CloseCommandExecute, arg => true);
            this.MinCommand = new DelegateCommand(MinCommandExecute, arg => true);
            this.ShiftsCommand = new DelegateCommand(ShiftsCommandExecute, arg => true);
            this.PreviousCommand = new DelegateCommand(PreviousCommandExecute, arg => true);
            this.NextCommand = new DelegateCommand(NextCommandExecute, arg => true);
            this.CurrentCommand = new DelegateCommand(CurrentCommandExecute, arg => true);
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            mainCalendar = new StandardGregorianCalendar();

            //Detect whether the application is running in a zh-CN locale
            subsidiaryCalendar = localCultureInfo.ToString() == "zh-CN" ? new ChineseLunarCalendar() : null;

            //WeekdayLabelsConfigure();
            //TimeSwitchButtonsConfigure();
            //DisplayCalendar(year, month);
            //this.DateLeft = DateTime.Now.ToShortDateString();
            //HighlightCurrentDate();
        }

        public void DisplayCalendar(int year, int month)
        {
            int dayNum = DateTime.DaysInMonth(year, month);
            calendarDisplayUniformGrid = GetCalendarUniformGrid(MainWindow.CalendarListBox);

            DateTime dateTime = new DateTime(year, month, 1);
            calendarDisplayUniformGrid.FirstColumn = (int)(dateTime.DayOfWeek);
            List<DateEntry> mainDateList = mainCalendar.GetDaysOfMonth(year, month);
            List<DateEntry> subsidiaryDateList = null;
            if (subsidiaryCalendar != null)
            {
                subsidiaryDateList = subsidiaryCalendar.GetDaysOfMonth(year, month);
            }

            for (int i = 0; i < dayNum; i++)
            {
                Label mainDateLabel = new Label();
                mainDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                mainDateLabel.VerticalAlignment = VerticalAlignment.Center;
                mainDateLabel.Background = Brushes.Black;
                mainDateLabel.Padding = new Thickness(0, 0, 0, 0);
                mainDateLabel.Margin = new Thickness(0, 0, 0, 0);

                //This label is used to hold the holiday string.
                Label hiddenLabel = new Label();
                hiddenLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                hiddenLabel.VerticalAlignment = VerticalAlignment.Stretch;
                hiddenLabel.Visibility = Visibility.Collapsed;

                //If the application is not running in zh-CN env, 
                //it can display the date number bigger.
                mainDateLabel.FontSize = (localCultureInfo.ToString() == "zh-CN") ? 20 : 25;

                //Weekend should be dispaly in red color.
                if (IsWeekEndOrFestival(ref dateTime, mainDateList, i))
                {
                    mainDateLabel.Foreground = Brushes.Red;
                    if (mainDateList[i].IsFestival)
                    {
                        hiddenLabel.Content = mainDateList[i].Text;
                    }
                }
                else
                {
                    hiddenLabel.Content = "";
                    mainDateLabel.Foreground = Brushes.White;
                }

                mainDateLabel.Content = mainDateList[i].DateOfMonth.ToString(NumberFormatInfo.CurrentInfo);

                //If the application is running in a non zh-CN locale, display no lunar calendar.
                Label subsidiaryDateLabel = null;
                if (subsidiaryDateList != null)
                {
                    subsidiaryDateLabel = new Label();
                    subsidiaryDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    subsidiaryDateLabel.VerticalAlignment = VerticalAlignment.Center;
                    subsidiaryDateLabel.Background = Brushes.Black;
                    subsidiaryDateLabel.Padding = new Thickness(0, 0, 0, 0);
                    subsidiaryDateLabel.FontSize = 13;

                    //Control the festival date to be red.
                    subsidiaryDateLabel.Foreground = subsidiaryDateList[i].IsFestival ? Brushes.Red : Brushes.White;
                    subsidiaryDateLabel.Content = subsidiaryDateList[i].Text;
                }

                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;

                stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                if (subsidiaryDateLabel != null)
                {
                    stackPanel.Children.Add(subsidiaryDateLabel);
                }
                MainWindow.CalendarListBox.Items.Add(stackPanel);

                //Display the current day in another color
                if (dateTime.Date == DateTime.Now.Date)
                {
                    mainDateLabel.Foreground = Brushes.DarkOrange;
                    if (subsidiaryDateLabel != null)
                    {
                        subsidiaryDateLabel.Foreground = Brushes.DarkOrange;
                    }
                }
                dateTime = dateTime.AddDays(1);
            }

        }

        private static bool IsWeekEndOrFestival(ref DateTime dateTime, List<DateEntry> mainDateList, int i)
        {
            return ((int)dateTime.DayOfWeek == 6) || ((int)dateTime.DayOfWeek == 0) || mainDateList[i].IsFestival;
        }

        private void HighlightCurrentDate()
        {
            UIElementCollection dayCollection = calendarDisplayUniformGrid.Children;
            ListBoxItem today;
            int index = DateTime.Now.Day - 1;
            today = (ListBoxItem)(dayCollection[index]);
            today.Foreground = Brushes.Blue;
        }

        private UniformGrid GetCalendarUniformGrid(DependencyObject uniformGrid)
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

        //Configure the color of the weekday label.
        private  void WeekdayLabelsConfigure()
        {
            UIElementCollection labelCollection =MainWindow.stackPanel1.Children;
            Label tempLabel;

            for (int i = 0; i < 7; i++)
            {
                tempLabel = (Label)labelCollection[i];
                tempLabel.Foreground = (i == 0 || i == 6) ? Brushes.Red : Brushes.White;
                tempLabel.Content = WeekDays[i];
            }
        }

        //private void TimeSwitchButtonsConfigure()
        //{
        //    PreviousYearButton.Click += PreviousYearOnClick;
        //    NextYearButton.Click += NextYearOnClick;
        //    PreviousMonthButton.Click += PreviousMonthOnClick;
        //    NextMonthButton.Click += NextMonthOnClick;
        //    CurrentMonthButton.Click += CurrentMonthOnClick;
        //}
        private void UpdateMonth()
        {
            MainWindow.CalendarListBox.BeginInit();
            MainWindow.CalendarListBox.Items.Clear();
            DisplayCalendar(year, month);
            MainWindow.CalendarListBox.EndInit();
            this.DateLeft = (new DateTime(year, month, 1)).ToString("Y", localCultureInfo);
            MainWindow.CalendarListBox.SelectedItem = null;

            //Check the calendar range and disable corresponding buttons
            //CheckRange();
        }

        private void PreviousMonthOnClick(Object sender, RoutedEventArgs e)
        {
            if (month == 1 && year == 1902)
            {
                return;
            }

            month -= 1;
            if (month == 0)
            {
                month = 12;
                year--;
            }
            UpdateMonth();
        }

        private void NextMonthOnClick(Object sender, RoutedEventArgs e)
        {
            if (month == 12 && year == 2100)
            {
                return;
            }

            month += 1;
            if (month > 12)
            {
                month = 1;
                year++;
            }
            UpdateMonth();
        }

        private void CurrentMonthOnClick(Object sender, RoutedEventArgs e)
        {
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;

            UpdateMonth();
            HighlightCurrentDate();
        }

        private void SelectedDateOnDisplay(Object sender, SelectionChangedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)MainWindow.CalendarListBox.SelectedItem;
            int selectedDay = MainWindow.CalendarListBox.SelectedIndex + 1;

            if (selectedDatePanel != null)
            {
                selectedDatePanel.Background = Brushes.Black;
                ((Label)selectedDatePanel.Children[mainLabelIndex]).Background = Brushes.Black;

                //Detect whether the application is running in a zh-CN locale
                if (localCultureInfo.ToString() == "zh-CN")
                {
                    ((Label)selectedDatePanel.Children[subsidiaryLabelIndex]).Background = Brushes.Black;
                }
            }

            if (stackPanel != null)
            {
                ((StackPanel)stackPanel).Background = Brushes.DarkBlue;
                ((Label)stackPanel.Children[mainLabelIndex]).Background = Brushes.DarkBlue;

                //If the application is not running in zh-CN env,
                //it has no second element.
                if (localCultureInfo.ToString() == "zh-CN")
                {
                    ((Label)stackPanel.Children[subsidiaryLabelIndex]).Background = Brushes.DarkBlue;
                }
            }
            selectedDatePanel = stackPanel;

            if (stackPanel != null)
            {
                this.day = selectedDay;
                DateTime dateTimeDisplay = new DateTime(year, month, day);
                string festivalString = null;

                if (string.IsNullOrEmpty((string)((Label)stackPanel.Children[hiddenLabelIndex]).Content) == false)
                {
                    festivalString = (string)((Label)stackPanel.Children[hiddenLabelIndex]).Content;
                }



                this.DateLeft = dateTimeDisplay.ToShortDateString() + " " + festivalString;
            }
            else
            {
                this.DateLeft = (new DateTime(year, month, 1)).ToShortDateString();
            }

        }

      
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
