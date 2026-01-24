using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Extras.Models;

public class BaseEnviados {
    public string? Name { get; set; }
    public byte[]? File { get; set; }
    public DateTime DTBase { get; set; }
    public string? NomeRegua { get; set; }
    public string? Canal { get; set; }
    public string? Mensagem { get; set; }
    public string? NumeroFatura { get; set; }
    public string? NumeroContrato { get; set; }
    public string? IdCliente { get; set; }
    public string? Contato { get; set; }
    public string? Nome { get; set; }
    public string? NomeCompleto { get; set; }
    public string? CPFCNPJ { get; set; }
    public double ValorFatura { get; set; }
    public DateTime VencimentoFatura { get; set; }
    public int AtrasoCliente { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? LinkFatura { get; set; }
    public string? CodBarras { get; set; }
    public string? TipoContato { get; set; }
    public string? ParticularidadesEmpresa { get; set; }
    public string? SobreNome { get; set; }
    public double TaxaJuros { get; set; }
    public string? NumeroParcela { get; set; }
    public string? NomeProduto { get; set; }
    public string? Prestacao { get; set; }
    public string? CodebarBase64 { get; set; }
    public string? BoletoBase64 { get; set; }
    public string? PixBase64 { get; set; }
    public string? CodigoPix { get; set; }
    public string? NumeroPedido { get; set; }
    public string? Endereco { get; set; }
    public string? Matricula { get; set; }
    public int? ClienteId { get; set; }
    public string? DesContr { get; set; }
    public string? Credor { get; set; }
    public string? ValorMedia { get; set; }
    public string? Pedra { get; set; }
}
