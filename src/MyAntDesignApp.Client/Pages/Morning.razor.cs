using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MyAntDesignApp.Models;
using static MyAntDesignApp.Client.Enum.Delete;


namespace MyAntDesignApp.Pages.Components
{
    /// <summary>
    /// Adds custom Request and Response examples.
    /// You should install it using the .AddSwaggerExamples() extension method
    /// </summary>
    public partial class Morning
    {
        [Inject] IJSRuntime Js { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [Inject] DrawerService DrawerService { get; set; }
        [Inject] IConfirmService ConfirmService { get; set; }
        private string backgroundImage;
        private string glassImage;
        private bool morning;
        protected override void OnInitialized()
        {
            var currentTime = DateTime.Now;
            int hour = currentTime.Hour;

            // if (hour >= 17 || hour < 5) // 下午5點到早上6點
            // {
            //     Navigation.NavigateTo("/night");
            // }

            // 根據當前時間設置背景圖片
            if (hour >= 17 || hour < 5) // 下午5點到早上5點
            {
                morning = false;
                backgroundImage = "/img/night.jpg";
                glassImage = "/img/nightglass.png";
            }
            else
            {
                morning  = true;
                backgroundImage = "/img/morning.jpg";
                glassImage = "/img/sunglass1.png";

            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Js.InvokeVoidAsync("init");
            }
        }

        #region Canlander
        public DateTime selectedValue = new DateTime(2025, 1, 30);
        private DateTime value = new DateTime(2025, 1, 30);

        public void OnSelect(DateTime value)
        {
            this.value = value;
            selectedValue = value;
            StateHasChanged();
        }


        private void OnPanelChange(DateTime value, DatePickerType type)
        {
            this.value = value;
            Console.WriteLine($"{value.ToString("YYYY-MM-DD")} {type}");
        }
        // Event
        public List<Data> datas => new List<Data>()
        {
            new Data()
            {
            Author = "Chu",
            Avatar = @"/img/avatar/Chu.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Eileen",
            Avatar = @"/img/avatar/Eileen.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Cindy",
            Avatar = @"/img/avatar/Cindy.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Andy",
            Avatar = @"/img/avatar/Andy.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Jane",
            Avatar = @"/img/avatar/Jane.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Ray",
            Avatar = @"/img/avatar/Ray.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Rocky",
            Avatar = @"/img/avatar/Rocky.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Sandy",
            Avatar = @"/img/avatar/Sandy.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Rona",
            Avatar = @"/img/avatar/Rona.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            },
            new Data()
            {
            Author = "Sharon",
            Avatar = @"/img/avatar/Sharon.png",
            Content = "最多十個字最多十個字",
            Actions = new List<RenderFragment>() {}
            }
        };

        public List<CalendarData> GetListData(DateTime value)
        {
            List<CalendarData> CalendarData = null;


            return CalendarData ?? new List<CalendarData>();
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
        private Form<Model> _form;

        private Model model = new Model();

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
            else
            {
                _submitting = false;
            }

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
        private void OnFinish(EditContext editContext)
        {
            if (_form.Validate() == false)
            {
                return;
            }
            Console.WriteLine(_form.Validate());
            _submitting = false;
            _visible = false;
        }

        private void OnFinishFailed(EditContext editContext)
        {
            Console.WriteLine(_form.Validate());
            Console.WriteLine($"Failed:{JsonSerializer.Serialize(model)}");
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
                var result = fileinfo.File.GetResponse<ResponseModel>();
                fileinfo.File.Url = result.url;
            }
        }

        #endregion

        #region CommentEvet
        bool _visibleComment = false;
        bool submitting = false;
        string _value = "";
        string _title = "";
        bool loading = false;
        private void HandleCommentCancel(MouseEventArgs e)
        {
            _visibleComment = false;
        }
        private async Task HandlePostDeleteAsync()
        {
            Random random = new Random();
            Array values = Enum.GetValues(typeof(PostDeletionWarning));
            PostDeletionWarning randomWarning = (PostDeletionWarning)values.GetValue(random.Next(values.Length));

            var message = randomWarning.GetDescription();

            var confirmResult = await ConfirmService.Show(message, "請不要讓回憶消失", ConfirmButtons.YesNo);
            if (confirmResult == ConfirmResult.Yes)
            {
                // tode delete API -> fake delete.
                StateHasChanged();
            }
        }
        #endregion
        #region Click event panel
        public async Task OnShowEvent(Data context)
        {
            loading = true;
            await Task.Delay(500); // 等待 500 毫秒
            _visibleComment = true;
            _title = context.Content;
            loading = false;

        }


        #endregion
    }
}