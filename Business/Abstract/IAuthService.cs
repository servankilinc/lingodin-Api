using Model.Dtos.UserDtos;
using Model.ViewModels;

namespace Business.Abstract;

public interface IAuthService
{
    // SignupAsync ---->  SendAccountVerifyCodeAgain(by id, not required) -----> VerifyUserAccount
    Task SignupAsync(UserCreateDto userCreateDto);
    Task<UserAuthResponseModel> VerifyUserAccount(OtpControlDto otpControlDto);
    Task SendAccountVerifyCodeAgain(Guid userId);


    // LoginAsync ---->  SendAccountVerifyCode(by mail, not required) -----> VerifyUserAccount
    Task<UserAuthResponseModel> LoginAsync(LoginByEmailDto loginRequest);
    Task<UserAuthResponseModel> VerifyUserAccount(OtpControlByEmail otpControlByEmail);
    Task SendAccountVerifyCodeAgain(string email);

    Task<UserResponseDto> CreateAuthorizedUserAsync(UserCreateDto userCreateDto);

    // SendPasswordResetMail ----> ResetPassword
    Task SendPasswordResetMail(string email);
    Task<UserAuthResponseModel> ResetPassword(UserPasswordResetDto userPasswordResetDto);
}