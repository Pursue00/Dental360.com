using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DentistManagement.Model.ServiceClass
{
    public class WorkTime
    {
        public List<string> cbHourList
        {
            get
            {
                return new List<string> { "08", "09","10","11","12","13","14","15","16","17","18","19","20","21","22" };
            }
        }
        public List<string> cbMinuteList
        {
            get
            {
                return new List<string> { "00","30"};
            }
        }
        public List<string> cbDateList
        {
            get
            {
                return new List<string> { "2015-03", "2015-04", "2015-05", "2015-06" };
            }
        }
    }
     
}
