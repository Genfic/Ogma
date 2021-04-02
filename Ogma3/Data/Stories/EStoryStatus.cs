using System.ComponentModel;

namespace Ogma3.Data.Stories
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