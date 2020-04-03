using System.ComponentModel.DataAnnotations;
namespace ProjectLeader.Classes.Enums
{
  public enum TypeEnum
  {
    [Display(Name = "Závada, chyba")]
    Bug = 1,
    [Display(Name = "Úkol")]
    Task = 2,
    [Display(Name = "Vylepšení")]
    Feature = 3
  }
}