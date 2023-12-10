using FileManagerServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using System.IO;
using System.IO.Compression;

namespace FileManagerServer.Controllers;

[ApiController]
[Route("/api/v1/FileDownload")]
public class FileDownloadController(
	ILogger<FileDownloadController> logger,
	IOptionsSnapshot<Config> options,
	FileManagerService fileManagerService)
	: ControllerBase
{
	private readonly ILogger<FileDownloadController> logger = logger;
	private readonly IOptionsSnapshot<Config> options = options;
	private readonly FileManagerService fileManagerService = fileManagerService;

	[HttpGet("{id}/{*route}")]
	public IActionResult DownloadFileAsync(string id, string? route, CancellationToken cancellationToken)
	{
		var path = fileManagerService.ConvertPath(id, route ?? string.Empty);
		if (System.IO.File.Exists(path))
		{
			var fileName = System.IO.Path.GetFileName(path);
			var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
			return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		}
		else if (System.IO.Directory.Exists(path))
		{
			var dirName = System.IO.Path.GetFileNameWithoutExtension(path);
			var memoryStream = new MemoryStream();
			ZipFile.CreateFromDirectory(path, memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return File(memoryStream, System.Net.Mime.MediaTypeNames.Application.Zip, $"{dirName}.zip");
		}
		else
		{
			throw new ArgumentException($"NotFound path. {id} {route}");
		}
	}
}
