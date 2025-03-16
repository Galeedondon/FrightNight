using AntDesign;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace MyAntDesignApp.Models
{

    public class Data
    {
        public int Id {get; set;}
        public string Author { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public string Locate { get; set; }
        public string PicId { get; set; }
        public DateTime CreateDate { get; set; }
        public List<DetailData>? DetailData { get; set; }
        public List<RenderFragment> Actions;
        public RenderFragment? Datetime;
    }

    public class DetailData
    {
        public int Id {get; set;}
        public string Author { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public string Locate { get; set; }
        public string PicId { get; set; }
        public DateTime CreateDate { get; set; }
        public List<RenderFragment> Actions;
        public RenderFragment? Datetime;
    }
    public class CalendarData
    {
        public BadgeStatus Type { get; set; }
        public string Content { get; set; }
        public DateTime MemoryDate { get; set; }

    }

    public class Auth
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("validDate")]
        public DateTime? ValidDate { get; set; }
    }

    
}