using InvoisysTest.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoisysTest.Domain.Validators;

public static class TipoDocumentoValidator
{
    public static bool TipoDocumentoValido(string tipoDocumento)
    {
        if (string.IsNullOrWhiteSpace(tipoDocumento))
            return false;

        return Enum.TryParse<TipoDocumentoEnum>(
            tipoDocumento.Trim(),
            ignoreCase: true,
            out _);
    }
}
