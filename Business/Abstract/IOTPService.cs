using Model.Dtos.UserDtos;
using Model.Entities;

namespace Business.Abstract;

public interface IOTPService
{
    Task SendConfirmationOTP(User user);
    Task VerifyConfirmationOTP(OtpControlDto otpControlDto);
}
