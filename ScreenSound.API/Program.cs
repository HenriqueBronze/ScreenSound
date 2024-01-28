using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScreenSoundContext>();
builder.Services.AddTransient<DAL<Artista>>();


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

app.MapGet("/Artistas", async ([FromServices]DAL<Artista> dal) =>
{
    return Results.Ok(dal.Listar());
});

app.MapGet("/Artistas/{nome}", async ([FromServices]DAL<Artista> dal, HttpContext context, string nome) =>
{
    try
    {
        using (var reader = new StreamReader(context.Request.Body))
        {
            var json = await reader.ReadToEndAsync();
            var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

            if (artista is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Artista {nome} não encontrato");
            }
            else
                await context.Response.WriteAsJsonAsync(artista);
        }
    }
    catch (JsonException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync($"Erro ao desserializar JSON: {ex.Message}");
    }
});

app.MapPost("/Artistas", async ([FromBody]Artista artista, [FromServices] DAL<Artista> dal) =>
{
     dal.Adicionar(artista);

    return Results.Created($"/Artistas/{artista.Nome}", artista);

});

app.MapDelete("/Artistas/{id}", async ([FromServices] DAL<Artista> dal, int id) => {
    var artista = dal.RecuperarPor(a => a.Id == id);
    if (artista is null)
    {
        return Results.NotFound();
    }
    
    dal.Deletar(artista);

    return Results.NoContent();

});


app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] Artista artista) => {
    var artistaAAtualizar = dal.RecuperarPor(x => x.Id == artista.Id);
    if (artistaAAtualizar is null)
    {
        return Results.NotFound();
    }
    artistaAAtualizar.Nome = artista.Nome;
    artistaAAtualizar.Bio = artista.Bio;
    artistaAAtualizar.FotoPerfil = artista.FotoPerfil;

    dal.Atualizar(artistaAAtualizar);
    return Results.Ok();
});


app.Run();

