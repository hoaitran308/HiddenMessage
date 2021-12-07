using HiddenMessage.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace HiddenMessage.Pages
{
    public partial class Show : ComponentBase
    {
        private string imageEncodedURL;
        private string hiddenMessage;
        private string password = "";
        private Bitmap imageBitmap;

        [Inject] private DESService DESService { get; set; }
        [Inject] private ImageService ImageService { get; set; }
        [Inject] private SteganographyService SteganographyService { get; set; }

        private async Task OnFileChangedAsync(InputFileChangeEventArgs e)
        {
            if (!e.File.ContentType.Contains("image/"))
            {
                return;
            }

            imageBitmap = await ImageService.GetBitmapAsync(e.File);
            imageEncodedURL = ImageService.GetImageUri(imageBitmap);
        }

        private void OnClickGetHiddenMessage()
        {
            if (imageBitmap == null)
            {
                return;
            }

            string messageEncrypted = SteganographyService.LSBDecode(imageBitmap);
            hiddenMessage = DESService.Decrypt(messageEncrypted, password);
        }
    }
}
