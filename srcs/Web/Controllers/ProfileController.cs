using Microsoft.AspNetCore.Mvc;
using Appservices.CreateProfileDtos;
using Appservices.OutputDtos;
using Serilog;
using Appservices;
using Appservices.Exceptions;

namespace Web.Controllers;


[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ProfileController : ControllerBase
{
    readonly ProfileInteractor _profile;
    public ProfileController(ProfileInteractor profile)
    {
        _profile = profile;
    }

    //Сделать для пользователя, профиля и для детского профиля отдельные контроллеры или объединить их в один?
    //сделать все отдельными контроллерами, но сделать создание профиля, 
    //в которое принимается все возоможные данные профиля, но для остального сделать чтобы каждый отвечал за свое

    /// <summary>
    /// Создание профиля. в поля json вводятся данные которые относятся User(потча, пароль и т.д.).
    /// </summary>
    /// <remarks>
    /// Если нужно создать пользователя, которого еще нет в кинопосике, то нужно пользоваться этим методом. 
    /// Т.е. USER НЕ СОЗДАЕТСЯ В КОНТРОЛЛЕРЕ USER, 
    /// как бы не звучало парадоксально, но связано это с тем, чтобы была целостность данных.
    /// userId: айди, который authService создал
    /// </remarks>
    /// <response code="201">создался профиль</response>
    /// <response code="400">Не прошло валидацию</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(CreateProfileDto input, CancellationToken cancellation)
    {
        try
        {
            var profile = await _profile.Create(input, cancellation);
            return new ObjectResult(profile);
        }
        catch (UserAlreadyExistsException)
        {
            return BadRequest();
        }

    }


    /// <summary>
    /// Возращает полный профиль по id, т.е. тянется вся инфа профиля(user, детские профиля, списки фильмов)
    /// </summary>
    /// <response code="200">нашелся профиль</response>
    /// <response code="404">не нашелся профиль</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet]
    public async Task<ProfileDto> Get(string id, CancellationToken cancellation)
    {
        return await _profile.GetProfile(id, cancellation);
    }

    [Route("~/willwatch/{profileId}/{filmID}/")]
    [HttpPost]
    public async Task<IActionResult> WillWatchPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddWillWatch(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("~/willwatch/{profileId}/{filmID}")]
    [HttpDelete]
    public async Task<IActionResult> WillWatchDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteWillWatch(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("~/watched/{profileId}/{filmID}")]
    [HttpPost]
    public async Task<IActionResult> WatchedPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddWatchded(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("~/watched/{profileId}/{filmID}")]
    [HttpDelete]
    public async Task<IActionResult> WatchedDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteWatchded(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("~/scored/{profileId}/{filmID}")]
    [HttpPost]
    public async Task<IActionResult> ScoredPost(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.AddScored(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }

    [Route("~/scored/{profileId}/{filmID}")]
    [HttpDelete]
    public async Task<IActionResult> ScoredDelete(string profileId, string filmID, CancellationToken cancellation)
    {
        var isSuccess = await _profile.DeleteScored(profileId, filmID, cancellation);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }
}
