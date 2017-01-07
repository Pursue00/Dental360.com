using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DentistManagement.Model.ServiceClass;
using DentistManagement.Model;
using DentistManagement.View;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DentistManagement.ViewModel
{
    public class ShiftsSettingViewModel : INotifyPropertyChanged
    {
        public static ShiftsSetting ShiftsSetting { get; set; }
        public static List<ShiftsInformation> shiftsinformationList { get; set; }
        //关闭
        public DelegateCommand CloseCommand { get; set; }

        public void CloseCommandExecute(object obj)
        {
            ShiftsSetting.Close();
        }
        //确定
        public DelegateCommand SaveCommand { get; set; }

        public void SaveCommandExecute(object obj)
        {
            shiftsinformationList = ShiftsInformationList.ToList();
            ShiftsSetting.DialogResult = true;
            ShiftsSetting.Close();
        }
        //上移
        public DelegateCommand UpCommand { get; set; }

        public void UpCommandExecute(object obj)
        {
            try
            {
                ShiftsInformation ShiftsInformation = ShiftsSetting.dgShiftsSet.SelectedItem as ShiftsInformation;
                if (ShiftsInformation != null)
                {
                    int index=0;
                    for (int i = 0; i < this.ShiftsInformationList.Count; i++)
                    {
                        if (ShiftsInformation.WorkTime== this.ShiftsInformationList[i].WorkTime)
                        {
                            index = i;
                        }
                    }
                    if (index==0)
                    {
                        return;
                    }
                    this.ShiftsInformationList.Remove(ShiftsInformation);
                    this.ShiftsInformationList.Insert(index - 1, ShiftsInformation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        //下移
        public DelegateCommand DownCommand { get; set; }

        public void DownCommandExecute(object obj)
        {
            try
            {
                ShiftsInformation ShiftsInformation = ShiftsSetting.dgShiftsSet.SelectedItem as ShiftsInformation;
                if (ShiftsInformation != null)
                {
                    int index = 0;
                    for (int i = 0; i < this.ShiftsInformationList.Count; i++)
                    {
                        if (ShiftsInformation.WorkTime == this.ShiftsInformationList[i].WorkTime)
                        {
                            index = i;
                        }
                    }
                    if (index == this.ShiftsInformationList.Count-1)
                    {
                        return;
                    }
                    this.ShiftsInformationList.Remove(ShiftsInformation);
                    this.ShiftsInformationList.Insert(index+1, ShiftsInformation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //删除
        public DelegateCommand DeleteCommand { get; set; }

        public void DeleteCommandExecute(object obj)
        {
            ShiftsInformation ShiftsInformation = ShiftsSetting.dgShiftsSet.SelectedItem as ShiftsInformation;
            DeleteRowReminder DeleteRowReminder = new DeleteRowReminder();
            DeleteRowReminder.ShowInTaskbar = false;
            DeleteRowReminder.Owner = ShiftsSetting;
            DeleteRowReminder.ShowDialog();
            if (DeleteRowReminder.DialogResult==true)
            {
                var item = this.ShiftsInformationList.SingleOrDefault(x => x.WorkTime == ShiftsInformation.WorkTime);
                if (item!=null)
                {
                    this.ShiftsInformationList.Remove(item);
                }
            }
        }

        //排班数据源
        private ObservableCollection<ShiftsInformation> _shiftsInformationList;
        public ObservableCollection<ShiftsInformation> ShiftsInformationList
        {
            get
            {
                return this._shiftsInformationList;
            }
            set
            {
                if (this._shiftsInformationList!=value)
                {
                    this._shiftsInformationList = value;
                    OnPropertyChanged("ShiftsInformationList");
                }
            }
        }
       
        public ShiftsSettingViewModel()
        {
            this.DeleteCommand = new DelegateCommand(DeleteCommandExecute, arg => true);
            this.CloseCommand = new DelegateCommand(CloseCommandExecute, arg => true);
            this.SaveCommand = new DelegateCommand(SaveCommandExecute, arg => true);
            this.UpCommand = new DelegateCommand(UpCommandExecute, arg => true);
            this.DownCommand = new DelegateCommand(DownCommandExecute, arg => true);
            this.ShiftsInformationList = new ObservableCollection<ShiftsInformation>(ShiftsInforService.RetrieveShiftsInforList());
            
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
