using Business.Abstract;
using Core.Utils.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos.UserDtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOTPService _oTPService;
    private readonly IOAuthService _oAuthService;
    public AuthController(IAuthService authService, IOTPService oTPService, IOAuthService oAuthService)
    { 
        _authService = authService;
        _oTPService = oTPService;
        _oAuthService = oAuthService;
    }


    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] UserCreateDto userRequest)
    {
        await _authService.SignupAsync(userRequest);
        return Ok();
    }


    [HttpPost("VerifyUser")]
    public async Task<IActionResult> VerifyUser([FromBody] OtpControlDto otpControlDto)
    {
        var resultUserRegister =  await _authService.VerifyUserAccount(otpControlDto);
        return Ok(resultUserRegister);
    }


    [HttpGet("SendVerifyCodeAgain")]
    public async Task<IActionResult> SendVerifyCodeAgain([FromQuery] Guid userId)
    {
        await _authService.SendAccountVerifyCodeAgain(userId);
        return Ok();
    }


    [HttpGet("SendVerifyCodeAgainByMail")]
    public async Task<IActionResult> SendVerifyCodeAgainByMail([FromQuery] string email)
    {
        await _authService.SendAccountVerifyCodeAgain(email);
        return Ok();
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginByEmailDto loginRequest)
    {
        var resultUserLogin = await _authService.LoginAsync(loginRequest);
        return Ok(resultUserLogin);
    }


    [HttpPost("VerifyUserByMail")]
    public async Task<IActionResult> VerifyUserByMail([FromBody] OtpControlByEmail controlByEmail)
    {
        var resultUserRegister = await _authService.VerifyUserAccount(controlByEmail);
        return Ok(resultUserRegister);
    }


    // Forget password processes...

    [HttpGet("SendPasswordResetMail")]
    public async Task<IActionResult> SendPasswordResetMail([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email)) throw new ArgumentException(nameof(email));
        await _authService.SendPasswordResetMail(email);
        return Ok();
    }


    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] UserPasswordResetDto userPasswordResetDto)
    {
        var result = await _authService.ResetPassword(userPasswordResetDto);
        return Ok(result);
    }



    [HttpPost("LoginByGoogle")]
    public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequest loginRequest)
    {
        var resultUserLogin = await _oAuthService.LoginByGoogle(loginRequest);
        return Ok(resultUserLogin);
    }
    

    [HttpPost("LoginByFacebook")]
    public async Task<IActionResult> LoginByFacebook([FromBody] FacebookLoginRequest loginRequest)
    {
        var resultUserLogin = await _oAuthService.LoginByFacebook(loginRequest);
        return Ok(resultUserLogin);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("CreateAuthorized")]
    public async Task<IActionResult> CreateAuthorized([FromBody] UserCreateDto userRequest)
    {
        var resultUserRegister = await _authService.CreateAuthorizedUserAsync(userRequest);
        return Ok(resultUserRegister);
    }



    [Authorize(Roles = "Admin")]
    [HttpGet("ChangeUserAccountVerifyStatus")]
    public async Task<IActionResult> ChangeUserAccountVerifyStatus([FromQuery] Guid userId)
    {
        await _authService.ChangeUserAccountVerifyStatusAsync(userId);
        return Ok();
    }


    [HttpGet("GetOTPExpirationTimeById")]
    public async Task<IActionResult> GetOTPExpirationTimeById(Guid userId)
    {
        var expiryTime = await _oTPService.GetOTPExpirationTime(userId);
        return Ok(expiryTime);
    }
     
    [HttpGet("GetOTPExpirationTimeByMail")]
    public async Task<IActionResult> GetOTPExpirationTimeByMail(string email)
    {
        var expiryTime = await _oTPService.GetOTPExpirationTime(email);
        return Ok(expiryTime);
    }
}
