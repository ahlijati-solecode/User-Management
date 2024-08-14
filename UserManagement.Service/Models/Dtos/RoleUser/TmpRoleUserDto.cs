using User_Management.Models.Dtos.Role;

namespace User_Management.Models.Dtos.UserRole
{
    public class TmpRoleUserDto
    {
        public int Id { get; set; }
        public RoleDto Role { get; set; }
        public List<UserDto> Users { get; set; }
    }
   
}
