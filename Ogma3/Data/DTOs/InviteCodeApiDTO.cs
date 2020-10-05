using System;

namespace Ogma3.Data.DTOs
{
    public class InviteCodeApiDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NormalizedCode { get; set; }
        public string? UsedByUserName { get; set; }
        public string IssuedByUserName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? UsedDate { get; set; }
    }
}