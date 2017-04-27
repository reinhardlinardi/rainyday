using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RainyDay.Models
{
    public class NewsMatch
    {
        public int start { get; set; } // start at index
        public int end { get; set; } // end at index
        public int isContent { get; set; } // match in content?

        public NewsMatch()
        {
            start = -1; // default = not found
            end = -1;
            isContent = -1; // -1 = tidak ketemu, 1 = true, 0 = false
        }
    }
}
