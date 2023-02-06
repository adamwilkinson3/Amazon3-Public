using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amazon3.Models
{
    public class OrderItem
    {
        [Key]
        public string OrderId { get; set; }

        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
    }
}