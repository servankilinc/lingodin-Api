using AutoMapper;
using Business.Abstract;
using Core.CrossCuttingConcerns;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using Model.Dtos.RoleDtos;
using Model.Entities;
using Model.ViewModels;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class RoleService : IRoleService
{
    private readonly IRoleDal _roleDal;
    private readonly IUserRoleDal _userRoleDal;
    private readonly IMapper _mapper;
    private const string RoleGroup = "Role-Cache-Group-Key";
    private const string RoleByName = "RoleByNameCacheKey";
    private const string AllRoleListByUser = "AllRoleListByUserCacheKey";
    private const string AllRoleList = "AllRoleListCacheKey";
    private const string RoleListForUser = "RoleListForUserCacheKey";
    public RoleService(IRoleDal roleDal, IUserRoleDal userRoleDal, IMapper mapper)
    {
        _roleDal = roleDal;
        _userRoleDal = userRoleDal;
        _mapper = mapper;
    }


    [Cache(RoleByName, [RoleGroup])]
    public async Task<RoleResponseDto> GetRoleByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

        var role = await _roleDal.GetAsync(filter: r  => r.Name == name);
        return _mapper.Map<RoleResponseDto>(role);
    }

    [Cache(AllRoleList, [RoleGroup])]
    public async Task<ICollection<RoleResponseDto>> GetAllRolesAsync()
    {
        var roleList = await _roleDal.GetAllAsync();
        var mappedList = roleList.Select(r => _mapper.Map<RoleResponseDto>(r)).ToList();
        return mappedList;
    }


    [Cache(RoleListForUser, [RoleGroup])]
    public async Task<ICollection<RoleResponseDto>> GetUserRolesAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        var roleList = await _userRoleDal.GetAllAsync(
            filter: ur => ur.UserId == userId,
            include : ur => ur.Include(i => i.Role)!
        );
        var mappedList = roleList.Select(r => _mapper.Map<RoleResponseDto>(r.Role)).ToList();
        return mappedList;
    }

    [Cache(AllRoleListByUser, [RoleGroup])]
    public async Task<ICollection<RoleByUserModel>> GetAllRolesByUserAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        var roleList = await _roleDal.GetAllAsync(
            include: r => r.Include(i => i.UserRoles!).ThenInclude(ur => ur.User)!
        );

        var mappedList = roleList
            .Select(r =>
                new RoleByUserModel(r.Id, r.Name, userId, r.UserRoles!.Any(ur => ur.UserId == userId))
            ).ToList();

        return mappedList;
    }

    [CacheRemoveGroup([RoleGroup])]
    [Validation(typeof(RoleCreateDto))]
    public async Task<RoleResponseDto> InsertRoleAsync(RoleCreateDto roleCreateDto)
    {
        var role = _mapper.Map<Role>(roleCreateDto);
        var insertedRole = await _roleDal.AddAsync(role);
        var responseDto = _mapper.Map<RoleResponseDto>(insertedRole);
        return responseDto;
    }


    [CacheRemoveGroup([RoleGroup])]
    public async Task DeleteRoleAsync(Guid roleId)
    {
        if (roleId == Guid.Empty) throw new ArgumentNullException(nameof(roleId));

        var role = new Role() { Id = roleId };
        await _roleDal.DeleteAsync(role);
    }


    [CacheRemoveGroup([RoleGroup])]
    [Validation(typeof(RoleUserRequestDto))]
    public async Task AddRoleToUserAsync(RoleUserRequestDto roleUserRequestDto)
    {
        var userRole = new UserRoles() { 
            UserId = roleUserRequestDto.UserId,
            RoleId = roleUserRequestDto.RoleId
        };
        await _userRoleDal.AddAsync(userRole);
    }


    [CacheRemoveGroup([RoleGroup])]
    [Validation(typeof(RoleUserRequestDto))]
    public async Task RemoveRoleFromUserAsync(RoleUserRequestDto roleUserRequestDto)
    {
        var userRole = new UserRoles()
        {
            UserId = roleUserRequestDto.UserId,
            RoleId = roleUserRequestDto.RoleId
        };
        await _userRoleDal.DeleteAsync(userRole);
    }
}