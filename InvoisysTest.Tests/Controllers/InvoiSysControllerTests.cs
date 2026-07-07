using InvoisysTest.Application.Services;
using InvoisysTest.Controllers;
using InvoisysTest.Domain.Models.Response.ValidaLoteDocumentoFiscal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvoisysTest.Tests.Controllers;

public class InvoiSysControllerTests
{
    [Fact]
    public async Task ValidaLoteDocumentoFiscal_DeveRetornarSaidaEsperada_QuandoJsonValido()
    {
        var controller = new InvoiSysController(new DocumentoFiscalService());
        await using var stream = File.OpenRead(Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "ValidaLoteDocumentoFiscalRequest.json"));

        var arquivo = new FormFile(
            stream,
            0,
            stream.Length,
            "arquivo",
            "ValidaLoteDocumentoFiscalRequest.json")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/json"
        };

        var resultado = await controller.ValidaLoteDocumentoFiscal(arquivo);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var response = Assert.IsType<ValidacaoLoteFiscalResponse>(okResult.Value);

        Assert.Equal("LOTE-001", response.LoteId);
        Assert.Equal(2, response.TotalDocumentos);
        Assert.Equal(1, response.Validos);
        Assert.Equal(1, response.Invalidos);

        Assert.Collection(
            response.Documentos,
            documento =>
            {
                Assert.Equal("DOC-001", documento.Id);
                Assert.Equal("Válido", documento.Status);
                Assert.Empty(documento.Erros);
            },
            documento =>
            {
                Assert.Equal("DOC-002", documento.Id);
                Assert.Equal("Inválido", documento.Status);
                Assert.Equal(
                    new[]
                    {
                        "Número não informado.",
                        "Valor deve ser maior que zero.",
                        "CNPJ do emitente inválido.",
                        "CNPJ do destinatário inválido."
                    },
                    documento.Erros);
            });
    }
}
