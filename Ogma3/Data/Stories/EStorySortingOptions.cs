using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ogma3.Data.Stories
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EStorySortingOptions
    {
        [Display(Name="Title ↑")]
        TitleAscending,
        [Display(Name="Title ↓")]
        TitleDescending,
        
        [Display(Name="Date ↑")]
        DateAscending,
        [Display(Name="Date ↓")]
        DateDescending,
        
        [Display(Name="Words ↑")]
        WordsAscending,
        [Display(Name="Words ↓")]
        WordsDescending,
        
        [Display(Name="Score ↑")]
        ScoreAscending,
        [Display(Name="Score ↓")]
        ScoreDescending,
        
        [Display(Name="Updated ↑")]
        UpdatedAscending,
        [Display(Name="Updated ↓")]
        UpdatedDescending,
    }
}