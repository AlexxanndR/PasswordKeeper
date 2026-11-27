using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using PasswordKeeper.Results;

namespace PasswordKeeper.Services.Icons;

public class IconStore
{
    private static readonly string[] AllowedExtensions =
    [
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".webp",
        ".ico",
    ];

    public const long MaxFileSizeBytes = 1024 * 1024;
    public const int MinImageDimension = 16;
    public const int MaxImageDimension = 512;

    public string IconsDirectory { get; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PasswordKeeper",
            "Icons"
        );

    public IReadOnlyList<string> Icons =>
        Directory.Exists(IconsDirectory)
            ? Directory
                .GetFiles(IconsDirectory)
                .Where(path =>
                    AllowedExtensions.Contains(
                        Path.GetExtension(path),
                        StringComparer.OrdinalIgnoreCase
                    )
                )
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList()
            : [];

    public async Task<IconImportResult> ImportAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return IconImportResult.Failure(
                $"Unsupported file type \"{extension}\". Allowed: {string.Join(", ", AllowedExtensions)}"
            );

        // Reject oversized files before buffering them when the stream length is known
        if (stream.CanSeek && stream.Length > MaxFileSizeBytes)
            return IconImportResult.Failure(TooLargeMessage(stream.Length));

        using var data = new MemoryStream();
        var buffer = ArrayPool<byte>.Shared.Rent(81920);
        try
        {
            int read;
            while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                if (data.Length + read > MaxFileSizeBytes)
                    return IconImportResult.Failure(TooLargeMessage(data.Length + read));

                data.Write(buffer, 0, read);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        if (data.Length == 0)
            return IconImportResult.Failure("File is empty");

        data.Position = 0;
        Bitmap bitmap;
        try
        {
            bitmap = new Bitmap(data);
        }
        catch (Exception)
        {
            return IconImportResult.Failure("File is not a valid image");
        }

        using (bitmap)
        {
            var size = bitmap.PixelSize;
            if (size.Width < MinImageDimension || size.Height < MinImageDimension)
                return IconImportResult.Failure(
                    $"Image is too small ({size.Width}x{size.Height}). Minimum is {MinImageDimension}x{MinImageDimension}"
                );
            if (size.Width > MaxImageDimension || size.Height > MaxImageDimension)
                return IconImportResult.Failure(
                    $"Image is too large ({size.Width}x{size.Height}). Maximum is {MaxImageDimension}x{MaxImageDimension}"
                );
        }

        Directory.CreateDirectory(IconsDirectory);
        var targetPath = GetUniquePath(
            Path.Combine(IconsDirectory, SanitizeFileName(fileName, extension))
        );

        data.Position = 0;
        await using var output = File.Create(targetPath);
        await data.CopyToAsync(output, cancellationToken);

        return IconImportResult.Success(
            targetPath,
            $"Icon \"{Path.GetFileName(targetPath)}\" imported"
        );
    }

    private static string TooLargeMessage(long bytes) =>
        $"File is too large ({bytes / 1024} KB). Maximum size is {MaxFileSizeBytes / 1024} KB";

    private static string SanitizeFileName(string fileName, string extension)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var name = new string([
            .. Path.GetFileName(fileName).Select(c => invalidChars.Contains(c) ? '_' : c),
        ]);
        return string.IsNullOrWhiteSpace(name) ? $"icon{extension}" : name;
    }

    private static string GetUniquePath(string path)
    {
        if (!File.Exists(path))
            return path;

        var directory = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var extension = Path.GetExtension(path);

        for (var i = 1; i <= 1000; i++)
        {
            var candidate = Path.Combine(directory, $"{name} ({i}){extension}");
            if (!File.Exists(candidate))
                return candidate;
        }

        return Path.Combine(directory, $"{name} ({Guid.NewGuid():N}){extension}");
    }
}
