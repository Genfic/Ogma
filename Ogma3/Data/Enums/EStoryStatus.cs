using System.ComponentModel;

namespace Ogma3.Data.Enums
{
    public enum EStoryStatus
    {
        [Description("In Progress")]
        InProgress = 1,
        Completed = 2,
        OnHiatus = 3,
        Cancelled = 4
    }
}