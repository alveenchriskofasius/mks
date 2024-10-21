using API.Context.Table;
using Microsoft.AspNetCore.Mvc.Rendering;
using MitraKaryaSystem.Models;

namespace API.Models
{
    public class SalesOrderViewModel
    {
        public SalesOrderModel SalesOrder { get; set; } = new SalesOrderModel();
        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
    }
}
