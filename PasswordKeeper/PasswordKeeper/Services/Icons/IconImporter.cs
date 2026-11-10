using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using PasswordKeeper.Results;
using System.Threading.Tasks;

namespace PasswordKeeper.Services.Icons;

public class IconImporter(IconStore iconStore)
{
    private static readonly FilePickerFileType ImageFiles = new("Images")
    {
        Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.webp", "*.ico"]
    };

    // Null result means the user cancelled the dialog
    public async Task<IconImportResult?> ImportAsync()
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider is null)
            return IconImportResult.Failure("File dialog is not available");

        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select platform icon",
            AllowMultiple = false,
            FileTypeFilter = [ImageFiles]
        });

        if (files.Count == 0)
            return null;

        await using var stream = await files[0].OpenReadAsync();
        return iconStore.Import(stream, files[0].Name);
    }

    private static IStorageProvider? GetStorageProvider()
        => Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime { MainWindow: { } window } => window.StorageProvider,
            ISingleViewApplicationLifetime { MainView: { } view } => TopLevel.GetTopLevel(view)?.StorageProvider,
            _ => null
        };
}
