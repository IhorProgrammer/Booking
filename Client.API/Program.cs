using Microsoft.EntityFrameworkCore;
using ProjectLibrary.Data;
using ProjectLibrary.ExceptionMiddleware;
using ProjectLibrary.Models.Mapping;
using ProjectLibrary.Services.Hash;
using ProjectLibrary.Services.LoggerService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBContext>( options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
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
