# InvoisysTest

Aplicacao .NET 8 para validar um lote de documentos fiscais recebido em um arquivo JSON.

O projeto foi criado para o desafio tecnico de processamento de lote de documentos fiscais. A solucao le um arquivo JSON, valida cada documento em memoria, classifica os documentos como validos ou invalidos e retorna um resumo consolidado do lote.

## Tipo de documento escolhido

A primeira etapa do desafio pede suporte a apenas um tipo de documento fiscal. Esta implementacao valida documentos do tipo:

```text
NFE
```

Documentos com outros tipos reconhecidos, como `NFSE`, sao marcados como nao suportados.

## Stack

- .NET 8
- ASP.NET Core Web API
- xUnit para testes automatizados
- Swagger para facilitar a execucao manual em ambiente de desenvolvimento

## Estrutura da solucao

```text
InvoisysTest/
  Controllers/
    InvoiSysController.cs

InvoisysTest.Application/
  Services/
    DocumentoFiscalService.cs

InvoisysTest.Domain/
  Enums/
  Interfaces/
  Models/

InvoisysTest.Tests/
  Controllers/
  Assets/
```

Responsabilidades principais:

- `InvoisysTest`: camada de entrada HTTP.
- `InvoisysTest.Application`: regras de validacao do lote.
- `InvoisysTest.Domain`: contratos, modelos e enums.
- `InvoisysTest.Tests`: testes automatizados.

## Regras de validacao

Cada documento do lote e validado com as seguintes regras:

- `id` deve ser informado.
- `tipo` deve ser informado e deve representar um tipo conhecido.
- Apenas `NFE` e suportado nesta etapa.
- `numero` nao pode ser vazio.
- `serie` nao pode ser vazia.
- `valor` deve ser maior que zero.
- `cnpjEmitente` deve conter 14 digitos numericos.
- `cnpjDestinatario`, quando informado, deve conter 14 digitos numericos.
- `dataEmissao` deve ser informada.
- Nao pode haver duplicidade dentro do lote para a combinacao `tipo`, `cnpjEmitente`, `serie` e `numero`.

## Como executar

Restaure, compile e execute a API:

```powershell
dotnet restore InvoisysTest.sln
dotnet build InvoisysTest.sln
dotnet run --project InvoisysTest\InvoisysTest.csproj
```

Com o perfil HTTP padrao, a API fica disponivel em:

```text
http://localhost:5273
```

O Swagger pode ser acessado em:

```text
http://localhost:5273/swagger
```

## Endpoint

```http
POST /api/InvoiSys/ValidaLoteDocumentoFiscal
Content-Type: multipart/form-data
```

Campo esperado no formulario:

```text
arquivo
```

O arquivo enviado deve ter extensao `.json`.

## Exemplo de entrada

```json
{
  "loteId": "LOTE-001",
  "documentos": [
    {
      "id": "DOC-001",
      "tipo": "NFE",
      "numero": "12345",
      "serie": "1",
      "valor": 1500.50,
      "cnpjEmitente": "12345678000195",
      "cnpjDestinatario": "98765432000110",
      "dataEmissao": "2026-04-10"
    },
    {
      "id": "DOC-002",
      "tipo": "NFE",
      "numero": "",
      "serie": "1",
      "valor": -10,
      "cnpjEmitente": "111",
      "cnpjDestinatario": "222",
      "dataEmissao": "2026-04-10"
    }
  ]
}
```

O mesmo exemplo esta disponivel em:

```text
InvoisysTest.Tests\Assets\ValidaLoteDocumentoFiscalRequest.json
```

## Exemplo de chamada com PowerShell

```powershell
$arquivo = Get-Item ".\InvoisysTest.Tests\Assets\ValidaLoteDocumentoFiscalRequest.json"

Invoke-RestMethod `
  -Uri "http://localhost:5273/api/InvoiSys/ValidaLoteDocumentoFiscal" `
  -Method Post `
  -Form @{ arquivo = $arquivo }
```

## Exemplo de saida

```json
{
  "loteId": "LOTE-001",
  "totalDocumentos": 2,
  "validos": 1,
  "invalidos": 1,
  "documentos": [
    {
      "id": "DOC-001",
      "status": "V\u00e1lido",
      "erros": []
    },
    {
      "id": "DOC-002",
      "status": "Inv\u00e1lido",
      "erros": [
        "N\u00famero n\u00e3o informado.",
        "Valor deve ser maior que zero.",
        "CNPJ do emitente inv\u00e1lido.",
        "CNPJ do destinat\u00e1rio inv\u00e1lido."
      ]
    }
  ]
}
```

## Testes

Execute os testes automatizados com:

```powershell
dotnet test InvoisysTest.sln
```

## Decisoes tecnicas

- Todo o processamento ocorre em memoria, sem banco de dados.
- O arquivo JSON e recebido por upload para manter a entrada explicita e simples de testar via API.
- As regras de negocio ficam concentradas em `DocumentoFiscalService`.
- O controller trata validacoes de entrada relacionadas ao arquivo, como ausencia, arquivo vazio, extensao invalida e JSON malformado.
- A solucao evita integracoes externas, filas, cache, autenticacao e persistencia, mantendo aderencia ao escopo do desafio.

## Melhorias futuras

- Ampliar a cobertura de testes para duplicidade, tipo invalido, tipo nao suportado, arquivo vazio e JSON invalido.
- Padronizar o status de saida como `VALIDO` e `INVALIDO`, caso seja exigida aderencia textual exata ao exemplo do desafio.
- Adicionar um arquivo de exemplo de saida versionado fora do README.
