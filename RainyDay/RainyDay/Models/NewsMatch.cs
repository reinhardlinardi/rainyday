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
        public int isContent { get; set; } // -1 = tidak ketemu, 0 = di judul, 1 = di beritanya

        public NewsMatch()
        {
            start = -1;
            end = -1;
            isContent = -1;
        }
    }
}
