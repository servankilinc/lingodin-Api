using Model.Entities;
using Model.ViewModels;

namespace Business.Abstract;

public interface ITokenService
{
    Task<AccessTokenResultModel> CreateAccessToken(User user);
}
