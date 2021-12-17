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
        private string passwordEncrypted = "";
        private Bitmap imageBitmap;
        private int? messageAcceptLength;

        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private CryptographyService CryptographyService { get; set; }
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
            messageAcceptLength = 3 * (imageBitmap.Height * imageBitmap.Width - 11) / 8;
            imageOriginalURL = ImageService.GetImageUri(imageBitmap);
        }

        private void OnClickHideMessage()
        {
            passwordEncrypted = CryptographyService.MD5Hash(password);
            messageEncrypted = CryptographyService.TripleDESEncrypt(message, passwordEncrypted);

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

            byte[] bytes = ImageService.GetImageBytes(imageBitmap);

            await JSRuntime.InvokeVoidAsync(
                "jsSaveAsFile",
                fileName,
                Convert.ToBase64String(bytes)
            );
        }
    }
}
