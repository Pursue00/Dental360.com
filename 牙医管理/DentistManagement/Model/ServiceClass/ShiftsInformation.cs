using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace DentistManagement.Model.ServiceClass
{
   public  class ShiftsInformation : INotifyPropertyChanged
    {
       //班次
       private string _workTime;
       public string WorkTime
       {
           get
           {
               return this._workTime;
           }
           set
           {
               if (this._workTime!=value)
               {
                   this._workTime = value;
                   OnPropertyChanged("WorkTime");
               }
           }
       }
       //上班时间
       private string _workStartHour;
       public string WorkStartHour
       {
           get
           {
               return this._workStartHour;
           }
           set
           {
               if (this._workStartHour != value)
               {
                   this._workStartHour = value;
                   OnPropertyChanged("WorkStartHour");
               }
           }
       }
       private string _workStartMinute;
       public string WorkStartMinute
       {
           get
           {
               return this._workStartMinute;
           }
           set
           {
               if (this._workStartMinute != value)
               {
                   this._workStartMinute = value;
                   OnPropertyChanged("WorkStartMinute");
               }
           }
       }
       //下班时间
       private string _workEndHour;
       public string WorkEndHour
       {
           get
           {
               return this._workEndHour;
           }
           set
           {
               if (this._workEndHour != value)
               {
                   this._workEndHour = value;
                   OnPropertyChanged("WorkEndHour");
               }
           }
       }
       private string _workEndMinute;
       public string WorkEndMinute
       {
           get
           {
               return this._workEndMinute;
           }
           set
           {
               if (this._workEndMinute != value)
               {
                   this._workEndMinute = value;
                   OnPropertyChanged("WorkEndMinute");
               }
           }
       }
       //颜色
       private string _corlor;
       public string Corlor
       {
           get
           {
               return this._corlor;
           }
           set
           {
               if (this._corlor != value)
               {
                   this._corlor = value;
                   OnPropertyChanged("Corlor");
               }
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
