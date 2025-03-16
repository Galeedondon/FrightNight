using System.ComponentModel.DataAnnotations;

namespace MyAntDesignApp.Client.Models
{
    public class responseModel
    {
        public class Memorys
        {
            public int Id { get; set; }
            [Required]
            public string Title { get; set; }
            public string PicKey { get; set; }
            [Required]
            public DateTime MemoryDate { get; set; } = DateTime.Now;
            public string Tags { get; set; }
            public string Locate { get; set; }
            public string CreateAt { get; set; }
            public DateTime CreateDate { get; set; }
            public string? UpdateAt { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string? DeleteAt { get; set; }
            public DateTime? DeleteDate { get; set; }
        }
        public class Comments
        {
            public int Id { get; set; }
            public int? ReplyId { get; set; }
            public int MemoryId { get; set; }
            public string Comment { get; set; }
            public string CreateAt { get; set; }
            public DateTime CreateDate { get; set; }
            public string? UpdateAt { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string? DeleteAt { get; set; }
            public DateTime? DeleteDate { get; set; }
        }

    }
}