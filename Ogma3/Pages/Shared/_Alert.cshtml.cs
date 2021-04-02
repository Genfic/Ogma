using Ogma3.Data.Enums;

namespace Ogma3.Pages.Shared
{
    public class Alert
    {
        public EAlertType AlertType { get; init; }
        public string Message { get; init; }
        public bool IsDismissible { get; init; } = true;
    }
}