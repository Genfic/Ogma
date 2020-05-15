using System;
using Ogma3.Data.Models;

namespace Ogma3.Data.DTOs
{
    public class InviteCodeApiDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string NormalizedCode { get; set; }
        public string? UserName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? UsedDate { get; set; }

        public static InviteCodeApiDTO FromInviteCode(InviteCode code)
        {
            return new InviteCodeApiDTO
            {
                Id = code.Id,
                Code = code.Code,
                NormalizedCode = code.NormalizedCode,
                UserName = code.UsedBy?.UserName,
                IssueDate = code.IssueDate,
                UsedDate = code.UsedDate
            };
        }
    }
}