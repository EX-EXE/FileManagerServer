using FileManagerServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace FileManagerServer.Controllers;

[ApiController]
[Route("/api/v1/FileManager")]
public class FileManagerController(
	ILogger<FileManagerController> logger,
	IOptionsSnapshot<Config> options,
	FileManagerService fileManagerService)
	: ControllerBase
{
	private readonly ILogger<FileManagerController> logger = logger;
	private readonly IOptionsSnapshot<Config> options = options;
	private readonly FileManagerService fileManagerService = fileManagerService;


	//[HttpGet("{id}/{*route}")]
	//public Task<IActionResult> GetFileInfoAsync(string id, string? route, CancellationToken cancellationToken)
	//{
	//}

	//[HttpPost("{id}/{*route}")]
	//[HttpPut("{id}/{*route}")]
	//public Task<IActionResult> PutFileAsync(string id, string? route, CancellationToken cancellationToken)
	//{
	//}

	[HttpDelete("{id}/{*route}")]
	public IActionResult DeleteFile(string id, string? route)
	{
		var path = fileManagerService.ConvertPath(id, route ?? string.Empty);
		if (System.IO.File.Exists(path))
		{
			System.IO.File.Delete(path);
			return NoContent();
		}
		else if (System.IO.Directory.Exists(path))
		{
			System.IO.Directory.Delete(path, true);
			return NoContent();
		}
		else
		{
			throw new ArgumentException($"NotFound path. {id} {route}");
		}
	}
}
