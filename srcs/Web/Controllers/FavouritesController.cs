using Microsoft.AspNetCore.Mvc;
using Appservices;

namespace Web.Controllers;

[ApiController]
public class FavouritesController : ControllerBase
{
    readonly ProfileFavouritesInteractor _profile;
    public FavouritesController(ProfileFavouritesInteractor profile)
    {
        _profile = profile;
    }

    [Route("willwatch/{profileId}/{filmID}/")]
    [HttpPost]
    public async Task<IActionResult> WillWatchPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddWillWatch(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("willwatch/{profileId}/{filmID}")]
    [HttpDelete]
    public async Task<IActionResult> WillWatchDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteWillWatch(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("watched/{profileId}/{filmID}")]
    [HttpPost]
    public async Task<IActionResult> WatchedPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddWatched(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("watched/{profileId}/{filmID}")]
    [HttpDelete]
    public async Task<IActionResult> WatchedDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteWatched(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("scored/{score}/{profileId}/{filmID}")]
    [HttpPost]
    public async Task<IActionResult> ScoredPost(string profileId, string filmID, uint score, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddScored(profileId, filmID, score, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }
}