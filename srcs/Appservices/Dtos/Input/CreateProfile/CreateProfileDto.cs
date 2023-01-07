using System.Reflection;
using System.Text.Json.Serialization;

namespace Appservices.CreateProfileDtos;

public class CreateProfileDto
{
    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; set; } = null!;

    /// <summary>
    /// Почта
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Хэши пароля
    /// </summary>
    public string Password { get; set; } = null!;
}

