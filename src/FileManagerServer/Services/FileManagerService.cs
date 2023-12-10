
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileManagerServer.Services;

public enum FileSystemType
{
	File,
	Directory,
}
public class FileSystemInfo
{
	public FileSystemType Type { get; set; } = FileSystemType.File;
	public string Name { get; set; } = string.Empty;
	public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.MinValue;
	public DateTimeOffset LastWriteTime { get; set; } = DateTimeOffset.MinValue;
	public long FileSize { get; set; } = -1;
}


public class FileManagerService(
	ILogger<FileManagerService> logger,
	IOptionsMonitor<Config> options)
{
	private readonly ILogger<FileManagerService> logger = logger;
	private readonly IOptionsMonitor<Config> options = options;

	public IEnumerable<string> GetIds()
	{
		return options.CurrentValue.BindDirectories.Select(x => x.Id);
	}

	public FileSystemInfo GetFileInfo(string id, string route)
	{
		var path = ConvertPath(id, route);
		if (!System.IO.File.Exists(path))
		{
			throw new FileNotFoundException($"NotFound file. {id} {route}");
		}
		var ioFileInfo = new System.IO.FileInfo(path);
		var resultFileInfo = new FileSystemInfo()
		{
			Type = FileSystemType.File,
			Name = ioFileInfo.Name,
			CreationTime = DateTime.SpecifyKind(ioFileInfo.CreationTime, DateTimeKind.Local),
			LastWriteTime = DateTime.SpecifyKind(ioFileInfo.LastWriteTime, DateTimeKind.Local),
			FileSize = ioFileInfo.Length,
		};
		return resultFileInfo;
	}

	public IEnumerable<FileSystemInfo> GetDirectoryInfos(string id, string route)
	{
		var path = ConvertPath(id, route);
		if (!System.IO.Directory.Exists(path))
		{
			throw new DirectoryNotFoundException($"NotFound directory. {id} {route}");
		}

		var ioDirectoryInfo = new System.IO.DirectoryInfo(path);
		foreach (var ioSubDirInfo in ioDirectoryInfo.GetDirectories())
		{
			yield return new FileSystemInfo()
			{
				Type = FileSystemType.Directory,
				Name = ioSubDirInfo.Name,
				CreationTime = DateTime.SpecifyKind(ioSubDirInfo.CreationTime, DateTimeKind.Local),
				LastWriteTime = DateTime.SpecifyKind(ioSubDirInfo.LastWriteTime, DateTimeKind.Local),
				FileSize = -1,
			};
		}
		foreach (var ioFileInfo in ioDirectoryInfo.GetFiles())
		{
			yield return new FileSystemInfo()
			{
				Type = FileSystemType.File,
				Name = ioFileInfo.Name,
				CreationTime = DateTime.SpecifyKind(ioFileInfo.CreationTime, DateTimeKind.Local),
				LastWriteTime = DateTime.SpecifyKind(ioFileInfo.LastWriteTime, DateTimeKind.Local),
				FileSize = ioFileInfo.Length,
			};
		}
	}

	public string ConvertPath(string id, string route)
	{
		var bindDirectory = options.CurrentValue.BindDirectories.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
		if (bindDirectory == default)
		{
			throw new ArgumentException($"NotFound Id. : {id}");
		}
		var replaceRouteSplit = route.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (replaceRouteSplit.Any(x => x.Equals("..", StringComparison.OrdinalIgnoreCase)))
		{
			throw new ArgumentException($"Invalid Path. : {route}");
		}
		var formatPath = System.IO.Path.Combine([bindDirectory.Path, .. replaceRouteSplit]);
		return formatPath;
	}
}
