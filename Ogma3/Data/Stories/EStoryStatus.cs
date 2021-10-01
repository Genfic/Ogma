using System.ComponentModel;
using Ogma3.Infrastructure.PostgresEnumHelper;

namespace Ogma3.Data.Stories;

[PostgresEnum]
public enum EStoryStatus
{
    [Description("In Progress")]
    InProgress = 1,
    Completed = 2,
    OnHiatus = 3,
    Cancelled = 4
}