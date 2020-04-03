using MongoBaseRepository;
using MongoBaseRepository.Classes;
using MongoDB.Bson;
using ProjectLeader.Classes;
using ProjectLeader.Service.Iface;
using ProjectLeader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectLeader.Controllers
{
  [Authorize]
  public class BaseController : Controller
  {
    protected IMongoRepository db;
    protected ITaskService taskService;
    protected ObjectId AuthUserId
    {
      get
      {
        MongoUser AuthUser = GetAuthUser();
        return AuthUser != null ? AuthUser._id : ObjectId.Empty;
      }
    }
    
    public BaseController(IMongoRepository _db = null, ITaskService _taskService = null)
    {
      db = _db ?? DependencyResolver.Current.GetService<IMongoRepository>();
      taskService = _taskService ?? DependencyResolver.Current.GetService<ITaskService>();
    }

    protected MongoUser GetAuthUser()
    {
      if (System.Web.HttpContext.Current.User != null) return db.GetItem<MongoUser>(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name);
      return null;
    }

    protected BaseViewModel GenerateViewModel(string name = null, string uid = null)
    {
      BaseViewModel model = new BaseViewModel();
      Project project = String.IsNullOrEmpty(name) && String.IsNullOrEmpty(uid) ? new Project() : !String.IsNullOrEmpty(name) ? db.GetItem<Project>(x => x.Name == name): db.GetItem<Project>(x => x._id == ObjectId.Parse(uid));
      if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(uid))
      {
        model.ProjectName = "Přehled projektů";
        model.ProjectDescription = "Seznam projektů k dispozici";
      }
      else
      {
        model.ProjectId = project._id;
        model.ProjectName = project.Name;
        model.ProjectDescription = project.Description;
      }
      return model;
    }
  }
}