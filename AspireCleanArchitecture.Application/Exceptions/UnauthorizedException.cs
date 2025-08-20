using System.Net;

namespace AspireCleanArchitecture.Application.Exceptions;

public class UnauthorizedException(ErrorCode code, string message) : ApiException(
  code, message, statusCode: HttpStatusCode.Unauthorized
)
{
}
