using System.Net;

namespace AspireCleanArchitecture.Application.Exceptions;

public class ConflictException(ErrorCode code, string message) : ApiException(
  code, message, statusCode: HttpStatusCode.Conflict
)
{
}
