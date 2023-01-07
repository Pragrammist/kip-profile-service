using Microsoft.AspNetCore.Mvc;
using Appservices.CreateChildProfileDtos;
using Appservices.OutputDtos;
using Appservices;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class ChildController : ControllerBase
{

    readonly ProfileInteractor _profile;
    public ChildController(ProfileInteractor profile)
    {
        _profile = profile;
    }
    ///<summary>
    /// добавление детского профиля в сущетсвующий взрослый профиль 
    ///</summary>
    [HttpPost]
    public async Task<IActionResult> Post(CreateChildProfileDto child)
    {
        var isSuccess = await _profile.AddChildProfile(child);
        if (isSuccess)
            return Ok();
        else
            return BadRequest();
    }



    ///<summary>
    /// удаление детского профиля по его id
    ///</summary>
    [HttpDelete]
    public async Task<IActionResult> Delete(string profileId, string name)
    {
        var isSuccess = await _profile.RemoveChildProfile(profileId, name);
        return isSuccess ? Ok() : BadRequest();
    }
}