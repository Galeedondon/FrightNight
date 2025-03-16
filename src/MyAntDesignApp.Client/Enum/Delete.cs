using System.ComponentModel;
using System.Reflection;

namespace MyAntDesignApp.Client.Enum
{
    public class Delete
    {
        public enum PostDeletionWarning
        {
            [Description("這篇貼文承載著許多美好的回憶和情感。刪除它，意味著刪除那些珍貴的瞬間。讓我們一起珍惜這些回憶，保留這些美好！所以你確定要刪除嗎？")]
            FirstWarning = 1,

            [Description("每一篇貼文都是我們生活的一部分，刪除它會讓一些重要的時刻消失。你真的想要失去這些回憶嗎？所以你確定要刪除嗎？")]
            SecondWarning,

            [Description("這篇貼文記錄了你的心聲和故事。刪除它，意味著永遠失去這些珍貴的文字。所以你確定要刪除嗎？")]
            ThirdWarning
        }
    }
}