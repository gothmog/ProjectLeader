using ProjectLeader.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLeader.Models
{
  public class TaskEditViewModel: BaseViewModel
  {
    public TaskEditViewModel(BaseViewModel model) : base(model) { }
    public TaskViewModel Task { get; set; }
  }
}