using Microsoft.AspNetCore.Mvc;
using Core.CreateProfileDtos;
using Core.OutputDtos;
using Serilog;
using Core;
using Core.Exceptions;

namespace Web.Controllers;


[ApiController]
[Route("profile")]
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

    /// <summary>
    /// Возращает полный профиль по id, т.е. тянется вся инфа профиля(user, детские профиля, списки фильмов)
    /// </summary>
    /// <response code="200">нашелся профиль</response>
    /// <response code="404">не нашелся профиль</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("login")]
    public async Task<ProfileDto> GetProfile(string loginOrEmail, string password, CancellationToken cancellation)
    {
        return await _profile.GetProfile(loginOrEmail, password, cancellation);
    }
}
