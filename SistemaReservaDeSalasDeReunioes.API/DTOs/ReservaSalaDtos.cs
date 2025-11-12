using System.ComponentModel.DataAnnotations;

namespace SistemaReservaDeSalasDeReunioes.API.DTOs;

// Convert records to classes to avoid DataAnnotations validation metadata issues on positional parameters.
public class SalaDto
{
    public int Id { get; init; }
    public string Local { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public int Capacidade { get; init; }
}

public class SalaCreateDto
{
    [Required]
    public string Local { get; set; } = string.Empty;

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Capacidade { get; set; }
}

public class ReservaDto
{
    public int Id { get; init; }
    public int SalaId { get; init; }
    public string SalaNome { get; init; } = string.Empty;
    public string SalaLocal { get; init; } = string.Empty;
    public string Responsavel { get; init; } = string.Empty;
    public DateTime Inicio { get; init; }
    public DateTime Fim { get; init; }
    public bool Cafe { get; init; }
    public int? QuantidadeCafe { get; init; }
    public string? DescricaoCafe { get; init; }
}

public class ReservaCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "SalaId inválido")]
    public int SalaId { get; set; }

    [Required, MinLength(2)]
    public string Responsavel { get; set; } = string.Empty;

    [Required]
    public DateTime Inicio { get; set; }

    [Required]
    public DateTime Fim { get; set; }

    public bool Cafe { get; set; }
    public int? QuantidadeCafe { get; set; }
    public string? DescricaoCafe { get; set; }
}

public class ReservaUpdateDto
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "SalaId inválido")]
    public int SalaId { get; set; }

    [Required, MinLength(2)]
    public string Responsavel { get; set; } = string.Empty;

    [Required]
    public DateTime Inicio { get; set; }

    [Required]
    public DateTime Fim { get; set; }

    public bool Cafe { get; set; }
    public int? QuantidadeCafe { get; set; }
    public string? DescricaoCafe { get; set; }
}
