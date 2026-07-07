namespace InvoisysTest.Domain.Models.Response.ValidaLoteDocumentoFiscal;

public class ValidacaoLoteFiscalResponse
{
    public string LoteId { get; set; } = string.Empty;
    public int TotalDocumentos { get; set; }
    public int Validos { get; set; }
    public int Invalidos { get; set; }
    public List<DocumentoFiscalResponse> Documentos { get; set; } = new();
}
