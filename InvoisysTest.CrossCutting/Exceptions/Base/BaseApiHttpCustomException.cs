using System.Net;

namespace InvoisysTest.CrossCutting.Exceptions.Base;

public class BaseApiHttpCustomException(string message, HttpStatusCode responseCode) : Exception(message)
{
    public HttpStatusCode ResponseCode { get; } = responseCode;
}
