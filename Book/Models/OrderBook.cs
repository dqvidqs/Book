using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Models
{
    public class OrderBook
    {
        [Key]
        public int id { get; set; }
        public virtual User user { get; set; }
        public virtual Book book { get; set; }
    }
    public class PostOrderBook
    {
        public int book_id { get; set; }
    }
}