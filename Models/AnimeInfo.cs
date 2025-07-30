using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeApi.Models
{
    [Table("AnimeInfo")]
    public class AnimeInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        public string AnimeName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public byte[] Image { get; set; }
        public int UserId { get; set; } // ✅ ต้องมี
    }
}
