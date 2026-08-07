using CubeServer.Data;
using CubeServer.Authentication;
using CubeServer.Features.ReportSignature;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;

#region debug-point C:program-start
Global.DebugReport("post-fix", "C", "Program.cs:10", "Program startup begins", new { cwd = Directory.GetCurrentDirectory() });
#endregion

#region debug-point A:before-app-init
Global.DebugReport("post-fix", "A", "Program.cs:14", "About to create CubeServerApp");
#endregion
Global.app = new CubeServerApp();

var builder = WebApplication.CreateBuilder(args);

#region debug-point A:after-app-init
Global.DebugReport("post-fix", "A", "Program.cs:18", "CubeServerApp created successfully");
#endregion

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.HandshakeTimeout = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMudServices();
builder.Services.AddHostedService<SchedulerService>();
builder.Services.AddSingleton<IReportSignatureStore, FileReportSignatureStore>();
builder.Services.AddSingleton<IReportHtmlPostProcessor, SignatureReportHtmlPostProcessor>();

// Persist the Data Protection key ring to disk so antiforgery tokens and ProtectedSessionStorage
// (used for login sessions - see CheckAuthState.razor) survive app pool recycles/deploys instead
// of being invalidated by a fresh in-memory key ring every restart.
var keyRingPath = builder.Configuration["DataProtection:KeyRingPath"];
if (string.IsNullOrWhiteSpace(keyRingPath))
{
    keyRingPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "CubeServer", "DataProtection-Keys");
}
Directory.CreateDirectory(keyRingPath);

var dataProtection = builder.Services.AddDataProtection()
    .SetApplicationName("CubeServer")
    .PersistKeysToFileSystem(new DirectoryInfo(keyRingPath));

if (OperatingSystem.IsWindows() &&
    builder.Configuration.GetValue("DataProtection:ProtectKeysWithDpapi", true))
{
    dataProtection.ProtectKeysWithDpapi(protectToLocalMachine: true);
}

#region debug-point B:before-build
Global.DebugReport("post-fix", "B", "Program.cs:35", "About to build WebApplication");
#endregion
var app = builder.Build();
Global.Services = app.Services;

#region debug-point B:after-build
Global.DebugReport("post-fix", "B", "Program.cs:39", "WebApplication built", new { env = app.Environment.EnvironmentName });
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

#region debug-point C:before-run
Global.DebugReport("post-fix", "C", "Program.cs:62", "About to enter app.Run()");
#endregion
app.Run();
