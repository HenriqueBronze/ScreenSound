using System.Text.Json.Serialization;

namespace ScreenSound.Modelos;

public class Musica
{
    public Musica(string nome)
    {
        Nome = nome;
    }

    [JsonPropertyName("Nome")]
    public string Nome { get; set; }

    public int Id { get; set; }
    public int? AnoLancamento { get; set; }
    public virtual Artista? Artista { get; set; }

    public void ExibirFichaTecnica()
    {
        Console.WriteLine($"Nome: {Nome}");
      
    }

    public override string ToString()
    {
        return @$"Id: {Id}
        Nome: {Nome}";
    }
}