namespace Core;

public interface ResetPasswordCodeStore
{
    Task<bool> CodeIsValid(string id, string code, CancellationToken token);

    Task<string> GenerateCode(string id, CancellationToken token);
}
