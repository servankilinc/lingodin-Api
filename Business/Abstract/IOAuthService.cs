using Core.Utils.Auth;
using Model.ViewModels;

namespace Business.Abstract;

public interface IOAuthService
{
    Task<UserAuthResponseModel> LoginByGoogle(GoogleLoginRequest googleLoginRequest);
    Task<UserAuthResponseModel> LoginByFacebook(FacebookLoginRequest facebookLoginRequest);
}
