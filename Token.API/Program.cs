using BookingLibrary.Data;
using BookingLibrary.Data.Mapping;
using BookingLibrary.ExceptionMiddleware;
using BookingLibrary.Services.LoggerService;
using BookingLibrary.Services.MessageSender;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


builder.Services.AddControllers();
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
builder.Services.AddDbContext<TokenDBContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<ILoggerManager>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    string connectionString = config.GetConnectionString("LoggerServerConnection") ?? throw new Exception("LoggerServerConnection null");
    return new LoggerManager(connectionString);
});
builder.Services.AddSingleton<IMessageSender>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    string connectionString = config.GetConnectionString("EmailServerConnection") ?? throw new Exception("EmailServerConnection null");
    return new MessageSender(connectionString);
});


builder.Services.AddDistributedMemoryCache(); // Додайте цей рядок



//Facebook
//builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
//{
//    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? throw new Exception("AppId null. Check appsettings");
//    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? throw new Exception("AppSecret null. Check appsetings");
//});

// Додайте службу захисту даних
builder.Services.AddDataProtection();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.MapControllers();
app.Run();
