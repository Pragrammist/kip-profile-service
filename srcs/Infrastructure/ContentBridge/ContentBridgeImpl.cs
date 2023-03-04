using Appservices;
using Infrastructure.ContentBridge.GrpcFilmService;


namespace Infrastructure;

public class ContentBridgeImpl : Appservices.ContentBridge
{
    readonly FilmServiceProto.FilmServiceProtoClient _client;


    public ContentBridgeImpl(FilmServiceProto.FilmServiceProtoClient client)
    {
        _client = client;
    }
    public async Task<bool> AddScore(string filmId, uint score, CancellationToken token)
    {
        var req = new ScoreRequest
        {
            FilmdId = filmId,
            Score =  score
        };
        var res = await _client.ScoreAsync(req, cancellationToken: token);
        return res.Success;
    }

    public async Task<bool> AddWatched(string filmId, CancellationToken token) =>
        (await _client.IncrWatchedCountAsync(FilmReq(filmId), cancellationToken: token)).Success;

    public async Task<bool> DeleteWatched(string filmId, CancellationToken token) =>
        (await _client.DecrWatchedCountAsync(FilmReq(filmId), cancellationToken: token)).Success;



    public async Task<bool> AddWillWatch(string filmId, CancellationToken token) =>
        (await _client.IncrWillWatchCountAsync(FilmReq(filmId), cancellationToken: token)).Success;   

    public async Task<bool> DeleteWillWatch(string filmId, CancellationToken token)=>
        (await _client.DecrWillWatchCountAsync(FilmReq(filmId), cancellationToken: token)).Success;


    
    public async Task<bool> AddNotInteresting(string filmId, CancellationToken token) =>
        (await _client.IncrNotInterestingCountAsync(FilmReq(filmId), cancellationToken: token)).Success;

    public async Task<bool> DeleteNotInteresting(string filmId, CancellationToken token) =>
        (await _client.DecrNotInterestingCountAsync(FilmReq(filmId), cancellationToken: token)).Success;


    FilmIdRequest FilmReq(string filmId) => new()
    {
        FilmdId = filmId
    };
}

