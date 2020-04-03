using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLeader.Models
{
  public class BaseViewModel
  {
    public BaseViewModel() { }
    public BaseViewModel(BaseViewModel model)
    {
      ProjectId = model.ProjectId;
      ProjectName = model.ProjectName;
      ProjectDescription = model.ProjectDescription;
    }

    public ObjectId ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
  }
}