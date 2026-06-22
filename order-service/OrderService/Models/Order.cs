using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Created";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }=new HashSet<OrderItem>();
    }
}
