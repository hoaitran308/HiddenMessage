using System;
using System.IO;
using System.Drawing;
using Microsoft.JSInterop;
using HiddenMessage.Services;
using System.Threading.Tasks;
using HiddenMessage.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HiddenMessage.Pages
{
    public partial class Hide : ComponentBase
    {
        private string imageOriginalURL;
        private string imageEncodedURL;
        private string message = "";
        private Bitmap imageBitmap;
        private int? messageAcceptLength;

        [Inject] private SteganographyService SteganographyService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        private async Task OnFileChangedAsync(InputFileChangeEventArgs e)
        {
            if (!e.File.ContentType.Contains("image/"))
            {
                messageAcceptLength = 0;
                return;
            }

            imageBitmap = await SteganographyService.GetBitmapAsync(e.File);
            imageOriginalURL = SteganographyService.GetImageUri(imageBitmap);
            messageAcceptLength = imageBitmap.MessageLength();
        }

        private void OnClickHideMessage()
        {
            if (message.Length > messageAcceptLength.Value)
            {
                return;
            }

            imageBitmap = SteganographyService.LSBEncode(imageBitmap, message);
            imageEncodedURL = SteganographyService.GetImageUri(imageBitmap);
        }

        private async Task OnClickDownloadAsync()
        {
            string fileName = @$"{DateTime.Now.ToFileTime()}.jpg";

            MemoryStream ms = new();
            imageBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            await JSRuntime.InvokeVoidAsync("jsSaveAsFile",
                        fileName,
                        Convert.ToBase64String(ms.ToArray())
                        );
        }
    }
}
