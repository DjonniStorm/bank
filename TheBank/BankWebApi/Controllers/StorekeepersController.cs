using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class StorekeepersController(IStorekeeperAdapter adapter) : ControllerBase
{
    private readonly IStorekeeperAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей кладовщика
    /// </summary>
    /// <returns>список кладовщиков</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о кладовщике по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись кладовщика</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи кладовщика
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] StorekeeperBindingModel model)
    {
        return _adapter.RegisterStorekeeper(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи кладовщика
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] StorekeeperBindingModel model)
    {
        return _adapter.ChangeStorekeeperInfo(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// вход для кладовщика
    /// </summary>
    /// <param name="model">модель с логином и паролем</param>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginBindingModel model) 
    {
        var res = _adapter.Login(model, out string token);
        if (string.IsNullOrEmpty(token))
        {
            return res.GetResponse(Request, Response);
        }

        Response.Cookies.Append(AuthOptions.CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(2)
        });

        return res.GetResponse(Request, Response);
    }

    /// <summary>
    /// Получение данных текущего кладовщика
    /// </summary>
    /// <returns>Данные кладовщика</returns>
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var response = _adapter.GetElement(userId);
        return response.GetResponse(Request, Response);
    }

    /// <summary>
    /// Выход кладовщика
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthOptions.CookieName);
        return Ok();
    }
}
