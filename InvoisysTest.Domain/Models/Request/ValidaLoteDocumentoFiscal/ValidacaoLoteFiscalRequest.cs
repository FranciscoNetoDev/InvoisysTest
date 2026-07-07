namespace InvoisysTest.Domain.Models.Request.GravarLoteDocumentoFiscal;
public class ValidacaoLoteFiscalRequest
{
    public string LoteId { get; set; } = string.Empty;
    public List<DocumentoFiscalRequest> Documentos { get; set; } = new();
}
