namespace InvoisysTest.Domain.Models.Request.GravarLoteDocumentoFiscal;

public class DocumentoFiscalRequest
{
    public string Id { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string CnpjEmitente { get; set; } = string.Empty;
    public string? CnpjDestinatario { get; set; }
    public DateTime DataEmissao { get; set; }
}
