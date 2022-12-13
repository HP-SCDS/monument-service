namespace MonumentService.Images
{
    using MonumentService.Model;
    using System.Reflection;
    using System.Reflection.Metadata;

    public class ImageManager : IImageManager
    {
        private static readonly string ImagesDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? ".", "images");
        private static readonly Func<int, string> ImagesUrl = idBien => $"https://servicios.jcyl.es/pweb/downloadFoto.do?numbien={idBien}";

        private readonly HttpClient m_client = new HttpClient();

        private readonly ILogger m_logger;

        public void Start()
        {
            if (!Directory.Exists(ImagesDirectory))
            {
                Directory.CreateDirectory(ImagesDirectory);
            }
            
            m_logger.LogInformation($"Images directory initialized at {ImagesDirectory}");
        }

        public ImageManager(ILogger<ImageManager> logger)
        {
            m_logger = logger;
        }

        public async Task<bool> SaveImageForMonument(Monument monument)
        {
            if (monument.IdBienCultural == null || monument.IdBienCultural == 0)
            {
                return false;
            }

            string imageName = $"{monument.Id}.jpg";
            string imagePath = Path.Combine(ImagesDirectory, imageName);
            if (File.Exists(imagePath))
            {
                return false;
            }

            try
            {
                HttpResponseMessage response = await m_client.GetAsync(ImagesUrl((int)monument.IdBienCultural));
                if (response.IsSuccessStatusCode)
                {
                    // the service for images returns a 200 instead of a 404 when the image is not found
                    if (response.Content.Headers.ContentLength <= 0)
                    {
                        m_logger.LogDebug($"Image for monument {monument.Id} ({monument.IdBienCultural}) not found");
                        return false;
                    }

                    await response.Content.ReadAsStreamAsync().ContinueWith(async streamTask =>
                    {
                        using (Stream stream = await streamTask)
                        {
                            using (FileStream fileStream = File.Create(imagePath))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    });
                }
                else
                {
                    m_logger.LogDebug($"Failed to download image for monument {monument.Id} ({monument.IdBienCultural}) (maybe not found if they fixed their status codes)");
                    return false;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, $"Error getting image for monument {monument.Id} ({monument.IdBienCultural})");
                return false;
            }

            return true;
        }

        public byte[]? GetImageForMonument(int id)
        {
            string imageName = $"{id}.jpg";
            string imagePath = Path.Combine(ImagesDirectory, imageName);
            if (!File.Exists(imagePath))
            {
                return null;
            }

            return File.ReadAllBytes(imagePath);
        }
    }
}
