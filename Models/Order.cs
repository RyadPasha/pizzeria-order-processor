using System;
using System.Collections.Generic;

namespace PizzeriaOrderProcessor.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
