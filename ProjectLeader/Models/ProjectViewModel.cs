using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using ProjectLeader.Classes;

namespace ProjectLeader.Models
{
  public class ProjectViewModel: BaseViewModel
  {
    public ProjectViewModel()
    {

    }

    public ProjectViewModel(Project project)
    {
      Id = project._id.ToString();
      Name = project.Name;
      Description = project.Description;
    }
	
	[DisplayName("Id")]
    public string Id { get; set; }
    [DisplayName("Název úkolu")]
    public string Name { get; set; }
    [DisplayName("Popis")]
    public string Description { get; set; }
    public string IdString { get; set; }
  }
}