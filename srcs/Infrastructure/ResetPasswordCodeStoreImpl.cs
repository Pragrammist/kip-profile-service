using Core;

namespace Infrastructure;

public class ResetPasswordCodeStoreImpl : ResetPasswordCodeStore // должен быть singlethon
{
    readonly double _lifeTimeOfCode;
    string CodeGeneration() => Path.GetRandomFileName().Replace(".", string.Empty)[..4];
    readonly Dictionary<string, string> _codes;
    public ResetPasswordCodeStoreImpl(double minutesLifeTime = 5)
    {
        _lifeTimeOfCode = TimeSpan.FromMinutes(minutesLifeTime).TotalMilliseconds;
        _codes = new Dictionary<string, string>();
    }
    
    public Task<bool> CodeIsValid(string id, string code, CancellationToken token)
    {
        var idWithCodeToFind = new KeyValuePair<string, string>(id,code);
        if(_codes.Contains(idWithCodeToFind) )
            return Task.FromResult(true);
        return Task.FromResult(false);
    }

    public Task<string> GenerateCode(string id, CancellationToken token)
    {
        var code = CodeGeneration();
        _codes[id] = code;
        SetDeleting(id, code);
        return Task.FromResult(code);
    }
    private void SetDeleting(string id, string code)
    {
        var lifeTimer = new System.Timers.Timer(_lifeTimeOfCode);
        lifeTimer.Elapsed += (sender, e) => {
            var idWithCodeToFind = new KeyValuePair<string, string>(id,code);
            if(_codes.Contains(idWithCodeToFind)) 
                _codes.Remove(id);
        };
        lifeTimer.Start();
    }
}
