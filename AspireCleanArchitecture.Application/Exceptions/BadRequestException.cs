using System.Net;

namespace AspireCleanArchitecture.Application.Exceptions;

public class BadRequestException(ErrorCode code, params string[] messages) : ApiException(
  code, messages.FirstOrDefault() ?? "Bad request", messages, HttpStatusCode.BadRequest
)
{
}
