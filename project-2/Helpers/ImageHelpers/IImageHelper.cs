namespace project_2.Helpers.ImageHelpers
{
    public interface IImageHelper
    {
        String SaveImages(IFormFile File, string OldName, string path);
    }
}
