using Ampara.Application.Common;
using Microsoft.Extensions.FileProviders;

namespace Ampara.Infrastructure.Storage;

// Stores uploads on the API's disk and serves them under /uploads. Enough for the course project;
// a cloud deploy would swap this for blob storage behind the same interface.
public class LocalFileStorage(UploadsRoot root) : IFileStorage
{
    public const string RequestPath = "/uploads";

    public async Task<string> SaveAsync(Stream content, string folder, string fileName, CancellationToken cancellationToken = default)
    {
        var safeFolder = Path.GetFileName(folder);
        var safeName = Path.GetFileName(fileName);
        var directory = Path.Combine(root.Path, safeFolder);
        Directory.CreateDirectory(directory);

        await using var file = File.Create(Path.Combine(directory, safeName));
        await content.CopyToAsync(file, cancellationToken);

        return $"{RequestPath}/{safeFolder}/{safeName}";
    }

    public Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!url.StartsWith(RequestPath + "/", StringComparison.Ordinal))
            return Task.CompletedTask;

        var relative = url[(RequestPath.Length + 1)..].Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(root.Path, relative));

        // Never delete outside the uploads folder, whatever the URL says.
        if (fullPath.StartsWith(root.Path + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}

public sealed record UploadsRoot(string Path);

public static class StorageExtensions
{
    public static IServiceCollection AddLocalFileStorage(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        var path = System.IO.Path.GetFullPath(System.IO.Path.Combine(contentRootPath, configuration["Storage:UploadsPath"] ?? "uploads"));
        Directory.CreateDirectory(path);

        services.AddSingleton(new UploadsRoot(path));
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        return services;
    }

    public static IApplicationBuilder UseUploads(this IApplicationBuilder app) =>
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(app.ApplicationServices.GetRequiredService<UploadsRoot>().Path),
            RequestPath = LocalFileStorage.RequestPath,
        });
}
