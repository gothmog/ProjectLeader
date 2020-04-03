using System.ComponentModel.DataAnnotations;
namespace ProjectLeader.Classes.Enums
{
  public enum PriorityEnum
  {
    [Display(Name = "Daleká budoucnost")]
    InDistantFuture = 9,
    [Display(Name = "Za dlouho")]
    LongTime = 8,
    [Display(Name = "Velmi nízká")]
    ExtraLow = 7,
    [Display(Name = "Nízká")]
    Low = 6,
    [Display(Name = "Tak akorát")]
    Medium = 5,
    [Display(Name = "Rychle")]
    Fast = 4,
    [Display(Name = "Velmi rychle")]
    VeryFast = 3,
    [Display(Name = "Už teď je pozdě")]
    Urgent = 2,
    [Display(Name = "Kritická")]
    Critical = 1
  }
}