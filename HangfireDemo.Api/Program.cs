using Hangfire;
using Hangfire.PostgreSql;
using HangfireDemo.Api;
using CompatibilityLevel = Hangfire.CompatibilityLevel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var hangfireConnectionString = builder.Configuration.GetConnectionString("Hangfire") ??
                               throw new ArgumentException("Unable to find Hangfire connection string");
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c =>
            c.UseNpgsqlConnection(hangfireConnectionString));
});
builder.Services.AddHangfireServer(serverOptions => { serverOptions.ServerName = Guid.NewGuid().ToString(); });

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.MapPost("/enable-workers", (IRecurringJobManager recurringJobManager) =>
    {
        recurringJobManager.AddOrUpdate("MyWorker", () => MyWorker.DoWork(), "0/10 * * * * *");
        return "Workers enabled";
    })
    .WithOpenApi();

app.MapPost("/disable-workers", (IRecurringJobManager recurringJobManager) =>
    {
        recurringJobManager.RemoveIfExists("MyWorker");
        return "Workers disabled";
    })
    .WithOpenApi();

app.Run();