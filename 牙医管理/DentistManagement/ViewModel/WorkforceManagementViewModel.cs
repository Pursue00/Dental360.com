using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DentistManagement.View;
using DentistManagement.Model;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using DentistManagement.Model.ServiceClass;
using System.Windows.Media;
using System.Windows.Input;
using DentistManagement.Helper;


namespace DentistManagement.ViewModel
{
    public class WorkforceManagementViewModel : INotifyPropertyChanged
    {
        public static WorkforceManagement WorkforceManagement { get; set; }
        public static ButtonListUC ButtonListUC { get; set; }
       
        private UniformGrid dutyDisplayUniformGrid;
        //关闭
        public DelegateCommand CloseCommand { get; set; }

        public void CloseCommandExecute(object obj)
        {
          
            WorkforceManagement.Close();
        }
        WorkforceManagements WorkforceManagements = null;
        //班次设置
        public DelegateCommand WorkTimeSettingCommand { get; set; }

        public void WorkTimeSettingCommandExecute(object obj)
        {
            ShiftsSetting ShiftsSetting = new ShiftsSetting();
            ShiftsSetting.ShowInTaskbar = false;
            ShiftsSetting.Owner = WorkforceManagement;
            ShiftsSetting.ShowDialog();
            if (ShiftsSetting.DialogResult==true)
            {
                ButtonListUC.listBox.ItemsSource = ShiftsSettingViewModel.shiftsinformationList;
                Initializationshift();
                WorkforceManagements = WorkforceManagement.dgWorkManagement.SelectedItem as WorkforceManagements;
                string[] shifts = getProperties(WorkforceManagements).Split(',');
                for (int i = 0; i < shifts.Length; i++)
                {
                    switch (shifts[i].Substring(shifts[i].IndexOf(':') + 1))
                    {
                        case "全勤":
                            {
                               var item= ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "全勤");
                               if (item == null)
                               {
                                   string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                   ClearShifts(date);
                               }
                            }
                            break;
                        case "休息":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "休息");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "请假":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "请假");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "早退":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "早退");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "迟到":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "迟到");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "缺勤":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "缺勤");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "早班":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "早班");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "中班":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "中班");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                        case "晚班":
                            {
                                var item = ShiftsSettingViewModel.shiftsinformationList.SingleOrDefault(x => x.WorkTime == "晚班");
                                if (item == null)
                                {
                                    string date = shifts[i].Substring(0, shifts[i].IndexOf(':'));
                                    ClearShifts(date);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void ClearShifts(string date)
        {
            switch (date)
            {
                case "Date1":
                    {
                        WorkforceManagements.Date1 = string.Empty;
                    }
                    break;
                case "Date2":
                    {
                        WorkforceManagements.Date2 = string.Empty;
                    }
                    break;
                case "Date3":
                    {
                        WorkforceManagements.Date3 = string.Empty;
                    }
                    break;
                case "Date4":
                    {
                        WorkforceManagements.Date4 = string.Empty;
                    }
                    break;
                case "Date5":
                    {
                        WorkforceManagements.Date5 = string.Empty;
                    }
                    break;
                case "Date6":
                    {
                        WorkforceManagements.Date6 = string.Empty;
                    }
                    break;
                case "Date7":
                    {
                        WorkforceManagements.Date7 = string.Empty;
                    }
                    break;
                case "Date8":
                    {
                        WorkforceManagements.Date8 = string.Empty;
                    }
                    break;
                case "Date9":
                    {
                        WorkforceManagements.Date9 = string.Empty;
                    }
                    break;
                case "Date10":
                    {
                        WorkforceManagements.Date10 = string.Empty;
                    }
                    break;
                case "Date11":
                    {
                        WorkforceManagements.Date11 = string.Empty;
                    }
                    break;
                case "Date12":
                    {
                        WorkforceManagements.Date12 = string.Empty;
                    }
                    break;
                case "Date13":
                    {
                        WorkforceManagements.Date13 = string.Empty;
                    }
                    break;
                case "Date14":
                    {
                        WorkforceManagements.Date14 = string.Empty;
                    }
                    break;
                case "Date15":
                    {
                        WorkforceManagements.Date15 = string.Empty;
                    }
                    break;
                case "Date16":
                    {
                        WorkforceManagements.Date16 = string.Empty;
                    }
                    break;
                case "Date17":
                    {
                        WorkforceManagements.Date17 = string.Empty;
                    }
                    break;
                case "Date18":
                    {
                        WorkforceManagements.Date18 = string.Empty;
                    }
                    break;
                case "Date19":
                    {
                        WorkforceManagements.Date19 = string.Empty;
                    }
                    break;
                case "Date20":
                    {
                        WorkforceManagements.Date20 = string.Empty;
                    }
                    break;
                case "Date21":
                    {
                        WorkforceManagements.Date21 = string.Empty;
                    }
                    break;
                case "Date22":
                    {
                        WorkforceManagements.Date22 = string.Empty;
                    }
                    break;
                case "Date23":
                    {
                        WorkforceManagements.Date23 = string.Empty;
                    }
                    break;
                case "Date24":
                    {
                        WorkforceManagements.Date24 = string.Empty;
                    }
                    break;
                case "Date25":
                    {
                        WorkforceManagements.Date25 = string.Empty;
                    }
                    break;
                case "Date26":
                    {
                        WorkforceManagements.Date26 = string.Empty;
                    }
                    break;
                case "Date27":
                    {
                        WorkforceManagements.Date27 = string.Empty;
                    }
                    break;
                case "Date28":
                    {
                        WorkforceManagements.Date28 = string.Empty;
                    }
                    break;
                case "Date29":
                    {
                        WorkforceManagements.Date29 = string.Empty;
                    }
                    break;
                case "Date30":
                    {
                        WorkforceManagements.Date30 = string.Empty;
                    }
                    break;
                case "Date31":
                    {
                        WorkforceManagements.Date31 = string.Empty;
                    }
                    break;
            }
        }

        private string _cellColor;
        public string CellColor
        {
            get
            {
                return this._cellColor;
            }
            set
            {
                if (this._cellColor != value)
                {
                    this._cellColor = value;
                    OnPropertyChanged("CellColor");
                }
            }
        }

        private ObservableCollection<WorkforceManagements> _workforceManagementList;
        public ObservableCollection<WorkforceManagements> WorkforceManagementList
        {
            get
            {
                return this._workforceManagementList;
            }
            set
            {
                if (this._workforceManagementList != value)
                {
                    this._workforceManagementList = value;
                    OnPropertyChanged("WorkforceManagementList");
                }
            }
        }

        private ObservableCollection<ShiftsInformation> _categorybuttonList;
        public ObservableCollection<ShiftsInformation> CategoryButtonList
        {
            get
            {
                return _categorybuttonList;
            }
            set
            {
                if (_categorybuttonList!=value)
                {
                    _categorybuttonList = value;
                    OnPropertyChanged("CategoryButtonList");
                }
              
            }
        }

        List<WorkforceManagements> WorkforceList = null;
        List<string> weekList = null;
        public WorkforceManagementViewModel()
        {
            this.CloseCommand = new DelegateCommand(CloseCommandExecute, arg => true);
            this.WorkTimeSettingCommand = new DelegateCommand(WorkTimeSettingCommandExecute, arg => true);
            this.PropertyChanged += ModelPropertyChanged;
            OnCollectionChange("en-US");  
        }

        void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "WorkforceDate":
                    {
                        if (WorkforceDate!=null)
                        {
                            //WorkforceList = null;
                            //weekList = null;
                            //WorkforceManagementList = null;
                            WorkforceList = new List<WorkforceManagements>();
                            weekList = new List<string>();
                            string[] getDate = WorkforceDate.Split('-');
                            int dayNum = DateTime.DaysInMonth(Convert.ToInt32(getDate[0]), Convert.ToInt32(getDate[1]));
                            for (int i = 1; i < dayNum + 1; i++)
                            {
                                DateTime dateTime = new DateTime(Convert.ToInt32(getDate[0]), Convert.ToInt32(getDate[1]), i);
                                weekList.Add(Week(dateTime));
                            }
                            switch (weekList.Count)
                            {
                                case 28:
                                    {
                                        Initializationshift();
                                        WorkforceList.Add(new WorkforceManagements() { Date1 = weekList[0], Date2 = weekList[1], Date3 = weekList[2], Date4 = weekList[3], Date5 = weekList[4], Date6 = weekList[5], Date7 = weekList[6], Date8 = weekList[7], Date9 = weekList[8], Date10 = weekList[9], Date11 = weekList[10], Date12 = weekList[11], Date13 = weekList[12], Date14 = weekList[13], Date15 = weekList[14], Date16 = weekList[15], Date17 = weekList[16], Date18 = weekList[17], Date19 = weekList[18], Date20 = weekList[19], Date21 = weekList[20], Date22 = weekList[21], Date23 = weekList[22], Date24 = weekList[23], Date25 = weekList[24], Date26 = weekList[25], Date27 = weekList[26], Date28 = weekList[27], Date29 = weekList[28], Rest = "合计", Allwork = "合计", Leave = "合计", Early = "合计", Late = "合计", Absence = "合计", EarlyWork = "合计", MinddleWork = "合计", LateWork = "合计" });
                                        WorkforceManagement.header29.Visibility = Visibility.Hidden;
                                        WorkforceManagement.header30.Visibility = Visibility.Hidden;
                                        WorkforceManagement.header31.Visibility = Visibility.Hidden;
                                    }
                                    break;
                                case 30:
                                    {
                                        Initializationshift();
                                        WorkforceList.Add(new WorkforceManagements() { Date1 = weekList[0], Date2 = weekList[1], Date3 = weekList[2], Date4 = weekList[3], Date5 = weekList[4], Date6 = weekList[5], Date7 = weekList[6], Date8 = weekList[7], Date9 = weekList[8], Date10 = weekList[9], Date11 = weekList[10], Date12 = weekList[11], Date13 = weekList[12], Date14 = weekList[13], Date15 = weekList[14], Date16 = weekList[15], Date17 = weekList[16], Date18 = weekList[17], Date19 = weekList[18], Date20 = weekList[19], Date21 = weekList[20], Date22 = weekList[21], Date23 = weekList[22], Date24 = weekList[23], Date25 = weekList[24], Date26 = weekList[25], Date27 = weekList[26], Date28 = weekList[27], Date29 = weekList[28], Date30 = weekList[29], Rest = "合计", Allwork = "合计", Leave = "合计", Early = "合计", Late = "合计", Absence = "合计", EarlyWork = "合计", MinddleWork = "合计", LateWork = "合计" });
                                        WorkforceManagement.header31.Visibility = Visibility.Hidden;
                                    }
                                    break;
                                case 31:
                                    {
                                        Initializationshift();
                                        WorkforceList.Add(new WorkforceManagements() { Date1 = weekList[0], Date2 = weekList[1], Date3 = weekList[2], Date4 = weekList[3], Date5 = weekList[4], Date6 = weekList[5], Date7 = weekList[6], Date8 = weekList[7], Date9 = weekList[8], Date10 = weekList[9], Date11 = weekList[10], Date12 = weekList[11], Date13 = weekList[12], Date14 = weekList[13], Date15 = weekList[14], Date16 = weekList[15], Date17 = weekList[16], Date18 = weekList[17], Date19 = weekList[18], Date20 = weekList[19], Date21 = weekList[20], Date22 = weekList[21], Date23 = weekList[22], Date24 = weekList[23], Date25 = weekList[24], Date26 = weekList[25], Date27 = weekList[26], Date28 = weekList[27], Date29 = weekList[28], Date30 = weekList[29], Date31 = weekList[30], Rest = "合计", Allwork = "合计", Leave = "合计", Early = "合计", Late = "合计", Absence = "合计", EarlyWork = "合计", MinddleWork = "合计", LateWork = "合计" });
                                        WorkforceManagement.header29.Visibility = Visibility.Visible;
                                        WorkforceManagement.header30.Visibility = Visibility.Visible;
                                        WorkforceManagement.header31.Visibility = Visibility.Visible;
                                    }
                                    break;
                               
                            }

                            WorkforceList.Add(new WorkforceManagements() { Name = "凡祥", Rest = "0", Allwork = "0", Leave = "0", Early = "0", Late = "0", Absence = "0", EarlyWork = "0", MinddleWork = "0", LateWork = "0" });
                            WorkforceManagementList = new ObservableCollection<WorkforceManagements>(WorkforceList);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 初始化班次（首次加载都为不可见）
        /// </summary>
        public void Initializationshift()
        {
            WorkforceManagement.headerAllwork.Visibility = Visibility.Hidden;
            WorkforceManagement.headerLeave.Visibility = Visibility.Hidden;
            WorkforceManagement.headerEarly.Visibility = Visibility.Hidden;
            WorkforceManagement.headerLate.Visibility = Visibility.Hidden;
            WorkforceManagement.headerAbsence.Visibility = Visibility.Hidden;
            WorkforceManagement.headerEarlyWork.Visibility = Visibility.Hidden;
            WorkforceManagement.headerMinddleWork.Visibility = Visibility.Hidden;
            WorkforceManagement.headerLateWork.Visibility = Visibility.Hidden;
            WorkforceManagement.headerRest.Visibility = Visibility.Hidden;
            if (ShiftsSettingViewModel.shiftsinformationList != null)
            {
                for (int i = 0; i < ShiftsSettingViewModel.shiftsinformationList.Count; i++)
                {
                    switch (ShiftsSettingViewModel.shiftsinformationList[i].WorkTime)
                    {
                        case "全勤":
                            {
                                WorkforceManagement.headerAllwork.Visibility = Visibility.Visible;
                            }
                            break;
                        case "请假":
                            {
                                WorkforceManagement.headerLeave.Visibility = Visibility.Visible;
                            }
                            break;
                        case "早退":
                            {
                                WorkforceManagement.headerEarly.Visibility = Visibility.Visible;
                            }
                            break;
                        case "迟到":
                            {
                                WorkforceManagement.headerLate.Visibility = Visibility.Visible;
                            }
                            break;
                        case "缺勤":
                            {
                                WorkforceManagement.headerAbsence.Visibility = Visibility.Visible;
                            }
                            break;
                        case "早班":
                            {
                                WorkforceManagement.headerEarlyWork.Visibility = Visibility.Visible;
                            }
                            break;
                        case "中班":
                            {
                                WorkforceManagement.headerMinddleWork.Visibility = Visibility.Visible;
                            }
                            break;
                        case "晚班":
                            {
                                WorkforceManagement.headerLateWork.Visibility = Visibility.Visible;
                            }
                            break;
                        case "休息":
                            {
                                WorkforceManagement.headerRest.Visibility = Visibility.Visible;
                            }
                            break;

                    }
                }
            }
            else
            {
                WorkforceManagement.headerAllwork.Visibility = Visibility.Visible;
                WorkforceManagement.headerLeave.Visibility = Visibility.Visible;
                WorkforceManagement.headerEarly.Visibility = Visibility.Visible;
                WorkforceManagement.headerLate.Visibility = Visibility.Visible;
                WorkforceManagement.headerAbsence.Visibility = Visibility.Visible;
                WorkforceManagement.headerEarlyWork.Visibility = Visibility.Visible;
                WorkforceManagement.headerMinddleWork.Visibility = Visibility.Visible;
                WorkforceManagement.headerLateWork.Visibility = Visibility.Visible;
                WorkforceManagement.headerRest.Visibility = Visibility.Visible;
            }
        }

        private string _workforceDate;
        public string WorkforceDate
        {
            get
            {
                return this._workforceDate;
            }
            set
            {
                if (this._workforceDate != value)
                {
                    this._workforceDate = value;
                    OnPropertyChanged("WorkforceDate");
                }
            }
        }

        private string Week(DateTime currentDay)
        {
            string[] weekdays = { "日", "一", "二", "三", "四", "五", "六" };
            string week = weekdays[Convert.ToInt32(currentDay.DayOfWeek)];
            return week;
        }

        ICommand onButtonClickCommand;
        public ICommand OnButtonClickCommand
        {
            get { return onButtonClickCommand ?? (onButtonClickCommand = new RelayCommand(ButtonClick)); }
        }

        ICommand onCollectionChangeCommand;
        public ICommand OnCollectionChangeCommand
        {
            get { return onCollectionChangeCommand ?? (onCollectionChangeCommand = new RelayCommand(OnCollectionChange)); }
        }

        public void OnCollectionChange(object lang)
        {
            CategoryButtonList = new ObservableCollection<ShiftsInformation>(ShiftsInforService.RetrieveShiftsInforList());
        }
       
        private void ButtonClick(object button)
        {
            Button clickedbutton = button as Button;
            if (clickedbutton != null)
            {
                string msg = string.Format("You Pressed : {0} button", clickedbutton.Tag);
                WorkforceManagements WorkforceManagements = WorkforceManagement.dgWorkManagement.SelectedItem as WorkforceManagements;
                if (WorkforceManagements != null)
                {
                    int index = WorkforceManagement.dgWorkManagement.CurrentCell.Column.DisplayIndex;
                    switch (index)
                    {
                        case 1:
                            {
                                WorkforceManagements.Date1 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 2:
                            {
                                WorkforceManagements.Date2 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 3:
                            {
                                WorkforceManagements.Date3 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 4:
                            {
                                WorkforceManagements.Date4 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 5:
                            {
                                WorkforceManagements.Date5 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 6:
                            {
                                WorkforceManagements.Date6 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 7:
                            {
                                WorkforceManagements.Date7 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 8:
                            {
                                WorkforceManagements.Date8 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 9:
                            {
                                WorkforceManagements.Date9 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 10:
                            {
                                WorkforceManagements.Date10 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 11:
                            {
                                WorkforceManagements.Date11 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 12:
                            {
                                WorkforceManagements.Date12 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 13:
                            {
                                WorkforceManagements.Date13 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 14:
                            {
                                WorkforceManagements.Date14 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 15:
                            {
                                WorkforceManagements.Date15 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 16:
                            {
                                WorkforceManagements.Date16 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 17:
                            {
                                WorkforceManagements.Date17 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 18:
                            {
                                WorkforceManagements.Date18 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 19:
                            {
                                WorkforceManagements.Date19 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 20:
                            {
                                WorkforceManagements.Date20 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 21:
                            {
                                WorkforceManagements.Date21 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 22:
                            {
                                WorkforceManagements.Date22 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 23:
                            {
                                WorkforceManagements.Date23 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 24:
                            {
                                WorkforceManagements.Date24 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 25:
                            {
                                WorkforceManagements.Date25 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 26:
                            {
                                WorkforceManagements.Date26 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 27:
                            {
                                WorkforceManagements.Date27 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 28:
                            {
                                WorkforceManagements.Date28 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 29:
                            {
                                WorkforceManagements.Date29 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 30:
                            {
                                WorkforceManagements.Date30 = clickedbutton.Tag.ToString();
                            }
                            break;
                        case 31:
                            {
                                WorkforceManagements.Date31 = clickedbutton.Tag.ToString();
                            }
                            break;
                    }
                    string[] shifts = getProperties(WorkforceManagements).Split(',');
                    int Rest = 0;
                    int Allwork = 0;
                    int Leave = 0;
                    int Early = 0;
                    int Late = 0;
                    int Absence = 0;
                    int EarlyWork = 0;
                    int MinddleWork = 0;
                    int LateWork = 0;
                    for (int i = 0; i < shifts.Length; i++)
                    {
                        switch (shifts[i].Substring(shifts[i].IndexOf(':')+1))
                        {
                            case "全勤":
                                {
                                    Allwork++;
                                }
                                break;
                            case "休息":
                                {
                                    Rest++;
                                }
                                break;
                            case "请假":
                                {
                                    Leave++;
                                }
                                break;
                            case "早退":
                                {
                                    Early++;
                                }
                                break;
                            case "迟到":
                                {
                                    Late++;
                                }
                                break;
                            case "缺勤":
                                {
                                    Absence++;
                                }
                                break;
                            case "早班":
                                {
                                    EarlyWork++;
                                }
                                break;
                            case "中班":
                                {
                                    MinddleWork++;
                                }
                                break;
                            case "晚班":
                                {
                                    LateWork++;
                                }
                                break;
                        }
                    }
                    WorkforceManagements.Absence = Absence.ToString();
                    WorkforceManagements.Rest = Rest.ToString();
                    WorkforceManagements.Allwork = Allwork.ToString();
                    WorkforceManagements.Early = Early.ToString();
                    WorkforceManagements.EarlyWork = EarlyWork.ToString();
                    WorkforceManagements.Late = Late.ToString();
                    WorkforceManagements.LateWork = LateWork.ToString();
                    WorkforceManagements.Leave = Leave.ToString();
                    WorkforceManagements.MinddleWork = MinddleWork.ToString();
                   
                }
            }
        }
        //遍历获取类的属性及属性的值：

        public string getProperties<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    tStr += string.Format("{0}:{1},", name, value);
                }
                else
                {
                    getProperties(value);
                }
            }
            return tStr;
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
