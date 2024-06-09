using Core.Utils;
using Core.Utils.Pagination;
using Model.Dtos.UserDtos;
using Model.Entities;

namespace Business.Abstract;

public interface IUserService
{
    Task<UserResponseDto> GetUserByIdAsync(Guid userId);
    Task<User> GetUserDetailByIdAsync(Guid userId);
    Task<User> GetUserDetailByEmailAsync(string email);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<Paginate<UserDetailResponseDto>> GetAllUserDetailListAsync(FSPModel fsp);
    Task<User> InsertUserAsync(User user);
    Task<UserResponseDto> UpdateUserAsync(UserUpdateDto userUpdateDto);
    Task<User> UpdateUserDetailAsync(User user); // dont open to public
    Task DeleteUserAsync(Guid userId);
    Task<bool> IsUserExistByEmailAsync(string email);
}