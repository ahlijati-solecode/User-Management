namespace User_Management.Models.Entities
{
    public partial class MdRefMenu
    {
        public MdRefMenu()
        {
            LgUserAccessRefs = new HashSet<LgRoleAccessRef>();
            MdUserAccessRefs = new HashSet<MdRoleAccessRef>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Key { get; set; } = null!;

        public virtual ICollection<LgRoleAccessRef> LgUserAccessRefs { get; set; }
        public virtual ICollection<MdRoleAccessRef> MdUserAccessRefs { get; set; }
    }
}
