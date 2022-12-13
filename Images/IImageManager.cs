namespace MonumentService.Images
{
    using MonumentService.Model;

    public interface IImageManager
    {
        void Start();

        Task<bool> SaveImageForMonument(Monument monument);

        byte[]? GetImageForMonument(int id);
    }
}
