using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ProjectLeader.Classes.Enums
{
  public enum StateEnum
  {
    [Display(Name = "Nový")]
    New = 1,
    [Display(Name = "Aktivní")]
    Active = 2,
    [Display(Name = "V procesu")]
    InProgress = 3,
    [Display(Name = "Vyřešený")]
    Solved = 4,
    [Display(Name = "Zrušený")]
    Canceled = 5
  }
}