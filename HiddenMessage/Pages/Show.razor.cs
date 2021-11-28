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
        private string hiddenMessage = "";
        private Bitmap imageBitmap;

        [Inject] private SteganographyService SteganographyService { get; set; }

        private async Task OnFileChangedAsync(InputFileChangeEventArgs e)
        {
            if (!e.File.ContentType.Contains("image/"))
            {
                return;
            }

            imageBitmap = await SteganographyService.GetBitmapAsync(e.File);
            imageEncodedURL = SteganographyService.GetImageUri(imageBitmap);
        }

        private void OnClickGetHiddenMessage()
        {
            if (imageBitmap == null)
            {
                return;
            }

            hiddenMessage = SteganographyService.LSBDecode(imageBitmap);
        }
    }
}
