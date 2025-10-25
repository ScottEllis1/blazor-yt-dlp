namespace BlazorLab.Services
{
    public interface IDownloadService
    {
        Task<DownloadResult> DownloadAsync(string url, CancellationToken ct = default);
    }

    public sealed record DownloadResult(int ExitCode, string StdOut, string StdErr, bool FileDownloaded);
}
