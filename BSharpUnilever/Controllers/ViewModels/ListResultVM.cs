using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels
{
    public class ListResultVM<T>
    {
        public int Skip { get; set; }

        public int Top { get; set; }

        public string OrderBy { get; set; }

        public bool Desc { get; set; }

        public int TotalCount { get; set; }

        public Dictionary<string, object> Bag { get; set; }

        public List<T> Data { get; set; }
    }
}
