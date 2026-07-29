using CubeServer.Data;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CubeServer.Features.ReportSignature
{
    public sealed class FileReportSignatureStore : IReportSignatureStore
    {
        const long MaxSignatureSizeBytes = 2 * 1024 * 1024;

        string GetDirectoryPath()
        {
            string dir = Path.Combine(Global.rootPath, "report-signatures");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        string Sanitize(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "user";
            }

            char[] chars = userId
                .Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_')
                .ToArray();

            string safe = new string(chars);
            safe = safe.Trim('_');
            return string.IsNullOrWhiteSpace(safe) ? "user" : safe;
        }

        string GetExtFromContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return ".png";
            contentType = contentType.Trim().ToLowerInvariant();
            return contentType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                _ => ".png"
            };
        }

        string GetContentTypeFromExt(string ext)
        {
            if (string.IsNullOrWhiteSpace(ext)) return "image/png";
            ext = ext.Trim().ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/png"
            };
        }

        string FindExistingFile(string dir, string baseName)
        {
            return Directory.EnumerateFiles(dir, baseName + ".*").FirstOrDefault();
        }

        public Task<Uri> GetSignatureFileUriAsync(string userId, CancellationToken cancellationToken = default)
        {
            string dir = GetDirectoryPath();
            string baseName = Sanitize(userId);
            string file = FindExistingFile(dir, baseName);
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                return Task.FromResult<Uri>(null);
            }

            return Task.FromResult(new Uri(file));
        }

        public async Task<string> GetSignatureDataUrlAsync(string userId, CancellationToken cancellationToken = default)
        {
            Uri uri = await GetSignatureFileUriAsync(userId, cancellationToken);
            if (uri == null)
            {
                return null;
            }

            string path = uri.LocalPath;
            if (!File.Exists(path))
            {
                return null;
            }

            byte[] bytes = await File.ReadAllBytesAsync(path, cancellationToken);
            string ext = Path.GetExtension(path);
            string ct = GetContentTypeFromExt(ext);
            string b64 = Convert.ToBase64String(bytes);
            return $"data:{ct};base64,{b64}";
        }

        public async Task SaveAsync(string userId, IBrowserFile file, CancellationToken cancellationToken = default)
        {
            if (file == null) return;
            if (file.Size <= 0) return;
            if (file.Size > MaxSignatureSizeBytes)
            {
                throw new InvalidOperationException($"Signature image must be <= {MaxSignatureSizeBytes} bytes.");
            }

            string dir = GetDirectoryPath();
            string baseName = Sanitize(userId);
            foreach (string existing in Directory.EnumerateFiles(dir, baseName + ".*"))
            {
                try { File.Delete(existing); } catch { }
            }

            string ext = GetExtFromContentType(file.ContentType);
            string dest = Path.Combine(dir, baseName + ext);

            await using Stream read = file.OpenReadStream(MaxSignatureSizeBytes, cancellationToken);
            await using FileStream write = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None);
            await read.CopyToAsync(write, cancellationToken);
        }

        public Task DeleteAsync(string userId, CancellationToken cancellationToken = default)
        {
            string dir = GetDirectoryPath();
            string baseName = Sanitize(userId);
            foreach (string existing in Directory.EnumerateFiles(dir, baseName + ".*"))
            {
                try { File.Delete(existing); } catch { }
            }
            return Task.CompletedTask;
        }
    }
}
