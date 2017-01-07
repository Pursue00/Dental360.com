using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DentistManagement.Model
{
    interface ICalendar
    {
        List<DateEntry> GetDaysOfMonth(int year, int month);
    }
}
