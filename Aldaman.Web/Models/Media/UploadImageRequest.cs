using Microsoft.AspNetCore.Http;

namespace Aldaman.Web.Models.Media;

/// <summary>
/// Request model for client-side image resizing and uploading.
/// </summary>
public class UploadImageRequest
{
    /// <summary>
    /// The uploaded image file stream.
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Target width in pixels.
    /// </summary>
    public int TargetWidth { get; set; }

    /// <summary>
    /// Target height in pixels.
    /// </summary>
    public int TargetHeight { get; set; }
}
