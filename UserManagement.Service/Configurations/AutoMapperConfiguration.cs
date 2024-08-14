using AutoMapper;
using Shared.Models.Core;
using Shared.Models.Dtos.Core;
using Shared.Models.Filters;
using Shared.Models.Requests;
using User_Management.Models.Dtos.Role;
using User_Management.Models.Dtos.User;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.RefUserAccess;
using User_Management.Models.Entities.Custom.Role;
using User_Management.Models.Filters;
using User_Management.Models.Requests;
using User_Management.Models.Requests.Role;
using User_Management.Models.Requests.UserAccess;
using User_Management.Models.Responses.Role;
using User_Management.Models.Responses.RoleAccess;
using User_Management.Models.Responses.UserAccess;
using User_Management.Models.Requests.RoleUser;
using User_Management.Models.Dtos.UserRole;
using User_Management.Models.Entities.Custom.RoleUser;
using User_Management.Models.Responses.RoleUser;
using User_Management.Models.Responses.UserRole;
using Shared.Service.Models.Entities;
using System.Globalization;

namespace User_Management.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<UserResponse, User>().ReverseMap(); 
            CreateMap<LgActivityRequest, PageFilter>().ReverseMap();
            CreateMap<LgActivityRequest, PagedDto>().ReverseMap(); 
            CreateMap<LgActivityRequest, ActivityFilter>().ReverseMap(); 
            CreateMap<RoleDto, MdRole>().ReverseMap(); ;
            CreateMap<RolePageRequest, RoleFilter>();

            CreateMap<RoleHistoryPagedRequest, PagedDto>();
            CreateMap<PageFilter, PagedDto>();
            CreateMap<ApprovalRequest, RoleDto>();
            CreateMap<CreateRoleRequest, RoleDto>();
            CreateMap<EditeRoleRequest, RoleDto>();
            CreateMap<RolePageRequest, PagedDto>();
            CreateMap<PagedDto, PageFilter>();
            CreateMap<MdRolePagedDto, LgRolePaged>()
                .ForMember(n => n.Is_Admin, n => n.MapFrom(n => n.IsAdmin))
                .ForMember(n => n.Is_Active, n => n.MapFrom(n => n.IsActive))
                .ReverseMap();

            CreateMap<Paged<MdRolePagedDto>, Paged<LgRolePaged>>().ReverseMap();
            CreateMap<Paged<LgRole>, Paged<LgRoleDto>>()                
                .ReverseMap();
            CreateMap<MdRolePagedResponse, MdRolePagedDto>().ReverseMap();
            CreateMap<MdRole, LgRole>().ReverseMap();
            CreateMap<LgRoleDto, LgRole>()
                .ForMember(n => n.CreatedDate, n => n.MapFrom(n => n.Time))
                .ForMember(n => n.CreatedBy, n => n.MapFrom(n => n.User))
                .ReverseMap();
            CreateMap<LgRoleDto, LgRoleResponse>().ReverseMap();
            CreateMap<Paged<LgRoleResponse>, Paged<LgRoleDto>>().ReverseMap();
            CreateMap<Paged<MdRolePagedDto>, Paged<MdRolePagedResponse>>();
            CreateMap<RoleDto, RoleResponse>().ReverseMap();

            CreateMap<RoleDto, LgRole>().ReverseMap();

            CreateMap<LgRefUserAccessPagedDto, RefUserAccessPaged>().ReverseMap();
            CreateMap<Paged<LgRefUserAccessPagedDto>, Paged<RefUserAccessPaged>>().ReverseMap();

            #region UserAccess

            CreateMap<Paged<RoleAccessHistoryDto>, Paged<LgRoleAccess>>()
                .ReverseMap();

            CreateMap<Paged<RoleAccessHistoryDto>, Paged<RoleAccessHistoryResponse>>()
                .ReverseMap();
            CreateMap<LgRoleAccess, RoleAccessHistoryDto>()
                .ForMember(n => n.User, n => n.MapFrom(m => m.CreatedBy))
                .ForMember(n => n.Time, n => n.MapFrom(m => m.CreatedDate))
                .ReverseMap();
            CreateMap<RoleAccessHistoryDto, RoleAccessHistoryResponse>()
                .ReverseMap();
            CreateMap<RoleAccessPageRequest, PagedDto>()
                .ReverseMap();
            CreateMap<MdRoleAccessRefDto, BaseUserAccessRefRequest>()
                .ForMember(n => n.MenuId, n => n.MapFrom(m => m.RefMenuId))
                .ReverseMap();
            CreateMap<LgRoleAccessRefDto, BaseUserAccessRefRequest>()
                .ForMember(n => n.MenuId, n => n.MapFrom(m => m.RefMenuId))
                .ReverseMap();
            CreateMap<MenuDto, MenuResponse>().ReverseMap();
            CreateMap<MdRoleAccessRefDto, BaseRoleAccessRefResponse>()
                .ForMember(n => n.Menu, n => n.MapFrom(m => m.RefMenu))
                .ReverseMap();
            CreateMap<RoleAccessDto, RoleAccessResponse>()
                .ForMember(n => n.Children, n => n.MapFrom(n => n.MdUserAccessRefs));
            CreateMap<MdUserAccessPaged, RoleAccessPagedResponse>();
            CreateMap<RoleAccessPageRequest, PagedDto>();
            CreateMap<RoleAccessPageRequest, RoleAccessFilter>();
            CreateMap<MdRoleAccessRef, LgRoleAccessRef>();
            CreateMap<LgRoleAccessRefDto, LgRoleAccessRef>()
                .ReverseMap();
            CreateMap<MdRoleAccessRef, MdRoleAccessRefDto>()
                .ReverseMap();
            CreateMap<MdUserAccessPaged, RoleAccessDto>();

            CreateMap<MdRefMenu, MenuDto>();

            CreateMap<Paged<MdUserAccessPaged>, Paged<RoleAccessDto>>();

            CreateMap<RoleAccessDto, RoleAccessPagedResponse>()
                .ForMember(n => n.Children, n => n.MapFrom(n => n.MdUserAccessRefs));

            CreateMap<Paged<RoleAccessDto>, Paged<RoleAccessPagedResponse>>();


            CreateMap<RoleAccessDto, MdRoleAccess>()
                .ForMember(n => n.MdUserAccessRefs, n => n.Ignore());

            CreateMap<MdRoleAccess, RoleAccessDto>()
                .ForMember(n => n.MdUserAccessRefs, n => n.MapFrom(n => n.MdUserAccessRefs))
                .ForMember(n => n.LgUserAccessRefs, n => n.MapFrom(n => n.LgUserAccessRefs))
                .ForMember(n => n.LgUserAccesses, n => n.Ignore());
            CreateMap<LgRoleAccess, MdRoleAccess>().ReverseMap();
            CreateMap<BaseUserAccessRequest, RoleAccessDto>()
                .ForMember(n => n.RoleId, n => n.MapFrom(n => n.RoleId))
                .ForMember(n => n.IsActive, n => n.MapFrom(n => n.IsActive))
                .ForMember(n => n.MdUserAccessRefs, n => n.MapFrom(n => n.Children))
                .ReverseMap();

            #endregion

            CreateMap<AddRoleUserRequest, RoleUserDto>().ReverseMap();
            CreateMap<EditRoleUserRequest, RoleUserDto>()
                .ForMember(n => n.Id, n => n.MapFrom(m => m.RoleUserId))
                .ForMember(n => n.IsActive, n => n.MapFrom(m => m.IsActive))
                .ReverseMap(); 
            CreateMap<CommitRoleUserRequest, RoleUserDto>()
                .ForMember(n => n.Id, n => n.MapFrom(m => m.RoleUserId))
                .ForMember(n => n.IsActive, n => n.MapFrom(m => m.IsActive))
                .ReverseMap();
            CreateMap<LgRoleUser, TmpRoleUser>()
                .ForMember(n => n.TmpRoleUserRefs, n => n.MapFrom(m => m.LgRoleUserRefs));
            CreateMap<LgRoleUserRef, TmpRoleUserRef>();
            CreateMap<RoleUserDto, MdRoleUser>().ReverseMap();
            CreateMap<RoleUserDto, TmpRoleUserRef>()
                .ForMember(n => n.Username, n => n.MapFrom(n => n.Username))
                .ReverseMap();

            CreateMap<RoleUserPageRequest, RoleUserFilter>().ReverseMap();
            CreateMap<RoleUserPageRequest, PagedDto>().ReverseMap();
            CreateMap<TmpRoleUserRef, TmpRoleUserRefDto>().ReverseMap();
            CreateMap<Paged<TmpRoleUserRef>, Paged<TmpRoleUserRefDto>>().ReverseMap();
            CreateMap<TmpRoleUser, MdRoleUser>()
                .ForMember(n => n.Id, n => n.Ignore());

            CreateMap<MdRoleUser, LgRoleUser>()
                .ForMember(n => n.Id, n => n.Ignore());

            CreateMap<User, UserDto>(); 
            CreateMap<LgRoleUserRef, User>();
            CreateMap<TmpRoleUserRef, LgRoleUserRef>()
                .ForMember(n => n.Id, n => n.Ignore())
                .ForMember(n => n.ParentId, n => n.Ignore())
                .ForMember(n => n.Parent, n => n.Ignore());
            CreateMap<TmpRoleUserRef, MdRoleUserRef>()
                .ForMember(n => n.Id, n => n.Ignore())
                .ForMember(n => n.ParentId, n => n.Ignore())
                .ForMember(n => n.Parent, n => n.Ignore());
            CreateMap<RoleUserPaged, Models.Dtos.UserRole.LgRoleAccessDto>()
                .ForMember(n => n.Role, n => n.MapFrom(n => n.Name));
            CreateMap<Paged<RoleUserPaged>, Paged<Models.Dtos.UserRole.LgRoleAccessDto>>();
            
            CreateMap<Paged<LgRoleUser>, Paged<UserRoleHistoryDto>>();
            CreateMap<LgRoleUser, UserRoleHistoryDto>()
                .ForMember(n => n.User, n => n.MapFrom(n => n.CreatedBy))
                .ForMember(n => n.Time, n => n.MapFrom(n => n.CreatedDate))
                .ForMember(n => n.Note, n => n.MapFrom(n => n.Note));
            CreateMap<TmpRoleUserRefDto, TmpRoleUserReResponse>();
            CreateMap<Paged<TmpRoleUserRefDto>, Paged<TmpRoleUserReResponse>>();
            CreateMap<Paged<UserRoleHistoryDto>, Paged<RoleUserHistoryResponse>>();
            CreateMap<UserRoleHistoryDto, RoleUserHistoryResponse>();

            CreateMap<LgRoleUserRef, MdRoleUserRef>()
                .ForMember(n => n.Parent, n => n.Ignore());
            CreateMap<RoleUserHistoryPageRequest, PagedDto>();
            CreateMap<MdRoleUserRef, LgRoleUserRef>();
            CreateMap<LgRoleUser, TmpRoleUserDto>()
                .ForMember(n => n.Users, n => n.MapFrom(n => n.LgRoleUserRefs));
            CreateMap<LgRoleUserRef, UserDto>();

            CreateMap<MdRoleAccessRefDto, UserAccessControlResponse>()
                .ForMember(n => n.Menu, n => n.MapFrom(n => n.RefMenu));

            CreateMap<BasePagedRequest, PagedDto>();
            CreateMap<TsTaskList, TsTaskListDto>()
                .ForMember(n => n.UserName, n => n.MapFrom(m => m.CreatedBy))
                .ForMember(n => n.ActivityDate, n => n.MapFrom(m => m.Time.Value.ToString("dd-MMM-yyyy HH:mm", new CultureInfo("en-US"))));
            CreateMap<Paged<TsTaskList>, Paged<TsTaskListDto>>()
                .ReverseMap();
        }
    }
}