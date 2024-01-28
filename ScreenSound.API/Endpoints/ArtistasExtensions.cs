using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ScreenSound.API.Endpoints
{
    public static class ArtistasExtensions
    {
       public static void AddEndPointsArtistas(this WebApplication app)
        {
            #region Endpoint Artistas
            app.MapGet("/Artistas", async ([FromServices] DAL<Artista> dal) =>
            {
                return Results.Ok(dal.Listar());
            });

            app.MapGet("/Artistas/{nome}", async ([FromServices] DAL<Artista> dal, HttpContext context, string nome) =>
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

            app.MapPost("/Artistas", async ([FromBody] ArtistaRequest artistaRequest, [FromServices] DAL<Artista> dal) =>
            {
                var artista = new Artista(artistaRequest.nome, artistaRequest.bio);

                dal.Adicionar(artista);

                return Results.Created($"/Artistas/{artistaRequest.nome}", artistaRequest);

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
            #endregion
        }
    }
}
