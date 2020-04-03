using Kendo.Mvc.UI;
using MongoBaseRepository.Classes;
using MongoDB.Bson;
using ProjectLeader.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectLeader.Helpers;
using ProjectLeader.Models;

namespace ProjectLeader.Controllers
{
    public class HomeController : BaseController
    {
      public ActionResult Index()
      {
        if (Properties.Settings.Default.FirstTime)
        {

        }
        return View(GenerateViewModel());
      }

      public ActionResult ProjectIndex(string uid)
      {
	    ViewBag.Items =  GetTasksForProject(uid);
        return View(GenerateViewModel(null, uid));
      }

      public ActionResult ProjectIndexByUser(string uid)
      {
        ViewBag.UserId = AuthUserId.ToString();
        return View(GenerateViewModel(null, uid));
      }

      [HttpPost]
      public ActionResult GetProjects([DataSourceRequest]DataSourceRequest request)
      {
        DataSourceResult result = taskService.GetProjects(request);
        return this.Json(result, JsonRequestBehavior.AllowGet);
      }

      [HttpGet]
      public ActionResult CreateProject()
      {
        return View(new ProjectViewModel(){ ProjectName = ""});
      }

      [HttpPost]
      public ActionResult CreateProject(ProjectViewModel project)
      {
        db.AddItem(new Project()
        {
          Description = project.Description,
          Name = project.Name
        }, AuthUserId);
        return View("Index", GenerateViewModel());
      }

      private IList<TreeViewItemModel> GetTasksForProject(string id)
      {
	      var tasks = taskService.SelectTasksForProject(id);
		  IList<TreeViewItemModel> tasksview = new List<TreeViewItemModel>();
		  foreach (var task in tasks)
		  {
			  TreeViewItemModel item = new TreeViewItemModel();
			  AddTreeViewNode(item, task);
			  tasksview.Add(item);
		  }
	      return tasksview;
      }

      private void AddTreeViewNode(TreeViewItemModel item, Task task)
      {
	      item.Expanded = true;
	      item.Id = task._id.ToString();
	      item.Text = String.Format("{0} - stav: {1} - priorita: {2} ", task.Name, task.StateEnum.GetDisplayName(), task.PriorityEnum.GetDisplayName());
	      item.Url = "/Task/UpsertTask?taskId=" + task._id.ToString();
	      if (task.Tasks != null)
	      {
		      item.HasChildren = true;
		      foreach (var subtask in task.Tasks)
		      {
			      TreeViewItemModel subitem = new TreeViewItemModel();
			      AddTreeViewNode(subitem, subtask);
			      item.Items.Add(subitem);
		      }
	      }
      }
    }
}
