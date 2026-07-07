# InvoisysTest

Aplicação .NET 8 para validar um lote de documentos fiscais recebido em um arquivo JSON.

O projeto foi criado para o desafio técnico de processamento de lote de documentos fiscais. A solução lê um arquivo JSON, valida cada documento em memória, classifica os documentos como válidos ou inválidos e retorna um resumo consolidado do lote.

## Tipo de documento escolhido

A primeira etapa do desafio pede suporte a apenas um tipo de documento fiscal. Esta implementação valida documentos do tipo:

```text
NFE
```

## Stack

* .NET 8
* ASP.NET Core Web API
* xUnit para testes automatizados
* Swagger para facilitar a execução manual em ambiente de desenvolvimento

## Estrutura da solução

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

* `InvoisysTest`: camada de entrada HTTP.
* `InvoisysTest.Application`: regras de validação do lote.
* `InvoisysTest.Domain`: contratos, modelos e enums.
* `InvoisysTest.Tests`: testes automatizados.

## Como executar

Restaure, compile e execute a API:

```powershell
dotnet restore InvoisysTest.sln
dotnet build InvoisysTest.sln
dotnet run --project InvoisysTest\InvoisysTest.csproj
```

Com o perfil HTTP padrão, a API fica disponível em:

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

Campo esperado no formulário:

```text
arquivo
```

O arquivo enviado deve ter a extensão `.json`.

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

## Exemplo de saída

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
      "status": "Inválido",
      "erros": [
        "Número não informado.",
        "Valor deve ser maior que zero.",
        "CNPJ do emitente inválido.",
        "CNPJ do destinatário inválido."
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
