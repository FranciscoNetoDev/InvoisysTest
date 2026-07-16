using InvoisysTest.Domain.Models.Request.GravarLoteDocumentoFiscal;
using InvoisysTest.Domain.Validators;

namespace InvoisysTest.Domain.Rules;

public static class NFSeRule
{
    private static string ValidaCnpjEmitente(string cpnj) 
    {
        if (string.IsNullOrWhiteSpace(cpnj))
          return ("CNPJ do emitente não informado.");
        else if (!CnpjValidator.CnpjValido(cpnj))
            return ("CNPJ do emitente inválido.");

        return string.Empty;
    }

    public static List<string> AplicaRegras(DocumentoFiscalRequest requestNFSE)
    {
        var erros = new List<string>();

        if (!TipoDocumentoValidator.TipoDocumentoValido(requestNFSE.Tipo))
        {
            erros.Add("Tipo inválido.");
            return erros;
        }

        if(ValidaCnpjEmitente(requestNFSE.CnpjEmitente) is string retornoValidacao && !string.IsNullOrWhiteSpace(retornoValidacao))
            erros.Add(retornoValidacao);

        return erros;
    }
}