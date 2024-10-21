﻿namespace API.Models
{
    public class SalesOrderDetailModel
    {
        public int ID { get; set; }
        public int? ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
