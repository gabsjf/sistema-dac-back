namespace Ampara.Application.Common;

// Identifies JPG/PNG by their first bytes, so a renamed file (e.g. script.exe → foto.jpg) is rejected.
public static class ImageFile
{
    public const long MaxSizeBytes = 5 * 1024 * 1024;

    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    // Returns ".jpg", ".png" or null when the content is not one of them.
    public static string? DetectExtension(ReadOnlySpan<byte> header)
    {
        if (header.StartsWith(JpegSignature))
            return ".jpg";

        if (header.StartsWith(PngSignature))
            return ".png";

        return null;
    }
}
