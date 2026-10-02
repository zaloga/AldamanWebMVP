using Aldaman.Persistence.Enums;

namespace Aldaman.Web.ViewModels.Media;

public sealed class UploadMediaViewModel
{
    public StorageProviderType ActiveStorageProvider { get; init; } = StorageProviderType.FileSystem;
}
