using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ogma3.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EClubSortingOptions
    {
        [Display(Name="Name ↑")]
        NameAscending,
        [Display(Name="Name ↓")]
        NameDescending,
        
        [Display(Name="Members ↑")]
        MembersAscending,
        [Display(Name="Members ↓")]
        MembersDescending,
        
        [Display(Name="Stories ↑")]
        StoriesAscending,
        [Display(Name="Stories ↓")]
        StoriesDescending,
        
        [Display(Name="Threads ↑")]
        ThreadsAscending,
        [Display(Name="Threads ↓")]
        ThreadsDescending,
        
        [Display(Name="Creation Date ↑")]
        CreationDateAscending,
        [Display(Name="Creation Date ↓")]
        CreationDateDescending
    }
}