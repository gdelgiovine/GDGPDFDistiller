using GDGPDFDistiller.Services;
using GDGPDFDistiller.Filters;
using System.Dynamic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using GDGPDFDistiller;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GDGPDFDistiller API",
        Version = "v1",
        Description = "API per la distillazione di PDF",
        Contact = new OpenApiContact
        {
            Name = "Supporto",
            Email = "info@gabrieledelgiovine.it",
            Url = new Uri("https://www.gabrieledelgiovine.it")
        }
         
    });
    
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "GDGPDFDistiller.xml"));
    c.EnableAnnotations();
    // Queste opzioni aiutano con la serializzazione
    c.CustomSchemaIds(type => type.FullName);
    c.SchemaFilter<RequiredPropertiesSchemaFilter>();
    c.OperationFilter<FileUploadOperationFilter>();





});

// Aggiungi i servizi al contenitore.
builder.Services.Configure<GhostscriptSettings>(builder.Configuration.GetSection("GhostscriptSettings"));
builder.Services.AddSingleton<ConversionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json?v=1", "GDGPDFDistiller API v1");
        c.RoutePrefix = "swagger"; // default
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



