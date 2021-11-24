using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship
{
    public class Person
    {
        public string Name { get; set; }
        public List<Book> TakenBooks { get; set; }
        public List<int> DaysTaken { get; set; }
        public List<DateTime> DateTaken { get; set; }
    }
}
