using System.ComponentModel.DataAnnotations;
namespace MyAntDesignApp.Client.Models;
public static class responseModel
{
    public class Memorys
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "請輸入回憶標題。"), StringLength(100)] public string Title { get; set; } = "";
        public string PicKey { get; set; } = "";
        public DateTime MemoryDate { get; set; } = DateTime.Today;
        [StringLength(200)] public string Tags { get; set; } = "";
        [StringLength(200)] public string Locate { get; set; } = "";
        public string CreateAt { get; set; } = "";
        public DateTime CreateDate { get; set; }
    }
    public class Comments
    {
        public int Id { get; set; }
        public int? ReplyId { get; set; }
        public int MemoryId { get; set; }
        public string Comment { get; set; } = "";
        public string CreateAt { get; set; } = "";
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
