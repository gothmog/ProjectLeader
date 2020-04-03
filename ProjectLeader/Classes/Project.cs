using MongoBaseRepository.Classes;
using System.ComponentModel;

namespace ProjectLeader.Classes
{
  public class Project: BaseNamedEntity
  {
    [DisplayName("Popis projektu")]
    public string Description { get; set; }
  }
}