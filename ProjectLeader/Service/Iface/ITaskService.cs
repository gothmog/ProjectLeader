using Kendo.Mvc.UI;
using MongoDB.Bson;
using ProjectLeader.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLeader.Service.Iface
{
  public interface ITaskService
  {
    DataSourceResult SelectTasksForProject([DataSourceRequest]DataSourceRequest request, string projectId, string taskId);
    DataSourceResult SelectTasksForProjectByUser([DataSourceRequest]DataSourceRequest request, string projectId, string userId);
    DataSourceResult SelectResourcesForTask([DataSourceRequest]DataSourceRequest request, string taskId);
    IList<Task> GetConcreteTask(ObjectId id);
    Task GetRootTaskForId(ObjectId id);
    Project GetProjectIdForTask(Task t);
    Task GetConcreteTaskFromRoot(Task task, ObjectId id);
    DataSourceResult GetProjects([DataSourceRequest] DataSourceRequest request, string uid);
    IList<Task> SelectTasksForProject(string projectId);
  }
}