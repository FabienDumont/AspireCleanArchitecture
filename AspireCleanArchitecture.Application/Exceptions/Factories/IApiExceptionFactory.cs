namespace AspireCleanArchitecture.Application.Exceptions.Factories;

public interface IApiExceptionFactory
{
  TException Create<TException>(ErrorCode code, params string[] messageArgs) where TException : ApiException;
}
