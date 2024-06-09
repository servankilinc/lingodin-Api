using AutoMapper;
using Business.Abstract;
using Core.CrossCuttingConcerns;
using Core.Utils.Auth;
using Core.Utils.Auth.Hashing;
using Google.Apis.Auth;
using Model.Dtos.UserDtos;
using Model.Entities;
using Model.ViewModels;
using System.Net.Http.Json;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class OAuthService : IOAuthService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly FacebookAppSettings _facebookAppSettings;
    private readonly GoogleJsonWebSignature.ValidationSettings _googleValidationSettings;
    private readonly IMapper _mapper;
    public OAuthService(IUserService userService, ITokenService tokenService, FacebookAppSettings facebookAppSettings, GoogleJsonWebSignature.ValidationSettings googleValidationSettings, IMapper mapper)
    {
        _userService = userService;
        _tokenService = tokenService;
        _facebookAppSettings = facebookAppSettings;
        _googleValidationSettings = googleValidationSettings;
        _mapper = mapper;
    }


    public async Task<UserAuthResponseModel> LoginByGoogle(GoogleLoginRequest googleLoginRequest)
    {
        if (string.IsNullOrWhiteSpace(googleLoginRequest.IdToken)) throw new ArgumentNullException(nameof(googleLoginRequest.IdToken));

        GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginRequest.IdToken, _googleValidationSettings);

        string userFullName = $"{payload.Name} {payload.FamilyName}";

        var model = await HandleUser(payload.Email, userFullName, AutheticatorType.Google);
        return model;
    }

    public async Task<UserAuthResponseModel> LoginByFacebook(FacebookLoginRequest facebookLoginRequest)
    {
        if (string.IsNullOrWhiteSpace(facebookLoginRequest.AccessToken)) throw new ArgumentNullException(nameof(facebookLoginRequest.AccessToken));

        string apiAccessToken = $"{_facebookAppSettings.AppId}|{_facebookAppSettings.AppSecret}";

        HttpClient httpClient = new HttpClient();
        
        FacebookAccesTokenDebugResponse? validationResponse = await httpClient.GetFromJsonAsync<FacebookAccesTokenDebugResponse>($"https://graph.facebook.com/v20.0/debug_token?input_token={facebookLoginRequest.AccessToken}&access_token={apiAccessToken}");
        if (validationResponse == null) throw new Exception("Facebook access debug response could not read!");

        FacebookAccesTokenDebug accesTokenDebug = validationResponse.data;
        if (accesTokenDebug.is_valid == false) throw new Exception("Access is not valid!");
         
        FacebookUserInfo? userInfo = await httpClient.GetFromJsonAsync<FacebookUserInfo>($"https://graph.facebook.com/v20.0/me?fields=first_name%2Clast_name&input_token={facebookLoginRequest.AccessToken}&access_token={apiAccessToken}");
        if (userInfo == null) throw new Exception("User information could not read from facebook!");


        string customMailAddressForFacebookUser = $"{userInfo.id}@facebook.com";
        string userFullName = $"{userInfo.first_name} {userInfo.last_name}";

        var model = await HandleUser(customMailAddressForFacebookUser, userFullName, AutheticatorType.Facebook);
        return model;
    }

    private async Task<UserAuthResponseModel> HandleUser(string mailAddress, string fullName, AutheticatorType autheticatorType)
    {
        User user;
        
        bool isUserExist = await _userService.IsUserExistByEmailAsync(mailAddress);

        if (isUserExist)
        {
            user = await _userService.GetUserDetailByEmailAsync(mailAddress);
        }
        else 
        {
            UserCreateDto createDto = new()
            {
                Email = mailAddress,
                FullName = fullName,
                Password = new Guid().ToString(),
                AutheticatorType = autheticatorType,
            };

            byte[] passwordSalt, passwordHash;
            HashingHelper.CreatePasswordHash(createDto.Password, out passwordHash, out passwordSalt);

            User userToInsert = _mapper.Map<User>(createDto);
            userToInsert.PasswordHash = passwordHash;
            userToInsert.PasswordSalt = passwordSalt;
            userToInsert.IsVerifiedUser = true; 

            user = await _userService.InsertUserAsync(userToInsert);
        }


        AccessTokenResultModel accessTokenResult = await _tokenService.CreateAccessToken(user);

        UserAuthResponseModel responseModel = new UserAuthResponseModel
        {
            User = _mapper.Map<UserResponseDto>(user),
            AccessToken = accessTokenResult.AccessToken,
            Roles = accessTokenResult.Roles
        };

        return responseModel;
    }
}



public class FacebookAccesTokenDebugResponse
{
    public FacebookAccesTokenDebug data { get; set; } = null!;
}
public class FacebookAccesTokenDebug
{
    public bool is_valid { get; set; }
    public string user_id { get; set; } = null!;
}

public class FacebookUserInfo
{
    public string id { get; set; } = null!;
    public string first_name { get; set; } = null!;
    public string last_name { get; set; } = null!;
}



/*
    curl -i -X GET \
    "https://graph.facebook.com/v20.0/debug_token?input_token=...&access_token=..." 
    RESPONSE => 
        {
          "data": {
            "app_id": "1108197060239437",
            "type": "USER",
            "application": "oneday",
            "data_access_expires_at": 1725454466,
            "expires_at": 1717686000,
            "is_valid": true,
            "scopes": [
              "email",
              "public_profile"
            ],
            "user_id": "122151146246223802"
          }
        }
*/

/*
    curl -i -X GET \
    "https://graph.facebook.com/v20.0/me?fields=first_name%2Clast_name&input_token=...&access_token=..."
    RESPONSE => 
        {
          "first_name": "Ali",
          "last_name": "Kılınç",
          "id": "122151146246223802"
        }

curl -i -X GET \

?input_token=EAAPv5jwd4E0BOZCkWbXcuAidf4OxZAlsLPC13SgaQjOkXztpvAUzlWDX0VviptqfGYqb4TZBGDYjtLBXSW2Fa3mtzSmNiWMTE1ZCoQF0I4ZChd3Hw72Ari06YOegJsx79uwqZCrnClmP33xfCsGBKAGe9Tlwf1QxwLAkPM0ZB4BbTWS1IoOtoQQaAn8hwv0hFWkWhZC7zojArwrJDjYLVzLg8bzRdgZDZD
&access_token=EAAPv5jwd4E0BOZCkWbXcuAidf4OxZAlsLPC13SgaQjOkXztpvAUzlWDX0VviptqfGYqb4TZBGDYjtLBXSW2Fa3mtzSmNiWMTE1ZCoQF0I4ZChd3Hw72Ari06YOegJsx79uwqZCrnClmP33xfCsGBKAGe9Tlwf1QxwLAkPM0ZB4BbTWS1IoOtoQQaAn8hwv0hFWkWhZC7zojArwrJDjYLVzLg8bzRdgZDZD"
 */

