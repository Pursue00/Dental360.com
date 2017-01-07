using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DentistManagement.Model;
using DentistManagement.View;

namespace DentistManagement.ViewModel
{
    public class DeleteRowReminderViewModel : INotifyPropertyChanged
    {
        public static DeleteRowReminder DeleteRowReminder { get; set; }
        //关闭
        public DelegateCommand CloseCommand { get; set; }

        public void CloseCommandExecute(object obj)
        {
            DeleteRowReminder.Close();
        }
        //确定
        public DelegateCommand SaveCommand { get; set; }

        public void SaveCommandExecute(object obj)
        {
            DeleteRowReminder.DialogResult = true;
            DeleteRowReminder.Close();
        }

        public DeleteRowReminderViewModel()
        {
            this.CloseCommand = new DelegateCommand(CloseCommandExecute, arg => true);
            this.SaveCommand = new DelegateCommand(SaveCommandExecute, arg => true);
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
