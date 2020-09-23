namespace Ogma3.Data.DTOs
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Color { get; set; }
        public bool IsStaff { get; set; }
        public int Order { get; set; }
    }
}