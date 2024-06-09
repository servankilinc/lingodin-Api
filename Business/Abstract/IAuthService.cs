using Model.Dtos.UserDtos;
using Model.ViewModels;

namespace Business.Abstract;

public interface IAuthService
{
    // signup methods
    Task SignupAsync(UserCreateDto userCreateDto);
    Task<UserAuthResponseModel> VerifyUserAccount(OtpControlDto otpControlDto);
    Task SendAccountVerifyCodeAgain(Guid userId);
    
    Task<UserAuthResponseModel> LoginAsync(LoginByEmailDto loginRequest);

    Task<UserResponseDto> CreateAuthorizedUserAsync(UserCreateDto userCreateDto);

    // password... forget processes
    Task SendPasswordResetMail(string email);
    Task<Guid> VerifyPasswordReset(OtpControlByEmail otpControlByEmail);
    Task<UserAuthResponseModel> ResetPassword(UserPasswordResetDto userPasswordResetDto);
}