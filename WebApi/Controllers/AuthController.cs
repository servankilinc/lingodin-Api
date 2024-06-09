﻿using Business.Abstract;
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
    private readonly IOAuthService _oAuthService;
    public AuthController(IAuthService authService, IOAuthService oAuthService)
    { 
        _authService = authService;
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

    
    // Forget password processes...

    [HttpGet("SendPasswordResetMail")]
    public async Task<IActionResult> SendPasswordResetMail([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email)) throw new ArgumentException(nameof(email));
        await _authService.SendPasswordResetMail(email);
        return Ok();
    }


    [HttpPost("VerifyPasswordReset")]
    public async Task<IActionResult> VerifyPasswordReset([FromBody] OtpControlByEmail otpControlDto)
    {
        var result = await _authService.VerifyPasswordReset(otpControlDto);
        return Ok(result);
    }


    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] UserPasswordResetDto userPasswordResetDto)
    {
        var result = await _authService.ResetPassword(userPasswordResetDto);
        return Ok(result);
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginByEmailDto loginRequest)
    {
        var resultUserLogin = await _authService.LoginAsync(loginRequest);
        return Ok(resultUserLogin);
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
}
