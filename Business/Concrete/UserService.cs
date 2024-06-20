using AutoMapper;
using Business.Abstract;
using Core.CrossCuttingConcerns;
using Core.Exceptions;
using Core.Utils;
using Core.Utils.Pagination;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using Model.Dtos.RoleDtos;
using Model.Dtos.UserDtos;
using Model.Entities;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class UserService : IUserService
{
    private readonly IUserDal _userDal;
    private readonly IMapper _mapper;
    private const string UserGroup = "User-Cache-Group-Key";
    private const string AllUserList = "AllUserListCacheKey";
    private const string UserInfoById = "UserInfoByIdCacheKey";
    private const string UserInfoByMail = "UserInfoByMailCacheKey";
    private const string UserDetailById = "UserDetailByIdCacheKey";
    private const string UserDetailByEmail = "UserDetailByEmailCacheKey";
    private const string IsUserExistByEmail = "IsUserExistByEmailCacheKey";
    public UserService(IUserDal userDal, IMapper mapper)
    {
        _userDal = userDal;
        _mapper = mapper;
    }


    [Cache(UserInfoById, [UserGroup])]
    public async Task<UserResponseDto> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        User user = await _userDal.GetAsync(filter: u => u.Id == userId);
        UserResponseDto responseDto = _mapper.Map<UserResponseDto>(user);
        return responseDto;
    }


    [Cache(UserInfoByMail, [UserGroup])]
    public async Task<UserResponseDto> GetUserByMailAsync(string mail)
    {
        if (string.IsNullOrWhiteSpace(mail)) throw new ArgumentNullException(nameof(mail));
        User user = await _userDal.GetAsync(filter: u => u.Email == mail);
        UserResponseDto responseDto = _mapper.Map<UserResponseDto>(user);
        return responseDto;
    }


    [Cache(AllUserList, [UserGroup])]
    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        IEnumerable<User> userList = await _userDal.GetAllAsync();
        List<UserResponseDto> mappedList = userList.Select(u => _mapper.Map<UserResponseDto>(u)).ToList();
        return mappedList;
    }


    [Cache(UserDetailByEmail, [UserGroup])]
    public async Task<User> GetUserDetailByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        User user = await _userDal.GetAsync(filter: u => u.Email == email); 
        return user;
    }


    [Cache(UserDetailById, [UserGroup])]
    public async Task<User> GetUserDetailByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        User user = await _userDal.GetAsync(filter: u => u.Id == userId); 
        return user;
    }


    public async Task<Paginate<UserDetailResponseDto>> GetAllUserDetailListAsync(FSPModel fsp)
    {
        Paginate<User> userList = await _userDal.GetPaginatedListByDynamicAsync(
            dynamicQuery: fsp.DynamicQuery!,
            index: fsp.PagingRequest!.Page,
            size: fsp.PagingRequest.PageSize,
            include: u => u.Include(u => u.UserRoles)!.ThenInclude(ur => ur.Role)!
        );
        List<UserDetailResponseDto> usersResponseList = userList.Items.
            Select(u => u.UserRoles != null ? 
                new UserDetailResponseDto(u.Id, u.FullName!, u.Email!, u.IsVerifiedUser, u.UserRoles.Select(ur => _mapper.Map<RoleResponseDto>(ur.Role))) :
                new UserDetailResponseDto(u.Id, u.FullName!, u.Email!, u.IsVerifiedUser)
            ).ToList();

        Paginate<UserDetailResponseDto> responseDto = new()
        {
            Count = userList.Count,
            Index = userList.Index,
            Pages = userList.Pages,
            Size = userList.Size,
            Items = usersResponseList
        };
        return responseDto;
    }


    [CacheRemoveGroup([UserGroup])]
    public async Task<User> InsertUserAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentNullException(nameof(user.Email));
        var emailExist = await IsUserExistByEmailAsync(user.Email);
        if (emailExist) throw new BusinessException("Email Address Already Exist");

        User insertedUser = await _userDal.AddAsync(user);
        return insertedUser;
    }


    [CacheRemoveGroup([UserGroup])]
    [Validation(typeof(UserUpdateDto))]
    public async Task<UserResponseDto> UpdateUserAsync(UserUpdateDto userUpdateDto)
    {   
        User existingUser = await _userDal.GetAsync(filter: u => u.Id == userUpdateDto.Id);
        if (existingUser == null) throw new BusinessException("Data(existing) for update Not Found !");
        _mapper.Map(source: userUpdateDto, destination: existingUser);
        User updatedUser = await _userDal.UpdateAsync(existingUser);
        return _mapper.Map<UserResponseDto>(updatedUser);
    }


    [CacheRemoveGroup([UserGroup])]
    public async Task<User> UpdateUserDetailAsync(User user) // not open to public
    {
        User updatedUser = await _userDal.UpdateAsync(user);
        return updatedUser;
    }


    [CacheRemoveGroup([UserGroup])]
    public async Task DeleteUserAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        var user = new User() { Id = userId };
        await _userDal.DeleteAsync(user);
    }


    [Cache(IsUserExistByEmail, [UserGroup])]
    public async Task<bool> IsUserExistByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        bool isExist = await _userDal.IsExistAsync(filter: u => u.Email == email);
        return isExist;
    }
}