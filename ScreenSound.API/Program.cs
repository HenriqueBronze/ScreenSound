using ScreenSound.Banco;
using ScreenSound.Modelos;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Artistas", () =>
{
    var dal = new DAL<Artista>(new ScreenSoundContext());
    return dal.Listar();
});

app.MapGet("/Artistas/{nome}", async (HttpContext context, string nome) =>
{
    try
    {
        using (var reader = new StreamReader(context.Request.Body))
        {
            var json = await reader.ReadToEndAsync();
            var dal = new DAL<Artista>(new ScreenSoundContext());
            var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

            await context.Response.WriteAsJsonAsync(artista);
        }
    }
    catch (JsonException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync($"Erro ao desserializar JSON: {ex.Message}");
    }
});




app.Run();

