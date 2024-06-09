using AutoMapper;
using Business.Abstract;
using Core.CrossCuttingConcerns;
using Core.Exceptions;
using Core.Utils.Auth;
using Core.Utils.Auth.Hashing;
using Model.Dtos.RoleDtos;
using Model.Dtos.UserDtos;
using Model.Entities;
using Model.ViewModels;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class AuthService : IAuthService
{
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IOTPService _OTPService; 
    private readonly ITokenService _tokenService; 
    private readonly IMapper _mapper;
    public AuthService(IRoleService roleService, IUserService userService, IOTPService oTPService, ITokenService tokenService, IMapper mapper)
    {
        _roleService = roleService;
        _userService = userService;
        _OTPService = oTPService;
        _tokenService = tokenService;
        _mapper = mapper;
    }



    [Validation(typeof(UserCreateDto))]
    public async Task SignupAsync(UserCreateDto userCreateDto)
    {
        // !!! Email unique(IsExist) control doing by _userService.InsertUserAsync 

        byte[] passwordSalt, passwordHash;
        HashingHelper.CreatePasswordHash(userCreateDto.Password, out passwordHash, out passwordSalt);

        User userToInsert = _mapper.Map<User>(userCreateDto);
        userToInsert.PasswordHash = passwordHash;
        userToInsert.PasswordSalt = passwordSalt;
        userToInsert.IsVerifiedUser = false;
        userToInsert.AutheticatorType = AutheticatorType.Email;

        User insertedUser = await _userService.InsertUserAsync(userToInsert);

        await _OTPService.SendConfirmationOTP(user: insertedUser);
    }



    public async Task SendAccountVerifyCodeAgain(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        User user = await _userService.GetUserDetailByIdAsync(userId);
        await _OTPService.SendConfirmationOTP(user: user);
    }



    [Validation(typeof(OtpControlDto))]
    public async Task<UserAuthResponseModel> VerifyUserAccount(OtpControlDto otpControlDto)
    {
        await _OTPService.VerifyConfirmationOTP(otpControlDto);
        // otp verified...
        User existingUser = await _userService.GetUserDetailByIdAsync(otpControlDto.UserId);
        existingUser.IsVerifiedUser = true;
        User updatedUser = await _userService.UpdateUserDetailAsync(existingUser);

        AccessTokenResultModel accessTokenResult = await _tokenService.CreateAccessToken(updatedUser);

        UserAuthResponseModel responseModel = new UserAuthResponseModel
        {
            User = _mapper.Map<UserResponseDto>(updatedUser),
            AccessToken = accessTokenResult.AccessToken,
            Roles = accessTokenResult.Roles
        };

        return responseModel;
    }



    [Validation(typeof(UserCreateDto))]
    public async Task<UserResponseDto> CreateAuthorizedUserAsync(UserCreateDto userCreateDto)
    {
        // !!! Email unique(IsExist) control doing by _userService.InsertUserAsync 

        byte[] passwordSalt, passwordHash;
        HashingHelper.CreatePasswordHash(userCreateDto.Password, out passwordHash, out passwordSalt);

        User userToInsert = _mapper.Map<User>(userCreateDto);
        userToInsert.PasswordHash = passwordHash;
        userToInsert.PasswordSalt = passwordSalt;
        userToInsert.IsVerifiedUser = true;
        userToInsert.AutheticatorType = AutheticatorType.Email;

        User insertedUser = await _userService.InsertUserAsync(userToInsert);

        var authorizedRole = await _roleService.GetRoleByNameAsync("Authorized");
        if (authorizedRole != null)
        {
            await _roleService.AddRoleToUserAsync(new RoleUserRequestDto { RoleId = authorizedRole.Id, UserId = insertedUser.Id });
            return _mapper.Map<UserResponseDto>(insertedUser);
        }
        else
        {
            throw new BusinessException("Failed to assign role to user");
        }
    }



    [Validation(typeof(LoginByEmailDto))]
    public async Task<UserAuthResponseModel> LoginAsync(LoginByEmailDto loginRequest)
    {
        var resultByMail = await _userService.IsUserExistByEmailAsync(loginRequest.Email);
        if (resultByMail == false) throw new BusinessException("Email Address is Not Exist");

        User user = await _userService.GetUserDetailByEmailAsync(loginRequest.Email);
        if (user.IsVerifiedUser == false) throw new BusinessException("NotVerifiedUser");
        if (user.AutheticatorType != AutheticatorType.Email) throw new BusinessException("OauthUserCannotLoginByMail");

        bool isValid = HashingHelper.VerifyPasswordHash(loginRequest.Password, user.PasswordHash!, user.PasswordSalt!);
        if(isValid == false ) throw new BusinessException("Password is not Correct");

        AccessTokenResultModel accessTokenResult = await _tokenService.CreateAccessToken(user);

        UserAuthResponseModel responseModel = new UserAuthResponseModel()
        {
            AccessToken = accessTokenResult.AccessToken,
            Roles = accessTokenResult.Roles,
            User = _mapper.Map<UserResponseDto>(user)
        };

        return responseModel;
    }



    public async Task SendPasswordResetMail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

        User user = await _userService.GetUserDetailByEmailAsync(email);
        if (user == null) throw new BusinessException("Email is not exist!");
        if (user.IsVerifiedUser == false) throw new BusinessException("NotVerifiedUser");
        if (user.AutheticatorType != AutheticatorType.Email) throw new BusinessException("OauthUserCannotResetPassword");

        await _OTPService.SendConfirmationOTP(user: user);
    }



    [Validation(typeof(OtpControlByEmail))]
    public async Task<Guid> VerifyPasswordReset(OtpControlByEmail otpControlByEmail)
    {
        User user = await _userService.GetUserDetailByEmailAsync(otpControlByEmail.Email);
        if (user == null) throw new BusinessException($"Not exist any user for {otpControlByEmail.Email}");

        await _OTPService.VerifyConfirmationOTP(new OtpControlDto(userId: user.Id, code: otpControlByEmail.Code));
        return user.Id;
    }



    [Validation(typeof(UserPasswordResetDto))]
    public async Task<UserAuthResponseModel> ResetPassword(UserPasswordResetDto userPasswordResetDto)
    {
        await _OTPService.VerifyConfirmationOTP(new OtpControlDto(userId: userPasswordResetDto.UserId, code: userPasswordResetDto.VerifyAgainOtpCode));

        byte[] passwordSalt, passwordHash;
        HashingHelper.CreatePasswordHash(userPasswordResetDto.Password!, out passwordHash, out passwordSalt);
        
        User existingUser = await _userService.GetUserDetailByIdAsync(userPasswordResetDto.UserId!);

        existingUser.PasswordHash = passwordHash;
        existingUser.PasswordSalt = passwordSalt;

        User updatedUser = await _userService.UpdateUserDetailAsync(existingUser);

        AccessTokenResultModel accessTokenResult = await _tokenService.CreateAccessToken(updatedUser);

        UserAuthResponseModel responseModel = new UserAuthResponseModel
        {
            User = _mapper.Map<UserResponseDto>(updatedUser),
            AccessToken = accessTokenResult.AccessToken,
            Roles = accessTokenResult.Roles
        };

        return responseModel;
    }
}