using Logger.API.Contracts;
using NLog;
using NLog.Web;
using System.Configuration;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{

    logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    // Add services to the container.


    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var app = builder.Build();



    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    logger.Error(ex);
}
finally
{
    LogManager.Shutdown();
}
