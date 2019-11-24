using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class Book
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string author { get; set; }
    }
}
