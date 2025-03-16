using System.ComponentModel.DataAnnotations;
using AntDesign;
using Microsoft.AspNetCore.Components;

namespace MyAntDesignApp.Models
{

    public class Model
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime PostDate { get; set; }
        public string Tag { get; set; }
        public string Locate { get; set; }
    }

    public class ResponseModel
    {
        public string name { get; set; }

        public string status { get; set; }

        public string url { get; set; }

        public string thumbUrl { get; set; }
    }
}