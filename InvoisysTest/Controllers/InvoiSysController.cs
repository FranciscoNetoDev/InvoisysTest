using InvoisysTest.Domain.Interfaces.Services;
using InvoisysTest.Domain.Models.Request.GravarLoteDocumentoFiscal;
using InvoisysTest.Domain.Models.Response.ValidaLoteDocumentoFiscal;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InvoisysTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiSysController : Controller
{
    private readonly IDocumentoFiscalService _documentoFiscalService;
    public InvoiSysController(IDocumentoFiscalService documentoFiscalService)
    {
        _documentoFiscalService = documentoFiscalService;
    }

    [HttpPost("ValidaLoteDocumentoFiscal")]
    public async Task<IActionResult> ValidaLoteDocumentoFiscal(IFormFile? arquivo)
    {
        if (arquivo is null)
            return RequisicaoInvalida("Nenhum arquivo foi informado.");

        if (arquivo.Length == 0)
            return RequisicaoInvalida("O arquivo informado está vazio.");

        if (!Path.GetExtension(arquivo.FileName).Equals(".json", StringComparison.OrdinalIgnoreCase))
            return RequisicaoInvalida("O arquivo deve estar no formato JSON.");

        ValidacaoLoteFiscalRequest? loteFiscal;

        try
        {
            using var stream = arquivo.OpenReadStream();

            loteFiscal = await JsonSerializer.DeserializeAsync<ValidacaoLoteFiscalRequest>(stream);
        }
        catch (JsonException)
        {
            return RequisicaoInvalida("O arquivo informado não contém um JSON válido.");
        }

        if (loteFiscal is null)
            return RequisicaoInvalida("Não foi possível ler os dados do lote fiscal.");

        ValidacaoLoteFiscalResponse resultado = _documentoFiscalService.ValidaLote(loteFiscal);

        return Ok(resultado);
    }

    private static BadRequestObjectResult RequisicaoInvalida(params string[] erros)
    {
        return new BadRequestObjectResult(new { Erros = erros });
    }
}
