namespace BlazorLab.Services
{
    public class DownloadEventNotifier
    {
        public event Action? VideoDownloaded;

        public void NotifyVideoDownloaded()
        {
            VideoDownloaded?.Invoke();
        }
    }
}
