using Client.API.Data;
using Client.API.ExceptionMiddleware;
using Client.API.Models.Mapping;
using Client.API.Services;
using Client.API.Services.Hash;
using Client.API.Services.LoggerService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClientContext>( options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();


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
