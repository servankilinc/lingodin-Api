using Model.Dtos.UserDtos;
using Model.ViewModels;

namespace Business.Abstract;

public interface IAuthService
{
    // SignupAsync ---->  SendAccountVerifyCodeAgain(not required) -----> VerifyUserAccount
    Task SignupAsync(UserCreateDto userCreateDto);
    Task<UserAuthResponseModel> VerifyUserAccount(OtpControlDto otpControlDto);
    Task SendAccountVerifyCodeAgain(Guid userId);


    // LoginAsync ----> VerifyUserAccount
    Task<UserAuthResponseModel> LoginAsync(LoginByEmailDto loginRequest);
    Task<UserAuthResponseModel> VerifyUserAccount(OtpControlByEmail otpControlByEmail);

    Task<UserResponseDto> CreateAuthorizedUserAsync(UserCreateDto userCreateDto);

    // SendPasswordResetMail ----> ResetPassword
    Task SendPasswordResetMail(string email);
    Task<UserAuthResponseModel> ResetPassword(UserPasswordResetDto userPasswordResetDto);
}