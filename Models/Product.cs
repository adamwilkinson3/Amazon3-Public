using System.ComponentModel.DataAnnotations;

namespace Amazon3.Models
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public string Url { get; set; }
    }
}
