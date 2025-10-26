using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlazorLab.Services
{
    public class VideoFileService : IVideoFileService
    {
        private static readonly string[] VideoExtensions = [".mp4", ".webm", ".ogg"];
        private readonly string _downloadRoot;

        public VideoFileService(IOptions<DownloaderOptions> options)
        {
            _downloadRoot = Path.Combine(AppContext.BaseDirectory, options.Value.DownloadRoot);
        }

        public List<string> GetVideoFiles()
        {            
            if (!Directory.Exists(_downloadRoot))
            {
                return new List<string>();
            }

            return Directory.GetFiles(_downloadRoot)
                .Where(f => VideoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderByDescending(File.GetCreationTimeUtc)
                .ToList();
        }
    }
}
