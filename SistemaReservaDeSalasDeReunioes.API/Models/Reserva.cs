using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaReservaDeSalasDeReunioes.API.Models;

public class Reserva
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Sala")]
    public int SalaId { get; set; }
    public Sala? Sala { get; set; }

    [Required]
    [Display(Name = "Responsável")]
    public string Responsavel { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Início")]
    public DateTime Inicio { get; set; }

    [Required]
    [Display(Name = "Fim")]
    public DateTime Fim { get; set; }

    // Campos opcionais de café
    [Display(Name = "Café? (Sim/Não)")]
    public bool Cafe { get; set; }

    [Display(Name = "Qtd Café")]
    public int? QuantidadeCafe { get; set; }

    [Display(Name = "Descrição Café")]
    public string? DescricaoCafe { get; set; }
}
