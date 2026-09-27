using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syed.PortfolioPlatform.Application.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
