using System.Security.Cryptography;

namespace Core.Utils.Auth;

public static class OTPHelper
{
    public static string GenerateSecureVerificationCode()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            int code = BitConverter.ToInt32(randomNumber, 0) % 1000000;
            return Math.Abs(code).ToString("D6");
        }
    }
}
