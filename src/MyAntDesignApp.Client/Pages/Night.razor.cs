using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MyAntDesignApp.CommonService;
using MyAntDesignApp.Models;
using Newtonsoft.Json;
using static MyAntDesignApp.Client.Models.Common;
using static MyAntDesignApp.Client.Models.responseModel;


namespace MyAntDesignApp.Pages.Components
{
    /// <summary>
    /// Adds custom Request and Response examples.
    /// You should install it using the .AddSwaggerExamples() extension method
    /// </summary>
    public partial class Night
    {
        [Inject] IJSRuntime Js { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] ApiVisitService ApiVisitService { get; set; }
        [Inject] IMessageService Message { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
        public bool initialLoading = true;
        public List<Data> memorys = new();
        public string userId = "";
        List<CalendarData> CalendarData = new();
        private string backgroundImage;
        private string glassImage;
        private bool morning = false;
        protected override async Task OnInitializedAsync()
        {

        }
        protected override void OnInitialized()
        {
            var currentTime = DateTime.Now;
            int hour = currentTime.Hour;
            // 根據當前時間設置背景圖片
            if (hour >= 17 || hour < 5) // 下午5點到早上5點
            {
                morning = false;
                backgroundImage = "/img/night.jpg";
                glassImage = "/img/nightglass.png";
            }
            else
            {
                morning = true;
                backgroundImage = "/img/morning.jpg";
                glassImage = "/img/sunglass1.png";
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var res = await ApiVisitService.Vlidation();
                if (res.Result == false)
                {
                    await Message.Error(res.Msg);
                    Navigation.NavigateTo("/");
                    await Js.InvokeVoidAsync("location.reload");
                }
                userId = await ApiVisitService.GetCurrentUser();
                await ProcessDate(DateTime.Now);
                await Js.InvokeVoidAsync("init");
                initialLoading = false;
                StateHasChanged();
            }

        }


        public async Task ProcessDate(DateTime date)
        {
            memorys = new();
            CalendarData = new();
            initialLoading = true;
            StateHasChanged();

            await Task.Delay(500);
            var resMomory = await ApiVisitService.GetAsync<List<Memorys>>($"Memory/GetByDate/{date.ToString("yyyy-MM-dd")}");

            memorys.AddRange(resMomory.Data.Select(item => new Data
            {
                Id = item.Id,
                CreateDate = item.CreateDate,
                Locate = item.Locate,
                PicId = item.PicKey,
                Author = item.CreateAt,
                Avatar = $"/img/avatar/{item.CreateAt}.png",
                Content = item.Title,
                Actions = new List<RenderFragment>()
            }));

            resMomory = await ApiVisitService.GetAsync<List<Memorys>>($"Memory/GetByMonth/{date.ToString("yyyy-MM-dd")}");
            Dictionary<string, BadgeStatus> authorStatusMapping = new Dictionary<string, BadgeStatus>
                {
                    { "Chu", BadgeStatus.Error },
                    { "Cindy", BadgeStatus.Error },
                    { "Eileen", BadgeStatus.Error },
                    { "Jane", BadgeStatus.Error },
                    { "Rona", BadgeStatus.Error },
                    { "Sandy", BadgeStatus.Error },
                    { "Sharon", BadgeStatus.Error },

                    { "Andy", BadgeStatus.Success },
                    { "Jerry", BadgeStatus.Success },
                    { "Ray", BadgeStatus.Success },
                    { "Rocky", BadgeStatus.Success },
                };

            CalendarData.AddRange(resMomory.Data.Select(item => new CalendarData
            {
                Type = authorStatusMapping.ContainsKey(item.CreateAt) ? authorStatusMapping[item.CreateAt] : BadgeStatus.Warning,
                Content = item.Title,
                MemoryDate = item.MemoryDate
            }));
            initialLoading = false;
            StateHasChanged();
        }

        #region Canlander
        public DateTime selectedValue = new DateTime(2025, 1, 30);
        private DateTime value = new DateTime(2025, 1, 30);

        public async void OnSelect(DateTime value)
        {
            this.value = value;
            await Js.InvokeVoidAsync("updateCalendarHeader", value.Year, value.Month);
            await ProcessDate(value);
            selectedValue = value;
            StateHasChanged();
        }


        private void OnPanelChange(DateTime value, DatePickerType type)
        {
            this.value = value;
            Console.WriteLine($"{value.ToString("YYYY-MM-DD")} {type}");
        }

        public List<CalendarData> GetListData(DateTime value)
        {
            var list = CalendarData
                      .Where(data => data.MemoryDate.Day == value.Day && data.MemoryDate.Month == value.Month)
                      .ToList();

            return list ?? new List<CalendarData>();
        }

        private RenderFragment DateCellRender(DateTime value)
        {
            var calendarData = GetListData(value);

            return builder =>
            {
                builder.OpenElement(0, "ul");
                builder.AddAttribute(1, "class", "events");

                foreach (var data in calendarData)
                {
                    builder.OpenElement(2, "li");
                    builder.AddAttribute(3, "key", data.Content);

                    builder.OpenComponent<Badge>(4);
                    builder.AddAttribute(5, "Status", data.Type);
                    builder.AddAttribute(6, "Text", data.Content);
                    builder.CloseComponent();

                    builder.CloseElement();  // </li>
                }

                builder.CloseElement();  // </ul>
            };
        }

        private int? GetMonthData(DateTime value)
        {
            if (value.Month == 8)
            {
                return 1394;
            }

            return null;
        }
        #endregion

        #region Add event panel

        bool _visible = false;
        bool _submitting = false;
        private Form<Memorys> _form;

        private Memorys model = new Memorys();

        private void ShowModal()
        {
            _visible = true;
        }

        /// <summary>
        /// on modal OK button is click, submit form manually
        /// </summary>
        /// <param name="e"></param>
        private async Task HandleOk(MouseEventArgs e)
        {
            _submitting = true;

            await Task.Delay(1000);
            _form.Validate();
            if (_form.Validate() != false)
            {
                _form.Submit();
            }
            _submitting = false;

        }

        private void HandleCancel(MouseEventArgs e)
        {
            Console.WriteLine(e);
            _visible = false;
        }

        /// <summary>
        /// when form is submited, close the modal
        /// </summary>
        /// <param name="args"></param>
        private async void OnFinish(EditContext editContext)
        {
            if (_form.Validate() == false)
            {
                return;
            }
            var user = await ApiVisitService.GetProfile();
            model.CreateAt = user.UserId;
            model.CreateDate = DateTime.Now;
            var resMomory = await ApiVisitService.QueryCommon<Memorys, Memorys>($"Memory", model);
            await Message.Success("又多一塊回憶碎片囉～");
            _commentId = null;
            _submitting = false;
            _visible = false;
            await ProcessDate(model.MemoryDate);
            model = new();
            StateHasChanged();
        }

        private void OnFinishFailed(EditContext editContext)
        {
            Console.WriteLine(_form.Validate());
            Console.WriteLine($"Failed:{System.Text.Json.JsonSerializer.Serialize(model)}");
        }

        // process img to base64
        private IBrowserFile selectedFile;
        private string base64Image;


        private async Task ConvertToBase64()
        {
            if (selectedFile != null)
            {
                using var memoryStream = new MemoryStream();
                await selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                base64Image = Convert.ToBase64String(fileBytes);
            }
        }

        void OnSingleCompleted(UploadInfo fileinfo)
        {
            if (fileinfo.File.State == UploadState.Success)
            {
                var result = fileinfo.File.GetResponse<ApiRes<string>>();
                model.PicKey = result.Data;
            }
        }

        #endregion

        #region CommentEvet
        bool _visibleComment = false;
        bool submitting = false;
        string _value = "";
        string _title = "";
        string _tags = "";
        string _imgUrl = "";
        int? _memoryId = null;
        DateTime _createDate = new();
        bool loading = false;
        private void HandleCommentCancel(MouseEventArgs e)
        {

            _visibleComment = false;
        }
        #endregion

        #region img

        bool BeforeUpload(UploadFileItem file)
        {
            var isJpgOrPng = file.Type == "image/jpeg" || file.Type == "image/png";
            if (!isJpgOrPng)
            {
                Message.Error("You can only upload JPG/PNG file!");
            }
            var isLt2M = file.Size / 1024 / 1024 < 2;
            if (!isLt2M)
            {
                Message.Error("Image must smaller than 2MB!");
            }
            return isJpgOrPng && isLt2M;
        }

        string imageUrl;

        void HandleChange(UploadInfo fileinfo)
        {
            loading = fileinfo.File.State == UploadState.Uploading;

            if (fileinfo.File.State == UploadState.Success)
            {
                imageUrl = fileinfo.File.ObjectURL;
            }
            InvokeAsync(StateHasChanged);
        }

        #endregion
        #region Click event panel
        public async Task OnShowEvent(Data context)
        {
            initialLoading = true;
            StateHasChanged();

            await Task.Delay(500); // 等待 500 毫秒
            _visibleComment = true;
            _title = context.Content;
            _createDate = context.CreateDate;
            _tags = context.Locate;
            _memoryId = context.Id;
            var resImg = await ApiVisitService.GetAsync<string>($"Memory/Picture/{context.PicId}");
            _imgUrl = resImg.Data;

            var resCommont = await ApiVisitService.GetAsync<List<Comments>>($"Comment/{context.Id}");
            await ProcessComment(resCommont.Data);

            initialLoading = false;
            StateHasChanged();

        }

        #region Comments

        int? _commentId = null;
        public List<Data> comments = new();

        public async Task ProcessComment(List<Comments> datas)
        {
            comments = new();
            foreach (var item in datas)
            {
                // reply comment pass.
                if (item.ReplyId == null)
                {
                    // if reply, add reply comment in comment
                    var replies = datas.Where(c => c.ReplyId == item.Id).OrderBy(c => c.CreateDate).ToList(); // 找到所有回復該留言的
                    List<DetailData> detailData = new List<DetailData>();

                    foreach (var reply in replies)
                    {
                        detailData.Add(new DetailData
                        {
                            Author = "⬑ " + reply.CreateAt,
                            Avatar = $"/img/avatar/{reply.CreateAt}.png",
                            Content = reply.Comment,
                            Datetime = GenerateDatetimeFragment(reply)
                        });
                    }

                    // add comment
                    comments.Add(new Data
                    {
                        Id = item.Id,
                        Author = item.CreateAt,
                        Avatar = $"/img/avatar/{item.CreateAt}.png",
                        Content = item.Comment,
                        CreateDate = item.CreateDate,
                        DetailData = detailData,  // 添加回復
                        Actions = new List<RenderFragment>() { replyAction(item.Id) },
                        Datetime = GenerateDatetimeFragment(item)
                    });
                }
            }
            comments = comments.OrderBy(c => c.CreateDate).ToList();
            await InvokeAsync(StateHasChanged);

        }
        private RenderFragment GenerateDatetimeFragment(Comments data) => builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddContent(1, MomentHelper.FromNow(data.CreateDate));
            builder.CloseElement();

            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "style", "float: right");
            if (data.CreateAt == userId)
            {
                builder.OpenElement(4, "a");
                builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => EditComment(data)));

                builder.AddContent(6, "Edit");
                builder.CloseElement(); // 關閉 Edit 按鈕

                // 添加分隔符
                builder.AddContent(7, " | "); // 使用 AddContent 添加分隔符

                builder.OpenElement(8, "a");
                builder.AddAttribute(9, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => DeleteComment(data)));
                builder.AddContent(10, "Delete");
                builder.CloseElement(); // 關閉 Delete 按鈕
            }


            builder.CloseElement(); // 關閉包含按鈕的 div
        };

        private RenderFragment replyAction(int index) => builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "id", $"reply-btn-{index}");
            builder.AddAttribute(2, "class", "comment-action");
            builder.AddAttribute(3, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, e => HighlightReply(index)));

            builder.AddContent(4, "Reply to");

            builder.CloseElement();
        };
        private async Task HighlightReply(int index)
        {
            _commentId = index;
            await Js.InvokeVoidAsync("highlightReply", $"reply-btn-{index}");
            await InvokeAsync(StateHasChanged);
        }

        private RenderFragment header => builder =>
        {
            builder.OpenElement(0, "div");
            if (memorys.Any())
            {
                builder.OpenElement(1, "span");
                builder.AddContent(2, $"{comments.Count} {(comments.Count > 1 ? "replies" : "reply")}");
                builder.CloseElement(); // span
            }
            builder.CloseElement(); // div
        };

        private async Task onSubmit()
        {
            submitting = true;
            if (string.IsNullOrEmpty(_value))
            {
                await Message.Error("你比手語嗎?");
                submitting = false;
                return;
            }
            await Task.Delay(1000);
            var newComment = new Comments();
            newComment.ReplyId = _commentId;
            newComment.MemoryId = (int)_memoryId;
            newComment.Comment = _value;
            newComment.CreateAt = userId;
            newComment.CreateDate = DateTime.Now;


            var resImg = await ApiVisitService.QueryCommon<Comments, Comments>($"Comment", newComment);
            // 重新獲取留言
            var resCommont = await ApiVisitService.GetAsync<List<Comments>>($"Comment/{_memoryId}");
            await Js.InvokeVoidAsync("highlightReply", $"reply-btn-{_commentId}");
            _commentId = null;
            await ProcessComment(resCommont.Data);
            // comments.Add(new Data()
            // {
            //     Author = userId,
            //     Avatar = $"/img/avatar/{userId}.png",
            //     Content = _value,
            //     Datetime = GenerateDatetimeFragment(DateTime.Now),
            //     Actions = new List<RenderFragment>() { replyAction(0) }
            // });

            submitting = false;
            _value = "";

            await InvokeAsync(StateHasChanged);
        }

        string _commentModal = "";
        string _commentModalOK = "";
        string _commentEdit = "";
        bool _commentVisible = false;
        private Comments adjustComment = new();
        private async void EditComment(Comments context)
        {
            _commentVisible = true;
            _commentModal = "Edit comment";
            _commentModalOK = "Edit";
            _commentEdit = context.Comment;
            adjustComment = context;
        }

        // 刪除按鈕事件處理
        private async void DeleteComment(Comments context)
        {
            _commentVisible = true;
            _commentModal = "Delete comment";
            _commentModalOK = "Delete";
            adjustComment = context;
        }

        private async void CommentOk()
        {
            try
            {
                if (_commentModalOK == "Edit")
                {
                    adjustComment.Comment = _commentEdit;
                    adjustComment.UpdateAt = userId;
                    adjustComment.UpdateDate = DateTime.Now;
                    var updateComment = await ApiVisitService.PutAsync<Comments, Comments>($"Comment", adjustComment);
                }
                else
                {
                    var updateComment = ApiVisitService.DeleteAsync($"Comment/{adjustComment.Id}/{userId}"); ;
                }
            }
            catch (Exception ex)
            {
                await Message.Error($"留言失敗，請稍後再試！");
            }
            finally
            {
                // 重新獲取留言
                var resCommont = await ApiVisitService.GetAsync<List<Comments>>($"Comment/{_memoryId}");
                await ProcessComment(resCommont.Data);
                _commentId = null;
                _commentVisible = false;

                await Message.Success($"{_commentModal} success.");
            }
        }
        private void CommentCancel()
        {
            _commentVisible = false;
        }

        public RenderFragment[] actions = new[] { edit, add };
        static RenderFragment edit = builder =>
        {
            builder.OpenElement(0, "a");
            builder.AddAttribute(1, "key", "list-loadmore-edit");
            builder.AddContent(2, "edit");  // 設置顯示文字為 "edit"
            builder.CloseElement();
        };

        static RenderFragment add = builder =>
        {
            builder.OpenElement(0, "a");
            builder.AddAttribute(1, "key", "list-loadmore-more");
            builder.AddContent(2, "more");  // 設置顯示文字為 "more"
            builder.CloseElement();
        };
        #endregion

        #endregion
    }
}