namespace MyAntDesignApp.Client.Models
{
    public class Common
    {
        public class ApiRes<T>
        {
            public bool? Result { get; set; } = null;
            public string? Msg { get; set; }
            public T? Data { get; set; }
        }
    }
}