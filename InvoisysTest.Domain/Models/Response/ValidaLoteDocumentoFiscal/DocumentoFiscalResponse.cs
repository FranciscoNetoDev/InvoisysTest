using System.Text.Json.Serialization;

namespace InvoisysTest.Domain.Models.Response.ValidaLoteDocumentoFiscal;

public class DocumentoFiscalResponse
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<string> Erros { get; set; } = new();
}
