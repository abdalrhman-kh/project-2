
namespace project_2.Helpers.ImageHelpers
{
    public class ImageHelper : IImageHelper
    {
        IWebHostEnvironment env;

        public ImageHelper(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public string SaveImages(IFormFile File, string OldName, string path)
        {
            var ImageName = "";
            if (File != null) {
                var TempName = Guid.NewGuid() + "_" + File.FileName;
                ImageName = Path.Combine(path, TempName);
                var FullPath = Path.Combine(env.WebRootPath, ImageName);
                using(var stream = new FileStream(FullPath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }
            }
            else
            {
                ImageName = OldName;
            }
            return ImageName;
        }
    }
}
