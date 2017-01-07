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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DentistManagement.Model;
using DentistManagement.ViewModel;
using System.Globalization;
using System.Windows.Controls.Primitives;
using DentistManagement.View;

namespace DentistManagement
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICalendar mainCalendar;
        private ICalendar subsidiaryCalendar;

        // Specified year, month and day
        private int year;
        private int month;
        private int day;
        private bool isByMonth = false;
        private bool isByDay = true;
        private bool isByWeek = false;
        private CultureInfo localCultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.ToString());
        private StackPanel selectedDatePanel;
        private UniformGrid calendarDisplayUniformGrid;
        private static int hiddenLabelIndex = 0;
        private static int mainLabelIndex = 1;
        private static int subsidiaryLabelIndex = 2;

        private string[] WeekDays = new string[]{ 
            DentistManagement.Properties.Resources.Sunday,
            DentistManagement.Properties.Resources.Monday,
            DentistManagement.Properties.Resources.Tuesday, 
            DentistManagement.Properties.Resources.Wednesday,
            DentistManagement.Properties.Resources.Thursday, 
            DentistManagement.Properties.Resources.Friday,
            DentistManagement.Properties.Resources.Saturday 
        };
        private string[] Hours = new string[] { "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22" };
        private string[] Minutes = new string[] { "00", "30"};
        private string Week(DateTime currentDay)
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(currentDay.DayOfWeek)];

            return week;
        }

        /// <summary>
        /// 判断是哪一天
        /// </summary>
        /// <param name="currentDay"></param>
        /// <returns></returns>
        private string Day(DateTime currentDay)
        {
            DateTime time1 = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            DateTime time2 = Convert.ToDateTime(currentDay.ToShortDateString());
            TimeSpan ts = time2.Subtract(time1);
            int day = ts.Days;
            string days = string.Empty;
            switch (day)
            {
                case 0:
                    {
                        days = "(今天)";
                    }
                    break;
                case 1:
                    {
                        days = "(明天)";
                    }
                    break;
                case 2:
                    {
                        days = "(后天)";
                    }
                    break;
                case -1:
                    {
                        days = "(昨天)";
                    }
                    break;
                case -2:
                    {
                        days = "(前天)";
                    }
                    break;
                default:
                    {
                        if (day>0)
                        {
                            days = "(" + day + "天后)";
                        }
                        else
                        {
                            days = "(" + day.ToString().Replace("-","") + "天前)";
                        }
                        
                    }
                    break;
            }
            return days;
        }

        public MainWindow()
        {
            InitializeComponent();
            WindowBehaviorHelper windowBehaviorHelper = new WindowBehaviorHelper(this);
            windowBehaviorHelper.RepairBehavior();
            this.DataContext = new MainWindowViewModel();
            MainWindowViewModel.MainWindow = this;
            this.Loaded += WindowOnLoad;
            this.MouseLeftButtonDown += WindowOnMove;
            this.CalendarListBox.SelectionChanged += SelectedDateOnDisplay;
            this.CalendarListBox1.SelectionChanged += SelectedDateOnDisplay;
            this.CalendarListBoxByMonth.SelectionChanged += SelectedDateOnDisplayRight;
            this.lbDate.Foreground = Brushes.Black;
            this.btnDay.Focus();
            this.CalendarListBox.Foreground = Brushes.Black;
            this.CalendarListBox.Padding = new Thickness(0, 0, 0, 0);
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            mainCalendar = new StandardGregorianCalendar();
            //Detect whether the application is running in a zh-CN locale
            subsidiaryCalendar = localCultureInfo.ToString() == "zh-CN" ? new ChineseLunarCalendar() : null;
            WeekdayLabelsConfigure();
            TimeSwitchButtonsConfigure();
        }

        /// <summary>
        /// 按月显示
        /// </summary>
        private void DisplayCalendarByMonth()
        {
            DateTime dateTime = new DateTime(year, month, 1);
            CalendarListBox.Visibility = System.Windows.Visibility.Collapsed;
            stackPanel1.Visibility = System.Windows.Visibility.Hidden;
            CalendarListBox1.Visibility = System.Windows.Visibility.Visible;
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBox1);
            //calendarDisplayUniformGrid.FirstColumn = (int)(dateTime.DayOfWeek);
            for (int i = 1; i < 13; i++)
            {
                Label mainDateLabel = new Label();
                mainDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                mainDateLabel.VerticalAlignment = VerticalAlignment.Center;
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
                mainDateLabel.Content = i + "月";
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                CalendarListBox1.Items.Add(stackPanel);
                //Display the current month in another color
                if (dateTime.Year ==DateTime.Now.Year)
                {
                    if (i == month)
                    {
                        mainDateLabel.Foreground = Brushes.DarkOrange;
                    }
                }
                else
                {
                    if (i ==1)
                    {
                        mainDateLabel.Foreground = Brushes.DarkOrange;
                    }
                }
               
            }
        }

        /// <summary>
        /// 点击月的时候按下右边的下一个
        /// </summary>
        private void DisplayCalendarByMonthOnRight()
        {
            DateTime dateTime = new DateTime(year, month, 1);
            CalendarListBox.Visibility = System.Windows.Visibility.Collapsed;
            stackPanel1.Visibility = System.Windows.Visibility.Hidden;
            CalendarListBox1.Visibility = System.Windows.Visibility.Visible;
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBox1);
            //calendarDisplayUniformGrid.FirstColumn = (int)(dateTime.DayOfWeek);
            for (int i = 1; i < 13; i++)
            {
                Label mainDateLabel = new Label();
                mainDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                mainDateLabel.VerticalAlignment = VerticalAlignment.Center;
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
                mainDateLabel.Content = i + "月";
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                CalendarListBox1.Items.Add(stackPanel);
                //Display the current month in another color
                if (dateTime.Year == DateTime.Now.Year)
                {
                    if (i == month)
                    {
                        mainDateLabel.Foreground = Brushes.DarkOrange;
                    }
                }
                else
                {
                    if (i == month)
                    {
                        mainDateLabel.Foreground = Brushes.DarkOrange;
                    }
                }

                //dateTime = dateTime.AddMonths(1);
            }
        }

        /// <summary>
        /// 右边按月显示
        /// </summary>
        private void DisplayCalendarByMonthRight()
        {
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBoxByMonth);
            List<DateEntry> subsidiaryDateList = null;
            if (subsidiaryCalendar != null)
            {
                subsidiaryDateList = subsidiaryCalendar.GetDaysOfMonth(year, month);
            }
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                MonthUserControl mainDateLabel = new MonthUserControl();
                mainDateLabel.FontSize = (localCultureInfo.ToString() == "zh-CN") ? 20 : 25;
                mainDateLabel.lbOne.Content = i;
                mainDateLabel.lbTwo.Content = subsidiaryDateList[i-1].Text;
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                //stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                CalendarListBoxByMonth.Items.Add(stackPanel);
                //Display the current month in another color
                if (year == DateTime.Now.Year)
                {
                    if (month==DateTime.Now.Month)
                    {
                        if (i == day)
                        {
                            mainDateLabel.lbThree.Content = "今";
                            mainDateLabel.lbOne.Foreground = Brushes.DarkOrange;
                            mainDateLabel.lbTwo.Foreground = Brushes.DarkOrange;
                            mainDateLabel.lbThree.Foreground = Brushes.DarkOrange;
                        }
                    }
                   
                }
                else
                {
                    if (month == DateTime.Now.Month)
                    {
                        if (i == day)
                        {
                            mainDateLabel.lbThree.Content = "今";
                            mainDateLabel.lbOne.Foreground = Brushes.DarkOrange;
                            mainDateLabel.lbTwo.Foreground = Brushes.DarkOrange;
                            mainDateLabel.lbThree.Foreground = Brushes.DarkOrange;
                        }
                    }
                }
                //dateTime = dateTime.AddMonths(1);
            }
        }

        /// <summary>
        /// 按天显示
        /// </summary>
        private void DisplayCalendarByDay()
        {
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBoxByDay);
            for (int i = 0; i < 14; i++)
            {
                DayUserControl mainDateLabel = new DayUserControl();
                mainDateLabel.FontSize = (localCultureInfo.ToString() == "zh-CN") ? 20 : 25;
                mainDateLabel.lbLeft.Content = Hours[i].ToString();
                mainDateLabel.lbRight.Content = Hours[i].ToString();
                if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                {
                    mainDateLabel.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                }
                
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                //stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                CalendarListBoxByDay.Items.Add(stackPanel);
                
            }
        }

        /// <summary>
        /// 按周显示
        /// </summary>
        private void DisplayCalendarByWeek()
        {
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBoxByWeek);
            for (int i = 0; i < 14; i++)
            {
                WeekUserControl mainDateLabel = new WeekUserControl();
                mainDateLabel.FontSize = (localCultureInfo.ToString() == "zh-CN") ? 20 : 25;
                mainDateLabel.lbLeft.Content = Hours[i].ToString();
                mainDateLabel.lbRight.Content = Hours[i].ToString();
                string week = Week(DateTime.Now);
                switch (week)
                {
                    case "周日":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbSundayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbSundayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                           
                        }
                        break;
                    case "周一":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbMondayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbMondayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                          
                        }
                        break;
                    case "周二":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbTuesdayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbTuesdayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                           
                        }
                        break;
                    case "周三":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbWednesdayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbWednesdayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                           
                        }
                        break;
                    case "周四":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbThursdayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbThursdayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                          
                        }
                        break;
                    case "周五":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbFridayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbFridayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                           
                        }
                        break;
                    case "周六":
                        {
                            if (Convert.ToInt32(Hours[i]) < 10 || Convert.ToInt32(Hours[i]) >= 20)
                            {
                                mainDateLabel.lbSaturdayUp.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                                mainDateLabel.lbSaturdayDown.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                            }
                           
                        }
                        break;
                }
                
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                //stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                CalendarListBoxByWeek.Items.Add(stackPanel);
       
            }
        }

        /// <summary>
        /// 按天显示
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        public void DisplayCalendar(int year, int month)
        {
            int dayNum = DateTime.DaysInMonth(year, month);
            CalendarListBox1.Visibility = System.Windows.Visibility.Collapsed;
            stackPanel1.Visibility = System.Windows.Visibility.Visible;
            CalendarListBox.Visibility = System.Windows.Visibility.Visible;
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBox);
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
                //mainDateLabel.Background = Brushes.Black;
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
                    mainDateLabel.Foreground = Brushes.Black;
                }
                mainDateLabel.Content = mainDateList[i].DateOfMonth.ToString(NumberFormatInfo.CurrentInfo);
                //mainDateLabel.Background = Brushes.DarkOrange;
                //If the application is running in a non zh-CN locale, display no lunar calendar.
                Label subsidiaryDateLabel = null;
                if (subsidiaryDateList != null)
                {
                    subsidiaryDateLabel = new Label();
                    subsidiaryDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    subsidiaryDateLabel.VerticalAlignment = VerticalAlignment.Center;
                    //subsidiaryDateLabel.Background = Brushes.Black;
                    subsidiaryDateLabel.Padding = new Thickness(0, 0, 0, 0);
                    subsidiaryDateLabel.FontSize = 13;
                    //Control the festival date to be red.
                    subsidiaryDateLabel.Foreground = subsidiaryDateList[i].IsFestival ? Brushes.Red : Brushes.Black;
                    subsidiaryDateLabel.Content = subsidiaryDateList[i].Text;
                    //subsidiaryDateLabel.Background = Brushes.DarkOrange;
                }
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                //stackPanel.Background = Brushes.DarkOrange;
                if (subsidiaryDateLabel != null)
                {
                    stackPanel.Children.Add(subsidiaryDateLabel);
                }
                CalendarListBox.Items.Add(stackPanel);
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

        /// <summary>
        /// 左边按周显示
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        private void DisplayCalendarByWeekOnLeft(int year, int month)
        {
            int dayNum = DateTime.DaysInMonth(year, month);
            CalendarListBox1.Visibility = System.Windows.Visibility.Collapsed;
            stackPanel1.Visibility = System.Windows.Visibility.Visible;
            CalendarListBox.Visibility = System.Windows.Visibility.Visible;
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBox);
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
                //mainDateLabel.Background = Brushes.Black;
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
                    mainDateLabel.Foreground = Brushes.Black;
                }
                mainDateLabel.Content = mainDateList[i].DateOfMonth.ToString(NumberFormatInfo.CurrentInfo);
                //mainDateLabel.Background = Brushes.DarkOrange;
                //If the application is running in a non zh-CN locale, display no lunar calendar.
                Label subsidiaryDateLabel = null;
                if (subsidiaryDateList != null)
                {
                    subsidiaryDateLabel = new Label();
                    subsidiaryDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    subsidiaryDateLabel.VerticalAlignment = VerticalAlignment.Center;
                    //subsidiaryDateLabel.Background = Brushes.Black;
                    subsidiaryDateLabel.Padding = new Thickness(0, 0, 0, 0);
                    subsidiaryDateLabel.FontSize = 13;
                    //Control the festival date to be red.
                    subsidiaryDateLabel.Foreground = subsidiaryDateList[i].IsFestival ? Brushes.Red : Brushes.Black;
                    subsidiaryDateLabel.Content = subsidiaryDateList[i].Text;
                    //subsidiaryDateLabel.Background = Brushes.DarkOrange;
                }
                //Compose the final displaying unit.
                StackPanel stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                stackPanel.Children.Add(hiddenLabel);
                stackPanel.Children.Add(mainDateLabel);
                if (i==day-1)
                {
                    stackPanel.Background = Brushes.White; 
                }
                if (i == day)
                {
                    stackPanel.Background = Brushes.White;
                }
                if (i == day+1)
                {
                    stackPanel.Background = Brushes.White;
                }
                if (i == day+2)
                {
                    stackPanel.Background = Brushes.White;
                }
                if (i == day+3)
                {
                    stackPanel.Background = Brushes.White;
                }
                if (i == day+4)
                {
                    stackPanel.Background = Brushes.White;
                }
                if (i == day + 5)
                {
                    stackPanel.Background = Brushes.White;
                }
                if (subsidiaryDateLabel != null)
                {
                    stackPanel.Children.Add(subsidiaryDateLabel);
                }
                CalendarListBox.Items.Add(stackPanel);
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

        /// <summary>
        /// 点击右边的天按钮
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        private void DisplayCalendarByDayOnRight(int year, int month)
        {
            int dayNum = DateTime.DaysInMonth(year, month);
            CalendarListBox1.Visibility = System.Windows.Visibility.Collapsed;
            stackPanel1.Visibility = System.Windows.Visibility.Visible;
            CalendarListBox.Visibility = System.Windows.Visibility.Visible;
            calendarDisplayUniformGrid = GetCalendarUniformGrid(CalendarListBox);
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
                //mainDateLabel.Background = Brushes.Black;
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
                    mainDateLabel.Foreground = Brushes.Black;
                }
                mainDateLabel.Content = mainDateList[i].DateOfMonth.ToString(NumberFormatInfo.CurrentInfo);
                //If the application is running in a non zh-CN locale, display no lunar calendar.
                Label subsidiaryDateLabel = null;
                if (subsidiaryDateList != null)
                {
                    subsidiaryDateLabel = new Label();
                    subsidiaryDateLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    subsidiaryDateLabel.VerticalAlignment = VerticalAlignment.Center;
                    //subsidiaryDateLabel.Background = Brushes.Black;
                    subsidiaryDateLabel.Padding = new Thickness(0, 0, 0, 0);
                    subsidiaryDateLabel.FontSize = 13;
                    //Control the festival date to be red.
                    subsidiaryDateLabel.Foreground = subsidiaryDateList[i].IsFestival ? Brushes.Red : Brushes.Black;
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
                CalendarListBox.Items.Add(stackPanel);
                //Display the current day in another color
                if (dateTime.Date.Day ==day)
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

        /// <summary>
        /// 判断是否是节假日
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="mainDateList"></param>
        /// <param name="i"></param>
        /// <returns></returns>
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
        private void WeekdayLabelsConfigure()
        {
            UIElementCollection labelCollection = this.stackPanel1.Children;
            Label tempLabel;
            for (int i = 0; i < 7; i++)
            {
                tempLabel = (Label)labelCollection[i];
                tempLabel.Foreground = (i == 0 || i == 6) ? Brushes.Red : Brushes.Black;
                tempLabel.Content = WeekDays[i];
            }
        }

        private void TimeSwitchButtonsConfigure()
        {
            PreviousMonthButton.Click += PreviousMonthOnClick;
            NextMonthButton.Click += NextMonthOnClick;
            CurrentMonthButton.Click += CurrentMonthOnClick;
        }

        //Event handler
        private void WindowOnLoad(Object sender, EventArgs e)
        {
            DisplayCalendar(year, month);
            lbDate.Content = DateTime.Now.ToShortDateString();
            lbDateRight.Content = DateTime.Now.ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo)+Week(DateTime.Now);
            HighlightCurrentDate();
            DisplayCalendarByDay();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowOnClose(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowOnMin(Object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 拖拽窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowOnMove(Object sender, EventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// 按天修改日期
        /// </summary>
        private void UpdateMonth()
        {
            CalendarListBox.BeginInit();
            CalendarListBox.Items.Clear();
            DisplayCalendar(year, month);
            CalendarListBox.EndInit();
            lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
            lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            GetWeek();
            CalendarListBox.SelectedItem = null;
            //Check the calendar range and disable corresponding buttons
            CheckRange();
        }

        /// <summary>
        /// 左边按周显示
        /// </summary>
        private void UpdateMonthByWeekOnLeft()
        {
            CalendarListBox.BeginInit();
            CalendarListBox.Items.Clear();
            DisplayCalendarByWeekOnLeft(year, month);
            CalendarListBox.EndInit();
            lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
            lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            GetWeek();
            CalendarListBox.SelectedItem = null;
            CheckRange();
        }

        /// <summary>
        /// 按月份修改
        /// </summary>
        private void UpdateByMonth()
        {
            CalendarListBox1.BeginInit();
            CalendarListBox1.Items.Clear();
            DisplayCalendarByMonth();
            CalendarListBox1.EndInit();
            lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
            lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            CheckRange();
        }

        /// <summary>
        /// 点击周的时候得到日期
        /// </summary>
        private void GetWeek()
        {
            int dayNum = DateTime.DaysInMonth(year, month);
            string week = Week(new DateTime(year, month, day));
            switch (week)
            {
                case "周日":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbSunday.Foreground = Brushes.Red;
                            lbSunday.Content = "周日" + "(" + "今" + ")";
                            if (day + 1 < dayNum)
                            {
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            }
                            if (day + 2 < dayNum)
                            {
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            }
                            if (day + 3 < dayNum)
                            {
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            }
                            if (day + 4 < dayNum)
                            {
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            }
                            if (day + 5 < dayNum)
                            {
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            }
                            if (day + 6 < dayNum)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            }
                        }

                    }
                    break;
                case "周一":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbMonday.Foreground = Brushes.Red;
                            lbMonday.Content = "周一" + "(" + "今" + ")";
                            if (day + 1 < dayNum)
                            {
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            }
                            if (day + 2 < dayNum)
                            {
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            }
                            if (day + 3 < dayNum)
                            {
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            }
                            if (day + 4 < dayNum)
                            {
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            }
                            if (day + 5 < dayNum)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            }
                            if (day > 1)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                            }
                        }
                    }
                    break;
                case "周二":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbTuesday.Foreground = Brushes.Red;
                            lbTuesday.Content = "周二" + "(" + "今" + ")";
                            if (day + 1 < dayNum)
                            {
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            }
                            if (day + 2 < dayNum)
                            {
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            }
                            if (day + 3 < dayNum)
                            {
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            }
                            if (day + 4 < dayNum)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            }
                            if (day > 2)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                            }
                            if (day > 1)
                            {
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                            }
                        }
                    }
                    break;
                case "周三":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbWednesday.Foreground = Brushes.Red;
                            lbWednesday.Content = "周三" + "(" + "今" + ")";
                            if (day + 1 < dayNum)
                            {
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            }
                            if (day + 2 < dayNum)
                            {
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            }

                            if (day + 3 < dayNum)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            }
                            if (day > 3)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                            }
                            if (day > 2)
                            {
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                            }
                            if (day > 1)
                            {
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                            }
                        }
                    }
                    break;
                case "周四":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbThursday.Foreground = Brushes.Red;
                            lbThursday.Content = "周四" + "(" + "今" + ")";
                            if (day + 1 < dayNum)
                            {
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            }
                            if (day + 2 < dayNum)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            }
                            if (day > 4)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                            }
                            if (day > 3)
                            {
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                            }
                            if (day > 2)
                            {
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                            }
                            if (day > 1)
                            {
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                            }
                        }
                    }
                    break;
                case "周五":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbFriday.Foreground = Brushes.Red;
                            lbFriday.Content = "周五" + "(" + "今" + ")";
                            if (day + 1 < dayNum)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            }
                            if (day > 5)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                            }
                            if (day > 4)
                            {
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                            }
                            if (day > 3)
                            {
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                            }
                            if (day > 2)
                            {
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                            }
                            if (day > 1)
                            {
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                            }
                        }
                    }
                    break;
                case "周六":
                    {
                        if (Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)) == week)
                        {
                            lbSaturday.Foreground = Brushes.Red;
                            lbSaturday.Content = "周六" + "(" + "今" + ")";
                            if (day > 1)
                            {
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                            }
                            if (day > 2)
                            {
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                            }
                            if (day > 3)
                            {
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                            }
                            if (day > 4)
                            {
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                            }
                            if (day > 5)
                            {
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                            }
                            if (day > 6)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                            }
                        }

                    }
                    break;
            }
        }

        /// <summary>
        /// 前一个月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousMonthOnClick(Object sender, RoutedEventArgs e)
        {
            if (isByMonth)
            {
                if (year <= 1902)
                {
                    return;
                }
                year -= 1;
                UpdateByMonth();
            }
            else
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
        }

        /// <summary>
        /// 下一个月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextMonthOnClick(Object sender, RoutedEventArgs e)
        {
            if (isByMonth)
            {
                if (year >= 2100)
                {
                    return;
                }

                year += 1;
                UpdateByMonth();
            }
            else
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
           
        }

        /// <summary>
        /// 当月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentMonthOnClick(Object sender, RoutedEventArgs e)
        {
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
            day = DateTime.Now.Day;
            if (isByMonth)
            {
                UpdateByMonth();
            }
            else
            {
                UpdateMonth();
            }
            HighlightCurrentDate();
        }

        /// <summary>
        /// 选择日期(右边)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedDateOnDisplayRight(Object sender, SelectionChangedEventArgs e)
        {
            MonthUserControl border = CalendarListBoxByMonth.SelectedItems as MonthUserControl;
            if (border!=null)
            {
                border.BorderBrush=new SolidColorBrush(Color.FromRgb(0, 228, 225));
                border.BorderThickness = new Thickness(2);
            }
            StackPanel stackPanel = null;
            int selectedDay;
            if (isByMonth)
            {
                stackPanel = (StackPanel)CalendarListBoxByMonth.SelectedItem;
                selectedDay = CalendarListBoxByMonth.SelectedIndex + 1;
                this.day = selectedDay;
                if (selectedDay == 0)
                {
                    this.day = 1;
                }
                this.lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month,day));
                return;
            }

            stackPanel = (StackPanel)CalendarListBoxByMonth.SelectedItem;
            selectedDay = CalendarListBoxByMonth.SelectedIndex + 1;
            if (selectedDatePanel != null)
            {
                selectedDatePanel.Background = Brushes.Gray;
                ((Label)selectedDatePanel.Children[mainLabelIndex]).Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                //Detect whether the application is running in a zh-CN locale
                if (localCultureInfo.ToString() == "zh-CN")
                {
                    ((Label)selectedDatePanel.Children[subsidiaryLabelIndex]).Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
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
                this.lbDate.Content = dateTimeDisplay.ToShortDateString() + " " + festivalString;
                lbDateRight.Content = dateTimeDisplay.ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            }
            else
            {
                this.lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            }
        }

        /// <summary>
        /// 选择日期(左边)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedDateOnDisplay(Object sender, SelectionChangedEventArgs e)
        {
            StackPanel stackPanel = null;
            int selectedDay;
            if (isByMonth)
            {
                stackPanel = (StackPanel)CalendarListBox1.SelectedItem;
                selectedDay = CalendarListBox1.SelectedIndex + 1;
                this.month = selectedDay;
                if (selectedDay == 0)
                {
                    this.month = 1;
                }
                this.lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                return;
            }

            stackPanel = (StackPanel)CalendarListBox.SelectedItem;
            selectedDay = CalendarListBox.SelectedIndex + 1;
            if (selectedDatePanel != null)
            {
                selectedDatePanel.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                ((Label)selectedDatePanel.Children[mainLabelIndex]).Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                //Detect whether the application is running in a zh-CN locale
                if (localCultureInfo.ToString() == "zh-CN")
                {
                    ((Label)selectedDatePanel.Children[subsidiaryLabelIndex]).Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
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
                this.lbDate.Content = dateTimeDisplay.ToShortDateString() + " " + festivalString;
                lbDateRight.Content = dateTimeDisplay.ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            }
            else
            {
                this.lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
            }
        }

        private void CheckRange()
        {
            //The calendar range is between 01/01/1902 and 12/31/2100
            //PreviousYearButton.IsEnabled = (year <= 1902) ? false : true;
            PreviousMonthButton.IsEnabled = (month == 01 && year <= 1902) ? false : true;
            //NextYearButton.IsEnabled = (year >= 2100) ? false : true;
            NextMonthButton.IsEnabled = (month == 12 && year >= 2100) ? false : true;
        }

        /// <summary>
        /// 天
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDay_Click(object sender, RoutedEventArgs e)
        {
            isByMonth = false;
            isByDay = true;
            isByWeek = false;
            gdDay.Visibility = System.Windows.Visibility.Visible;
            gdWeek.Visibility = System.Windows.Visibility.Collapsed;
            gdMonth.Visibility = System.Windows.Visibility.Collapsed;
            UpdateMonth();
            HighlightCurrentDate();
            DisplayCalendarByDay();
        }

        /// <summary>
        /// 周
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWeek_Click(object sender, RoutedEventArgs e)
        {
            gdDay.Visibility = System.Windows.Visibility.Collapsed ;
            gdWeek.Visibility = System.Windows.Visibility.Visible;
            gdMonth.Visibility = System.Windows.Visibility.Collapsed;
            isByMonth = false;
            isByWeek = true;
            isByDay = false;
            UpdateMonthByWeekOnLeft();
            HighlightCurrentDate();
            DisplayCalendarByWeek();
        }

        /// <summary>
        /// 月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMonth_Click(object sender, RoutedEventArgs e)
        {
            gdDay.Visibility = System.Windows.Visibility.Collapsed;
            gdWeek.Visibility = System.Windows.Visibility.Collapsed;
            gdMonth.Visibility = System.Windows.Visibility.Visible;
            isByMonth = true;
            isByDay = false;
            isByWeek = false;
            UpdateByMonth();
            CalendarListBoxByMonth.BeginInit();
            CalendarListBoxByMonth.Items.Clear();
            //DisplayCalendarByMonth();
            DisplayCalendarByMonthRight();
            CalendarListBoxByMonth.EndInit();
        }

        /// <summary>
        /// 前一天时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (isByDay)
            {
                day -= 1;
                if (day == 0)
                {
                    month--;
                    if (month == 0)
                    {
                        month = 12;
                        year--;
                    }
                    day = DateTime.DaysInMonth(year, month);
                }
                CalendarListBox.BeginInit();
                CalendarListBox.Items.Clear();
                DisplayCalendarByDayOnRight(year, month);
                CalendarListBox.EndInit();
            }
            if (isByWeek)
            {
                UpdateMonthByWeekOnLeft();
                #region 点击周后前一个日期
                lbSunday.Foreground = Brushes.Black;
                lbMonday.Foreground = Brushes.Black;
                lbTuesday.Foreground = Brushes.Black;
                lbWednesday.Foreground = Brushes.Black;
                lbThursday.Foreground = Brushes.Black;
                lbFriday.Foreground = Brushes.Black;
                lbSaturday.Foreground = Brushes.Black;
                int dayNum = DateTime.DaysInMonth(year, month);
                string week = Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                switch (week)
                {
                    case "周日":
                        {
                            if (day - 1 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 2 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 3 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 4 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 5 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 6 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 7 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                        }
                        break;
                    case "周一":
                        {
                            if (day - 2 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 3 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 4 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 5 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 6 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 7 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 8 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                        }
                        break;
                    case "周二":
                        {
                            if (day - 3 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 4 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 5 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 6 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 7 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 8 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 9 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                        }
                        break;
                    case "周三":
                        {
                            if (day - 4 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 5 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 6 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 7 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 8 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 9 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 10 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                        }
                        break;
                    case "周四":
                        {
                            if (day - 5 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 6 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 7 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 8 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 9 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 10 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 11 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 11)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                        }
                        break;
                    case "周五":
                        {
                            if (day - 6 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 7 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 8 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 9 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 10 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 11 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 12 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 12)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 12)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 11)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                        }
                        break;
                    case "周六":
                        {
                            if (day - 7 == 0)
                            {
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 6)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 8 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 5)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 9 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 4)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 10 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 3)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 11 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 2)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 12 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 11)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day - 13 == 0)
                            {
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 11)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 12)).ToString("M.d") + ")";
                                month--;
                                if (month == 0)
                                {
                                    year--;
                                    month = 12;
                                }
                                day = DateTime.DaysInMonth(year, month);
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day - 13)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day - 12)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day - 11)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day - 10)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day - 9)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day - 8)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day - 7)).ToString("M.d") + ")";
                        }
                        break;
                }
                day -= 7;
                if (day == 0)
                {
                    month--;
                    if (month == 0)
                    {
                        month = 12;
                        year--;
                    }
                    day = DateTime.DaysInMonth(year, month);
                }
                if (day < 0)
                {
                    month--;
                    if (month == 0)
                    {
                        month = 12;
                        year--;
                    }
                    day = DateTime.DaysInMonth(year, month) - Convert.ToInt32(day.ToString().Replace("-", ""));

                }
                #endregion
            }
            if (isByMonth)
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
                CalendarListBoxByMonth.BeginInit();
                CalendarListBoxByMonth.Items.Clear();
                //DisplayCalendarByMonth();
                DisplayCalendarByMonthRight();
                CalendarListBoxByMonth.EndInit();
                CalendarListBox1.BeginInit();
                CalendarListBox1.Items.Clear();
                DisplayCalendarByMonthOnRight();
                CalendarListBox1.EndInit();
            }
            lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
            lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                

        }

        /// <summary>
        /// 当天时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCurrent_Click(object sender, RoutedEventArgs e)
        {
            day = DateTime.Now.Day;
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
            GetWeek();
            CalendarListBoxByMonth.BeginInit();
            CalendarListBoxByMonth.Items.Clear();
            //DisplayCalendarByMonth();
            DisplayCalendarByMonthRight();
            CalendarListBoxByMonth.EndInit();
            if (isByMonth)
            {
                CalendarListBox1.BeginInit();
                CalendarListBox1.Items.Clear();
                DisplayCalendarByMonthOnRight();
                CalendarListBox1.EndInit();
            }
            if (isByDay)
            {
                CalendarListBox.BeginInit();
                CalendarListBox.Items.Clear();
                DisplayCalendarByDayOnRight(year, month);
                CalendarListBox.EndInit();
            }
            if (isByWeek)
            {
                UpdateMonthByWeekOnLeft();
            }
            lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
            lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
        }

        /// <summary>
        /// 下一个时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (isByDay)
            {
                day += 1;
                if (day > DateTime.DaysInMonth(year, month))
                {
                    day = 1;
                    month++;
                    if (month > 12)
                    {
                        month = 1;
                        year++;
                    }
                }
                CalendarListBox.BeginInit();
                CalendarListBox.Items.Clear();
                DisplayCalendarByDayOnRight(year, month);
                CalendarListBox.EndInit();
            }
            if (isByWeek)
            {
                UpdateMonthByWeekOnLeft();
                #region 点击周后下一个日期
                lbSunday.Foreground = Brushes.Black;
                lbMonday.Foreground = Brushes.Black;
                lbTuesday.Foreground = Brushes.Black;
                lbWednesday.Foreground = Brushes.Black;
                lbThursday.Foreground = Brushes.Black;
                lbFriday.Foreground = Brushes.Black;
                lbSaturday.Foreground = Brushes.Black;
                int dayNum = DateTime.DaysInMonth(year, month);
                string week = Week(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                switch (week)
                {
                    case "周日":
                        {
                            if (day + 7 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7 - dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 8 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 8 - dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 9 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 9 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 10 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 10 - dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 11 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 11 - dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 12 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 12 - dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 13 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 13 - dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 8)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 9)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 10)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 11)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 12)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 13)).ToString("M.d") + ")";
                        }
                        break;
                    case "周一":
                        {
                            if (day + 6 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 6 - dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 7 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7- dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 8 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 8 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 9 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 9- dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 11 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day +11 - dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 12 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 12 - dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 13 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 13 - dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 8)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 9)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 10)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 11)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 12)).ToString("M.d") + ")";
                        }
                        break;
                    case "周二":
                        {
                            if (day + 5 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 5- dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 6 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 6 - dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 7 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 8 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 8 - dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 9 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 9 - dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 10 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 10 - dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 11 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 11- dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 8)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 9)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 10)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 11)).ToString("M.d") + ")";
                        }
                        break;
                    case "周三":
                        {
                            if (day + 4 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day +4- dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 5 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 5- dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 6 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 6 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 7 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7 - dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 8 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 8- dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 9 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 9 - dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 10 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 10 - dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 8)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 9)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 10)).ToString("M.d") + ")";
                        }
                        break;
                    case "周四":
                        {
                            if (day + 3 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 3 - dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 4 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 4 - dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 5 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 5 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 6 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 6 - dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 7 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7 - dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 8 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 8 - dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 9 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 9 - dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 8)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 9)).ToString("M.d") + ")";
                        }
                        break;
                    case "周五":
                        {
                            if (day + 2 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 2 - dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 3 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 3 - dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 4 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 4 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 5 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 5 - dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 6 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 6 - dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 7 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7- dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 8 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 8 - dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 8)).ToString("M.d") + ")";
                        }
                        break;
                    case "周六":
                        {
                            if (day + 1 > dayNum)
                            {
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 1 - dayNum;
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 2 > dayNum)
                            {

                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 2 - dayNum;
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 3 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 3 - dayNum;
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 4 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 4 - dayNum;
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 5 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 5 - dayNum;
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 6 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 6 - dayNum;
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            if (day + 7 > dayNum)
                            {
                                lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                                lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                                lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                                lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                                lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                                lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                                month++;
                                if (month > 12)
                                {
                                    month = 1;
                                    year++;
                                }
                                day = day + 7 - dayNum;
                                lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day)).ToString("M.d") + ")";
                                day += 5;
                                lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
                                lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
                                return;
                            }
                            lbSunday.Content = "周日" + "(" + (new DateTime(year, month, day + 1)).ToString("M.d") + ")";
                            lbMonday.Content = "周一" + "(" + (new DateTime(year, month, day + 2)).ToString("M.d") + ")";
                            lbTuesday.Content = "周二" + "(" + (new DateTime(year, month, day + 3)).ToString("M.d") + ")";
                            lbWednesday.Content = "周三" + "(" + (new DateTime(year, month, day + 4)).ToString("M.d") + ")";
                            lbThursday.Content = "周四" + "(" + (new DateTime(year, month, day + 5)).ToString("M.d") + ")";
                            lbFriday.Content = "周五" + "(" + (new DateTime(year, month, day + 6)).ToString("M.d") + ")";
                            lbSaturday.Content = "周六" + "(" + (new DateTime(year, month, day + 7)).ToString("M.d") + ")";
                        }
                        break;
                }
                day += 7;
                if (day > DateTime.DaysInMonth(year, month))
                {
                    day = day - DateTime.DaysInMonth(year, month);
                    month++;
                    if (month > 12)
                    {
                        month = 1;
                        year++;
                    }

                }
                if (day == DateTime.DaysInMonth(year, month))
                {
                    day = 1;
                    month++;
                    if (month > 12)
                    {
                        month = 1;
                        year++;
                    }
                }
                #endregion
            }
               
            if (isByMonth)
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
              
                CalendarListBoxByMonth.BeginInit();
                CalendarListBoxByMonth.Items.Clear();
                //DisplayCalendarByMonth();
                DisplayCalendarByMonthRight();
                CalendarListBoxByMonth.EndInit();
                CalendarListBox1.BeginInit();
                CalendarListBox1.Items.Clear();
                DisplayCalendarByMonthOnRight();
                CalendarListBox1.EndInit();
               
            }
            lbDate.Content = (new DateTime(year, month, day)).ToShortDateString();
            lbDateRight.Content = (new DateTime(year, month, day)).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + Week(new DateTime(year, month, day)) + Day(new DateTime(year, month, day));
         
          
        }
      
        #region 按天的时候移动和拖拽

        private void dayCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pointUp = this.caDayListBox.PointFromScreen(new Point());
            Point pointDown = this.caDayListBox.PointToScreen(new Point());
            Point point = this.dayCanvas.PointToScreen(new Point());
            double sub = Convert.ToInt32(point.Y) - Convert.ToInt32(pointDown.Y);
            double unit = (caDayListBox.ActualWidth) / 26;
            string timeLeft = (sub / (unit)).ToString("f1");
            string[] strTimeLeft = timeLeft.Split('.');
            int indexLeft = Convert.ToInt32(strTimeLeft[0]);
            if (indexLeft < Hours.Length)
            {
                if (Convert.ToInt32(strTimeLeft[1]) > 5)
                {
                    if (indexLeft>0)
                    {
                        dayCanvas.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0]) - 1].ToString() + ":" + "30"; 
                    }
                    else
                    {
                        dayCanvas.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0])].ToString() + ":" + "30"; 
                    }
                }
                else
                {
                    if (indexLeft>0)
                    {
                        dayCanvas.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0]) - 1].ToString() + ":" + "00";
                    }
                    else
                    {
                        dayCanvas.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0])].ToString() + ":" + "00";
                    }
                    
                }
            }

            string timeRight = ((sub + dayCanvas.ActualHeight) / (unit)).ToString("f1");
            string[] strTimeRight = timeRight.Split('.');
            int indexRight = Convert.ToInt32(strTimeRight[0]);
            if (indexRight < Hours.Length)
            {
                if (Convert.ToInt32(strTimeRight[1]) > 5)
                {
                    if (indexRight>0)
                    {
                        dayCanvas.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0]) - 1].ToString() + ":" + "30";
                    }
                    else
                    {
                        dayCanvas.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0])].ToString() + ":" + "30";
                    }
                }
                else
                {
                    if (indexRight > 0)
                    {
                        dayCanvas.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0]) - 1].ToString() + ":" + "00";
                    }
                    else
                    {
                        dayCanvas.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0])].ToString() + ":" + "00";
                    }
                   
                }
            }
            string hourLeft = dayCanvas.lbTimeLeft.Content.ToString().Substring(0, dayCanvas.lbTimeLeft.Content.ToString().IndexOf(":"));
            string hourRight = dayCanvas.lbTimeRight.Content.ToString().Substring(0, dayCanvas.lbTimeRight.Content.ToString().IndexOf(":"));
            string minutLeft = dayCanvas.lbTimeLeft.Content.ToString().Substring(dayCanvas.lbTimeLeft.Content.ToString().LastIndexOf(":")+1);
            string minutRight = dayCanvas.lbTimeRight.Content.ToString().Substring(dayCanvas.lbTimeRight.Content.ToString().LastIndexOf(":") + 1);
            int hourSub = Convert.ToInt32(hourRight) - Convert.ToInt32(hourLeft);
            int minuteSub = Convert.ToInt32(minutRight) - Convert.ToInt32(minutLeft);
            if (minuteSub>0)
            {
                dayCanvas.lbTimeHour.Content = "(" + (hourSub * 60 + 30).ToString() + "m" + ")";
            }
            if (minuteSub==0)
            {
                 dayCanvas.lbTimeHour.Content = "(" + (hourSub * 60).ToString() + "m" + ")";
            }
            if (minuteSub<0)
            {
                dayCanvas.lbTimeHour.Content = "(" + ((hourSub-1) * 60 + 30).ToString() + "m" + ")";
            }
           
        }

        #endregion

        #region 按周的时候移动和拖拽

        private void dayCanvasWeek_MouseMove(object sender, MouseEventArgs e)
        {
            Point pointUp = this.caWeekListBox.PointFromScreen(new Point());
            Point pointDown = this.caWeekListBox.PointToScreen(new Point());
            Point point = this.dayCanvasWeek.PointToScreen(new Point());
            double sub = Convert.ToInt32(point.Y) - Convert.ToInt32(pointDown.Y);
            double unit = (caWeekListBox.ActualWidth) / 26;
            string timeLeft = (sub / (unit)).ToString("f1");
            string[] strTimeLeft = timeLeft.Split('.');
            int indexLeft = Convert.ToInt32(strTimeLeft[0]);
            if (indexLeft < Hours.Length)
            {
                if (Convert.ToInt32(strTimeLeft[1]) > 5)
                {
                    if (indexLeft>0)
                    {
                        dayCanvasWeek.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0])-1].ToString() + ":" + "30";
                    }
                    else
                    {
                        dayCanvasWeek.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0])].ToString() + ":" + "30";
                    }
                }
                else
                {
                    if (indexLeft>0)
                    {
                        dayCanvasWeek.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0])-1].ToString() + ":" + "00";
                    }
                    else
                    {
                        dayCanvasWeek.lbTimeLeft.Content = Hours[Convert.ToInt32(strTimeLeft[0])].ToString() + ":" + "00";
                    }
                   
                }
            }
           
            string timeRight = ((sub + dayCanvasWeek.ActualHeight) / (unit)).ToString("f1");
            string[] strTimeRight = timeRight.Split('.');
            int indexRight = Convert.ToInt32(strTimeRight[0]);
            if (indexRight < Hours.Length)
            {
                if (Convert.ToInt32(strTimeRight[1]) > 5)
                {
                    if (indexRight>0)
                    {
                        dayCanvasWeek.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0]) - 1].ToString() + ":" + "30"; 
                    }
                    else
                    {
                        dayCanvasWeek.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0])].ToString() + ":" + "30";
                    }
                    
                }
                else
                {
                    if (indexRight>0)
                    {
                        dayCanvasWeek.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0]) - 1].ToString() + ":" + "00";
                    }
                    else
                    {
                        dayCanvasWeek.lbTimeRight.Content = Hours[Convert.ToInt32(strTimeRight[0])].ToString() + ":" + "00";
                    }
                   
                }
            }
            string hourLeft = dayCanvasWeek.lbTimeLeft.Content.ToString().Substring(0, dayCanvasWeek.lbTimeLeft.Content.ToString().IndexOf(":"));
            string hourRight = dayCanvasWeek.lbTimeRight.Content.ToString().Substring(0, dayCanvasWeek.lbTimeRight.Content.ToString().IndexOf(":"));
            string minutLeft = dayCanvasWeek.lbTimeLeft.Content.ToString().Substring(dayCanvasWeek.lbTimeLeft.Content.ToString().LastIndexOf(":") + 1);
            string minutRight = dayCanvasWeek.lbTimeRight.Content.ToString().Substring(dayCanvasWeek.lbTimeRight.Content.ToString().LastIndexOf(":") + 1);
            int hourSub = Convert.ToInt32(hourRight) - Convert.ToInt32(hourLeft);
            int minuteSub = Convert.ToInt32(minutRight) - Convert.ToInt32(minutLeft);
            if (minuteSub > 0)
            {
                dayCanvasWeek.lbTimeHour.Content = "(" + (hourSub * 60 + 30).ToString() + "m" + ")";
            }
            if (minuteSub == 0)
            {
                dayCanvasWeek.lbTimeHour.Content = "(" + (hourSub * 60).ToString() + "m" + ")";
            }
            if (minuteSub < 0)
            {
                dayCanvasWeek.lbTimeHour.Content = "(" + ((hourSub - 1) * 60 + 30).ToString() + "m" + ")";
            }
        }

        #endregion
    }
}
