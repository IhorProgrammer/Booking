using BookingLibrary.ExceptionMiddleware;
using BookingLibrary.Services.LoggerService;
using BookingLibrary.Services.MessageSender;
using Email.API.Data;
using Email.API.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine(connectionString);
builder.Services.AddDbContext<EmailDBContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<ILoggerManager>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    string connectionString = config.GetConnectionString("LoggerServerConnection") ?? throw new Exception("LoggerServerConnection null");
    return new LoggerManager(connectionString, config);
});
builder.Services.AddSingleton<IMessageSender>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    string connectionString = "Email error";
    return new MessageSender(connectionString);
});

builder.Services.AddSingleton<IEmail, Email.API.Services.Email.Email>();


var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();


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
