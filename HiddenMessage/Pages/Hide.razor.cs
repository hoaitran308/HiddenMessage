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
        private string message;
        private string password;
        private string messageEncrypted = "";
        private Bitmap imageBitmap;
        private int? messageAcceptLength;

        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private DESService DESService { get; set; }
        [Inject] private ImageService ImageService { get; set; }
        [Inject] private SteganographyService SteganographyService { get; set; }

        private async Task OnFileChangedAsync(InputFileChangeEventArgs e)
        {
            if (!e.File.ContentType.Contains("image/"))
            {
                messageAcceptLength = 0;
                return;
            }

            imageBitmap = await ImageService.GetBitmapAsync(e.File);
            messageAcceptLength = (imageBitmap.Height * imageBitmap.Width - 32) / 8;
            imageOriginalURL = ImageService.GetImageUri(imageBitmap);
        }

        private void OnClickHideMessage()
        {
            messageEncrypted = DESService.Encrypt(message, password);

            if (messageEncrypted.Length > messageAcceptLength
                || string.IsNullOrEmpty(password))
            {
                return;
            }

            imageBitmap = SteganographyService.LSBEncode(imageBitmap, messageEncrypted);
            imageEncodedURL = ImageService.GetImageUri(imageBitmap);
        }

        private async Task OnClickDownloadAsync()
        {
            string fileName = @$"{DateTime.Now.ToFileTime()}.jpg";

            MemoryStream ms = new();
            imageBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            await JSRuntime.InvokeVoidAsync(
                "jsSaveAsFile",
                fileName,
                Convert.ToBase64String(ms.ToArray())
            );
        }
    }
}
