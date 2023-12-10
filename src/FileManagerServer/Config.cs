namespace FileManagerServer;



public class Config
{
	public BindingDirectoryConfig[] BindDirectories { get; set; } = [];
}

public class BindingDirectoryConfig
{
	public string Id { get; set; } = string.Empty;
	public string Path { get; set; } = string.Empty;
}