using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDotnet.Domain.Repositories
{
    public class PageBasedRequest
    {

        public int Page { get; set; }
        public int PageSize { get; set; }
        public string OrderByProperty { get; set; }

        public PageBasedRequest()
        {
            Page = 1;
            PageSize = 10;
            OrderByProperty = "Id";
        }

    }
}