using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using System.Text.Json;

namespace ScreenSound.API.Endpoints
{

    public static class MusicasExtensions
    {
        public static void AddEndPointsMusicas(this WebApplication app)
        {
            #region Endpoint Músicas
            app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
            {
                return Results.Ok(dal.Listar());
            });

            app.MapGet("/Musicas/{nome}", async ([FromServices] DAL<Musica> dal, HttpContext context, string nome) =>
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

            app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] Musica musica) =>
            {
                dal.Adicionar(musica);
                return Results.Ok();
            });

            app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) => {
                var musica = dal.RecuperarPor(a => a.Id == id);
                if (musica is null)
                {
                    return Results.NotFound();
                }
                dal.Deletar(musica);
                return Results.NoContent();

            });

            app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] Musica musica) => {
                var musicaAAtualizar = dal.RecuperarPor(a => a.Id == musica.Id);
                if (musicaAAtualizar is null)
                {
                    return Results.NotFound();
                }
                musicaAAtualizar.Nome = musica.Nome;
                musicaAAtualizar.AnoLancamento = musica.AnoLancamento;

                dal.Atualizar(musicaAAtualizar);
                return Results.Ok();
            });
            #endregion
        }
    }
}
