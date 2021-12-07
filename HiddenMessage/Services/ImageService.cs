using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace HiddenMessage.Services
{
    public class ImageService
    {
        private const int MaxInputFile = 10 * 1024 * 1024;

        public async Task<Bitmap> GetBitmapAsync(IBrowserFile file)
        {
            using Stream stream = file.OpenReadStream(MaxInputFile);

            MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream);

            return new Bitmap(memoryStream);
        }

        public string GetImageUri(Bitmap bitmap)
        {
            MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return $"data:image/jpeg;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
        }
    }
}
