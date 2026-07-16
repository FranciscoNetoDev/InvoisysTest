using InvoisysTest.Domain.Enums;
using InvoisysTest.Domain.Extensions;
using InvoisysTest.Domain.Interfaces.Services;
using InvoisysTest.Domain.Models.Request.GravarLoteDocumentoFiscal;
using InvoisysTest.Domain.Models.Response.ValidaLoteDocumentoFiscal;
using InvoisysTest.Domain.Rules;
using InvoisysTest.Domain.Validators;

namespace InvoisysTest.Application.Services;

public class DocumentoFiscalService : IDocumentoFiscalService
{
    private const string DocumentoDuplicadoNoLote = "Documento duplicado no lote.";

    public ValidacaoLoteFiscalResponse ValidaLote(ValidacaoLoteFiscalRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var documentos = request.Documentos ?? new List<DocumentoFiscalRequest>();

        var documentosValidacao = new List<DocumentoFiscalValidacaoResultado>();

        foreach (var documento in documentos)
        {
            var resultado = new DocumentoFiscalValidacaoResultado(new DocumentoFiscalResponse
            {
                Id = documento.Id ?? string.Empty,
                Erros = new List<string>()
            });

            ValidarDocumento(documento, resultado.Response);

            if (resultado.Response.Erros.Any())
                resultado.DefinirStatus(StatusDocumentoFiscalEnum.Invalido);

            documentosValidacao.Add(resultado);
        }

        ValidarDuplicidadeNoLote(documentos, documentosValidacao);

        return new ValidacaoLoteFiscalResponse
        {
            LoteId = request.LoteId,
            TotalDocumentos = documentosValidacao.Count,
            Validos = documentosValidacao.Count(x => x.Status == StatusDocumentoFiscalEnum.Valido),
            Invalidos = documentosValidacao.Count(x => x.Status == StatusDocumentoFiscalEnum.Invalido),
            Documentos = documentosValidacao.Select(x => x.Response).ToList()
        };
    }

    private static void ValidarDocumento(
        DocumentoFiscalRequest documento,
        DocumentoFiscalResponse response)
    {
        if (string.IsNullOrWhiteSpace(documento.Id))
            response.Erros.Add("ID não informado.");

        ValidarTipo(documento, response);

        if (string.IsNullOrWhiteSpace(documento.Numero))
            response.Erros.Add("Número não informado.");

        if (string.IsNullOrWhiteSpace(documento.Serie))
            response.Erros.Add("Série não informada.");

        if (documento.Valor <= 0)
            response.Erros.Add("Valor deve ser maior que zero.");

        if (!string.IsNullOrWhiteSpace(documento.CnpjDestinatario) &&
            !CnpjValidator.CnpjValido(documento.CnpjDestinatario))
            response.Erros.Add("CNPJ do destinatário inválido.");

        if (documento.DataEmissao == default)
            response.Erros.Add("Data de emissão não informada.");

        response.Erros.AddRange(NFSeRule.AplicaRegras(documento));
    }

    private static void ValidarTipo(
        DocumentoFiscalRequest documento,
        DocumentoFiscalResponse response)
    {
        if (string.IsNullOrWhiteSpace(documento.Tipo))
        {
            response.Erros.Add("Tipo não informado.");
            return;
        }

        if (!Enum.TryParse<TipoDocumentoEnum>(
                documento.Tipo.Trim(),
                ignoreCase: true,
                out var tipoDocumento))
        {
            response.Erros.Add("Tipo inválido.");
            return;
        }

    }

    private static void ValidarDuplicidadeNoLote(
        List<DocumentoFiscalRequest> documentos,
        List<DocumentoFiscalValidacaoResultado> documentosValidacao)
    {
        var documentosComIndice = documentos
            .Select((documento, indice) => new
            {
                Documento = documento,
                Indice = indice,
                Chave = MontaChave(documento)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Chave))
            .ToList();

        var documentosDuplicados = documentosComIndice
            .GroupBy(x => x.Chave)
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        foreach (var duplicado in documentosDuplicados)
        {
            var resultado = documentosValidacao[duplicado.Indice];
            var response = resultado.Response;

            resultado.DefinirStatus(StatusDocumentoFiscalEnum.Invalido);

            if (!response.Erros.Contains(DocumentoDuplicadoNoLote))
                response.Erros.Add(DocumentoDuplicadoNoLote);
        }
    }

    private static string? MontaChave(DocumentoFiscalRequest documento)
    {
        if (string.IsNullOrWhiteSpace(documento.Tipo) ||
            string.IsNullOrWhiteSpace(documento.CnpjEmitente) ||
            string.IsNullOrWhiteSpace(documento.Serie) ||
            string.IsNullOrWhiteSpace(documento.Numero))
        {
            return null;
        }

        return string.Join("|",
            documento.Tipo.Trim().ToUpperInvariant(),
            documento.CnpjEmitente.Trim(),
            documento.Serie.Trim(),
            documento.Numero.Trim());
    }

    

    private sealed class DocumentoFiscalValidacaoResultado
    {
        public DocumentoFiscalValidacaoResultado(DocumentoFiscalResponse response)
        {
            Response = response;
            DefinirStatus(StatusDocumentoFiscalEnum.Valido);
        }

        public DocumentoFiscalResponse Response { get; }
        public StatusDocumentoFiscalEnum Status { get; private set; }

        public void DefinirStatus(StatusDocumentoFiscalEnum status)
        {
            Status = status;
            Response.Status = status.GetDescription();
        }
    }

}
