using Business.Abstract;
using Core.CrossCuttingConcerns;
using Core.Utils.Auth;
using Microsoft.IdentityModel.Tokens;
using Model.Dtos.RoleDtos;
using Model.Entities;
using Model.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Concrete;

[BusinessExceptionHandler]
internal class TokenService : ITokenService
{
    private readonly IRoleService _roleService;
    private readonly TokenOptions _tokenOptions;
    public TokenService(TokenOptions tokenOptions, IRoleService roleService)
    {
        _tokenOptions = tokenOptions;
        _roleService = roleService;
    }

    public async Task<AccessTokenResultModel> CreateAccessToken(User user)
    {
        ICollection<RoleResponseDto> rolelist = await _roleService.GetUserRolesAsync(user.Id);

        // ************** Claim List Filling **************
        List<Claim> claimList = new List<Claim>();
        string userName = user.FullName ?? "anonymous";
        claimList.Add(new Claim(ClaimTypes.Name, userName));
        claimList.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        if (user.Email != null) claimList.Add(new Claim(ClaimTypes.Email, user.Email));
        if (rolelist.Any())
        {
            rolelist.ToList().ForEach(r => {
                if (r.Name != null) claimList.Add(new Claim(ClaimTypes.Role, r.Name));
            });
        }

        // ************** Jwt Token Creatating **************
        DateTime _accessTokenExp = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey));
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claimList,
            expires: _accessTokenExp,
            signingCredentials: signingCredentials
        );

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

        AccessToken accessToken = new AccessToken(token, _accessTokenExp);

        AccessTokenResultModel resultModel = new AccessTokenResultModel
        {
            AccessToken = accessToken,
            Roles = rolelist
        };

        return resultModel;
    }
}
