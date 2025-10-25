namespace BlazorLab.Services
{
    using System.Diagnostics;
    using Microsoft.Extensions.Options;

    public sealed class DownloadService : IDownloadService
    {
        private readonly string _downloadRoot;
        private readonly DownloadEventNotifier _downloadNotifier;

        public DownloadService(IOptions<DownloaderOptions> options, DownloadEventNotifier notificationService)
        {
            string baseDir = AppContext.BaseDirectory;
            _downloadRoot = Path.GetFullPath(Path.Combine(baseDir, options.Value.DownloadRoot));
            Directory.CreateDirectory(_downloadRoot);
            _downloadNotifier = notificationService;
        }

        public async Task<DownloadResult> DownloadAsync(string url, CancellationToken ct = default)
        {
            // Quick URL check (in addition to Razor validation)
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return new DownloadResult(-1, "", "Invalid URL (must be http/https).", false);
            }

            // I don't really care about filenames, so I'm using the same template that is used in the archive file to determine uniqueness.
            string outputTemplate = Path.Combine(_downloadRoot, "%(extractor)s_%(id)s.%(ext)s");
            string archiveFile = Path.Combine(_downloadRoot, "archive.txt");

            string args = string.Join(' ',
            [
                EscapedQuotes(url),
                "-o", EscapedQuotes(outputTemplate),
                "--write-info-json",
                "--no-playlist",
                "--restrict-filenames",
                "--trim-filenames", "60",
                "--download-archive", EscapedQuotes(archiveFile)
            ]);

            ProcessStartInfo psi = new()
            {
                FileName = "yt-dlp", // Assumes yt-dlp is in PATH, and it will be when deployed in Docker
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new() { StartInfo = psi, EnableRaisingEvents = false };
            process.Start();

            Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
            Task<string> stderrTask = process.StandardError.ReadToEndAsync(ct);

            await process.WaitForExitAsync(ct).ConfigureAwait(false);

            string stdout = await stdoutTask.ConfigureAwait(false);
            string stderr = await stderrTask.ConfigureAwait(false);

            bool fileDownloaded = false;

            // Parse the destination file path from stdout. This is how we know if a file was actually downloaded.
            // yt-dlp doesn't error out if a file is already in the archive; it just skips it.
            foreach (var line in stdout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                const string marker = "[download] Destination: ";
                if (line.StartsWith(marker))
                {
                    fileDownloaded = true;
                    _downloadNotifier.NotifyVideoDownloaded();
                    break;
                }
            }

            return new DownloadResult(process.ExitCode, stdout, stderr, fileDownloaded);
        }

        private static string EscapedQuotes(string s) => $"\"{s}\"";
    }
}
