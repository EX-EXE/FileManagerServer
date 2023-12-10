using FileManagerServer.Components;
using FileManagerServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net;

namespace FileManagerServer;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorComponents()
			.AddInteractiveServerComponents();

		builder.Services.Configure<Config>(builder.Configuration);
		builder.Services.AddFluentUIComponents();
		builder.Services.AddHttpClient();
		builder.Services.AddSingleton<FileManagerService>();

		builder.Services.AddControllers(options => options.Filters.Add<ConvertExceptionFilterAttribute>());
		builder.Services.AddEndpointsApiExplorer();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseAntiforgery();


		app.MapControllers();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.Run();
	}
}

public class ConvertExceptionFilterAttribute : ExceptionFilterAttribute
{
	public override void OnException(ExceptionContext context)
	{
		var result = new ObjectResult(new { context.Exception.Message, context.Exception.Source, ExceptionType = context.Exception.GetType().FullName });
		result.StatusCode = (int)HttpStatusCode.InternalServerError;
		if (context.Exception is NotImplementedException)
		{
			result.StatusCode = (int)HttpStatusCode.NotImplemented;
		}
		else if (context.Exception is ArgumentException)
		{
			result.StatusCode = (int)HttpStatusCode.BadRequest;
		}
		else if (context.Exception is InvalidOperationException)
		{
			result.StatusCode = (int)HttpStatusCode.InternalServerError;
		}
		else if (context.Exception is FileNotFoundException)
		{
			result.StatusCode = (int)HttpStatusCode.NotFound;
		}
		else if (context.Exception is DirectoryNotFoundException)
		{
			result.StatusCode = (int)HttpStatusCode.NotFound;
		}
		context.Result = result;
	}
}
