using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DentistManagement.Model
{
    public class MyPoint : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        private int m_x;

        public int X
        {
            get { return m_x; }
            set { m_x = value; OnPropertyChanged("X"); }
        }
        private int m_y;

        public int Y
        {
            get { return m_y; }
            set { m_y = value; OnPropertyChanged("Y"); }
        }
    }
}