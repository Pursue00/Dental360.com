using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DentistManagement.Model.ServiceClass
{
    
   public  class ShiftsInforService
    {
        private static  List<ShiftsInformation> ShiftsInformationList = null;
        static ShiftsInforService()
        {
            ShiftsInformationList = new List<ShiftsInformation>();
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "全勤", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Green" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "请假", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Yellow" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "早退", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Pink" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "迟到", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Red" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "缺勤", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Red" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "早班", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Blue" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "中班", WorkStartHour = "10", WorkStartMinute = "00", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Paper" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "晚班", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Orange" });
            ShiftsInformationList.Add(new ShiftsInformation() { WorkTime = "休息", WorkStartHour = "09", WorkStartMinute = "30", WorkEndHour = "20", WorkEndMinute = "00", Corlor = "Green" });
        }
        public static List<ShiftsInformation> RetrieveShiftsInforList()
        {
            return ShiftsInformationList;
        }
    }
}
