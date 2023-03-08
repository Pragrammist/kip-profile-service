using Microsoft.AspNetCore.Mvc;
using Core;

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
    [HttpPut]
    public async Task<IActionResult> WillWatchPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddWillWatch(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("willwatch/delete/{profileId}/{filmID}")]
    [HttpPut]
    public async Task<IActionResult> WillWatchDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteWillWatch(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("watched/{profileId}/{filmID}")]
    [HttpPut]
    public async Task<IActionResult> WatchedPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddWatched(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("watched/delete/{profileId}/{filmID}")]
    [HttpPut]
    public async Task<IActionResult> WatchedDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteWatched(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }


    [Route("notinteresting/{profileId}/{filmID}")]
    [HttpPut]
    public async Task<IActionResult> NotInterestingPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddNotInteresting(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("notinteresting/delete/{profileId}/{filmID}")]
    [HttpPut]
    public async Task<IActionResult> NotInterestingDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteNotInteresting(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }



    [Route("scored/{score}/{profileId}/{filmID}")]
    [HttpPut]
    public async Task<IActionResult> ScoredPost(string profileId, string filmID, uint score, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddScored(profileId, filmID, score, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }
}