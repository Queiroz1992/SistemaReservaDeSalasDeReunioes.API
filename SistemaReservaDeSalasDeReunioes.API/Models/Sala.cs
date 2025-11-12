using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SistemaReservaDeSalasDeReunioes.API.Models;

public class Sala
{
    public int Id { get; set; }

    [Required]
    public string Local { get; set; } = string.Empty;

    [Required]
    public string Nome { get; set; } = string.Empty;

    public int Capacidade { get; set; }

    [JsonIgnore]
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
