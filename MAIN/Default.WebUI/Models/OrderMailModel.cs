using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Default.WebUI.Models
{
    public class OrderMailModel
    {
        public OrderMailModel()
        {
            Items = new List<OrderItem>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        public List<OrderItem> Items { get; set; }
        public string Text { get; set; }
    }

    public class OrderItem
    {
        public string Title { get; set; }

        public int Count { get; set; }

        public decimal UnitPrice { get; set; }

        public string Note { get; set; }
    }
}