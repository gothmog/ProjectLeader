using ProjectLeader.Classes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProjectLeader.Models
{
  public class TaskInsertViewModel
  {
    [DisplayName("Název úkolu")]
    public string NameIns { get; set; }
    [DisplayName("Popis")]
    public string DescriptionIns { get; set; }
    [DisplayName("Stav")]
    public StateEnum StateEnumIns { get; set; }
    [DisplayName("Priorita")]
    public PriorityEnum PriorityEnumIns { get; set; }
    [DisplayName("Typ")]
    public TypeEnum TypeEnumIns { get; set; }
    [DisplayName("Uživatel")]
    public string UserIdIns { get; set; }
    public string ParentTaskIdIns { get; set; }
    public string ProjectIdIns { get; set; }

		[DisplayName("Začátek")]
		public DateTime? StartInsert { get; set; }
		[DisplayName("Konec")]
		public DateTime? EndInsert { get; set; }

		[DisplayName("Procenta")]
    public int PercentIns { get; set; }
  }
}