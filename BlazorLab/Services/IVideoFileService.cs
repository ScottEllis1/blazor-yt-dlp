using System.Collections.Generic;

namespace BlazorLab.Services
{
    public interface IVideoFileService
    {
        List<string> GetVideoFiles();
    }
}
