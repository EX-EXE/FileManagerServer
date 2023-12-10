using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FileManagerServer.Controllers;

[ApiController]
[Route("/api/v1/FileManager")]
public class FileManagerController(
	ILogger<FileManagerController> logger,
	IOptionsSnapshot<Config> options)
	: ControllerBase
{
	private readonly ILogger<FileManagerController> logger = logger;
	private readonly IOptionsSnapshot<Config> options = options;


	//[HttpGet("{id}/{*route}")]
	//public Task<IActionResult> GetFileInfoAsync(string id, string? route, CancellationToken cancellationToken)
	//{
	//}

	//[HttpPost("{id}/{*route}")]
	//[HttpPut("{id}/{*route}")]
	//public Task<IActionResult> PutFileAsync(string id, string? route, CancellationToken cancellationToken)
	//{
	//}

	//[HttpDelete("{id}/{*route}")]
	//public Task<IActionResult> DeleteFileAsync(string id, string? route, CancellationToken cancellationToken)
	//{
	//}
}
