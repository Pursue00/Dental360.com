using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DentistManagement.Model
{
    class DateEntry
    {
        public int DateOfMonth;
        public string Text;
        public bool IsFestival;

        public DateEntry(int date, string text, bool isFestival)
        {
            DateOfMonth = date;
            Text = text;
            IsFestival = isFestival;
        }
    }
    class Month
    {
        public int DateOfMonth;
        public Month(int month)
        {
            DateOfMonth = month;
        }
    }
}
