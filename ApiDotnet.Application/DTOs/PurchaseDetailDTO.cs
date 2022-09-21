using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDotnet.Application.DTOs
{
    public class PurchaseDetailDTO
    {
        public int Id { get; set; }
        public string Person { get; set; }
        public string Product { get; set; }
        public DateTime Date { get; set; }

    }
}