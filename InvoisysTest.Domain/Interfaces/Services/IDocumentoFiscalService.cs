using InvoisysTest.Domain.Models.Request.GravarLoteDocumentoFiscal;
using InvoisysTest.Domain.Models.Response.ValidaLoteDocumentoFiscal;

namespace InvoisysTest.Domain.Interfaces.Services;

public interface IDocumentoFiscalService
{
    ValidacaoLoteFiscalResponse ValidaLote(ValidacaoLoteFiscalRequest request);
}
