using DentistManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DentistManagement.Model.ServiceClass;

namespace DentistManagement.ViewModel
{
    public class ButtonListVM : INotifyPropertyChanged
    {
        public ButtonListVM()
        {
            OnCollectionChange("en-US");                    // Default buttons from English
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

        private List<ShiftsInformation> categorybuttonList = new List<ShiftsInformation>();
        public List<ShiftsInformation> CategoryButtonList
        {
            get
            {
                return categorybuttonList;
            }
            set
            {
                categorybuttonList = value;
                RaisePropertyChanged("CategoryButtonList");
            }
        }

        private void ButtonClick(object button)
        {
            Button clickedbutton = button as Button;

            if (clickedbutton != null)
            {
                // Here we can check (clickedbutton.Tag) value with static string properties of ButtonNames class..

                string msg = string.Format("You Pressed : {0} button", clickedbutton.Tag);                
                MessageBox.Show(msg);
            }
        }

        public  void OnCollectionChange(object lang)
        {
            CategoryItem item1 = new CategoryItem();
            CategoryItem item2 = new CategoryItem();
            CategoryItem item3 = new CategoryItem();         

            if (lang.ToString().Equals("en-US"))                    // If English button is pressed
            {
                item1.ButtonContent = EnglishCategory.SaveButton;
                item1.ButtonTag = ButtonNames.SaveButton;

                item2.ButtonContent = EnglishCategory.OpenButton;
                item2.ButtonTag = ButtonNames.OpenButton;

                item3.ButtonContent = EnglishCategory.CloseButton;
                item3.ButtonTag = ButtonNames.CloseButton;               
            }
            else                                                    // If Danish button is pressed
            {
                item1.ButtonContent = DanishCategory.SaveButton;
                item1.ButtonTag = ButtonNames.SaveButton;

                item2.ButtonContent = DanishCategory.OpenButton;
                item2.ButtonTag = ButtonNames.OpenButton;

                item3.ButtonContent = DanishCategory.CloseButton;
                item3.ButtonTag = ButtonNames.CloseButton;              
            }


            CategoryButtonList = new List<ShiftsInformation>();           // Intialize the button list
            CategoryButtonList = ShiftsInforService.RetrieveShiftsInforList();
            //CategoryButtonList.Add(item1);
            //CategoryButtonList.Add(item2);
            //CategoryButtonList.Add(item3);          
        }       
    

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class CategoryItem
    {
        public string ButtonContent { get; set; }
        public string ButtonTag { get; set; }
    }



    
}
