namespace InvoisysTest.Domain.Validators;

public static class CnpjValidator
{
    public static bool CnpjValido(string cnpj)
    {
        return cnpj.Length == 14 && cnpj.All(char.IsDigit);
    }
}
