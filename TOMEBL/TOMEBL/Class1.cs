using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMEBL
{
    public class Class1
    {
        public static ObservableCollection<string> Info = new ObservableCollection<string>()
        {
            "test",
            "here"
        };

        public static bool Test()
        {
            return Info.Where(x => x == "test").Any();
        }
    }
}
