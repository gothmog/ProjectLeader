using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MongoBaseRepository;
using MongoDB.Bson;
using ProjectLeader.Classes;
using ProjectLeader.Service.Iface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectLeader.Models;
using ProjectLeader.Helpers;
using MongoBaseRepository.Classes;
using System.Web.Mvc.Html;
using ProjectLeader.Classes.Enums;
using System.Linq.Expressions;

namespace ProjectLeader.Service
{
  public class TaskService: ITaskService
  {
    IMongoRepository db;

    public TaskService(IMongoRepository _db = null)
    {
      db = _db ?? DependencyResolver.Current.GetService<IMongoRepository>();
    }

    #region Tasks

    public DataSourceResult SelectTasksForProject([DataSourceRequest]DataSourceRequest request, string projectId, string taskId)
    {
      IList<MongoUser> users = db.GetCollection<MongoUser>(x => true).ToList();
      foreach (Kendo.Mvc.SortDescriptor sd in request.Sorts) sd.Member = ConvertTaskViewModel(sd.Member);
      foreach (Kendo.Mvc.FilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.FilterDescriptor))
      {
        fd.Member = ConvertTaskViewModel(fd.Member);
      }
      foreach (Kendo.Mvc.CompositeFilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor)) ConvertTaskFilters(fd);
      ObjectId id = ObjectId.Empty;
      if (ObjectId.TryParse(projectId, out id))
      {
        IQueryable<Task> query = db.GetCollection<Task>(x => x.ProjectId == ObjectId.Parse(projectId)).AsQueryable();
        query.Each(x => x.User = users.FirstOrDefault(y => y._id == x.UserId));
        return query.ToDataSourceResult(request, task => new TaskViewModel(task));
      }
      if (ObjectId.TryParse(taskId, out id))
      {
        IList<Task> tasks = GetConcreteTask(id);
        if (tasks.Any() && tasks[0].Tasks != null)
        {
          return tasks[0].Tasks.ToDataSourceResult(request, task => new TaskViewModel(task));
        }
      }
      return new DataSourceResult();
    }

    public IList<Task> SelectTasksForProject(string projectId)
    {
	    ObjectId id = ObjectId.Empty;
	    if (ObjectId.TryParse(projectId, out id))
	    {
		    IQueryable<Task> query = db.GetCollection<Task>(x => x.ProjectId == ObjectId.Parse(projectId)).AsQueryable();
		    return query.ToList();
	    }
		else return new List<Task>();
    }

    public DataSourceResult SelectTasksForProjectByUser([DataSourceRequest]DataSourceRequest request, string projectId, string userId)
    {
      IList<MongoUser> users = db.GetCollection<MongoUser>(x => true).ToList();
      foreach (Kendo.Mvc.SortDescriptor sd in request.Sorts) sd.Member = ConvertTaskViewModel(sd.Member);
      foreach (Kendo.Mvc.FilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.FilterDescriptor))
      {
        fd.Member = ConvertTaskViewModel(fd.Member);
      }
      foreach (Kendo.Mvc.CompositeFilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor)) ConvertTaskFilters(fd);
      ObjectId id = ObjectId.Empty;
      if (ObjectId.TryParse(projectId, out id))
      {
        IQueryable<Task> query = db.GetCollection<Task>(x => x.ProjectId == ObjectId.Parse(projectId)).AsQueryable();
        if (ObjectId.TryParse(userId, out id))
        {
          IList<Task> userTasks = new List<Task>();
          GetTasksByExpression(userTasks, query.ToList(), x => x.UserId == id);
          userTasks.Each(x => x.User = users.FirstOrDefault(y => y._id == x.UserId));
          return userTasks.ToDataSourceResult(request, task => new TaskViewModel(task));
        }
      }
      return new DataSourceResult();
    }

    private void ConvertTaskFilters(Kendo.Mvc.CompositeFilterDescriptor filter)
    {
      if (filter != null)
      {
        foreach (Kendo.Mvc.FilterDescriptor fd in filter.FilterDescriptors.Where(x => x is Kendo.Mvc.FilterDescriptor)) fd.Member = ConvertTaskViewModel(fd.Member);
        foreach (Kendo.Mvc.CompositeFilterDescriptor cfd in filter.FilterDescriptors.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor)) ConvertTaskFilters(cfd);
      }
    }

    private string ConvertTaskViewModel(string member)
    {
      switch (member)
      {
        case "Username": return "User.UserName";
      }
      return member;
    }

    public DataSourceResult GetProjects([DataSourceRequest]DataSourceRequest request)
    {
        IQueryable<Project> query = db.GetCollection<Project>(x=> true).AsQueryable();
        return query.ToDataSourceResult(request, project => new ProjectViewModel(project));
    }

    public IList<Task> GetConcreteTask(ObjectId id)
    {
      IList<Task> findedTask = new List<Task>();
      IList<Task> tasks = db.GetCollection<Task>(x => true);
      GetTaskFromHierarchy(tasks, id, findedTask, String.Empty);
      return findedTask;
    }

    public Task GetRootTaskForId(ObjectId id)
    {
      Task returnedTask = null;
      GetRootTask(id, ref returnedTask);
      return returnedTask;
    }

    public Project GetProjectIdForTask(Task t)
    {
      Task returnedTask = null;
      GetRootTask(t._id, ref returnedTask);
      if (returnedTask != null)
      {
        Project project = db.GetItem<Project>(x=> x._id == returnedTask.ProjectId);
        if (project != null) return project;
      }
      return null;
    }

    private void GetRootTask(ObjectId id, ref Task retTask)
    {
      IList<Task> tasks = GetConcreteTask(id);
      if (tasks.Any())
      {
        if (tasks.First().ProjectId != ObjectId.Empty)
        {
          retTask = tasks.First();
        }
        else GetRootTask(tasks.First().ParentTaskId, ref retTask);
      } 
    }

    private void GetTasksByExpression(IList<Task> tasks, IList<Task> tasksForObserve, Func<Task, bool> exp)
    {
      foreach (Task t in tasksForObserve.Where(exp))
      {
        tasks.Add(t);
        if (t.Tasks != null && t.Tasks.Any()) GetTasksByExpression(tasks, t.Tasks, exp);
      }
    }

    public Task GetConcreteTaskFromRoot(Task task, ObjectId id)
    {
      if (task.Tasks != null)
      {
        Task retTask = task;
        GetConcreteTaskFromRootRecursive(task.Tasks, id, ref retTask);
        return retTask;
      }
      return task;
    }

    private void GetConcreteTaskFromRootRecursive(IList<Task> tasks, ObjectId id, ref Task task)
    {
      if (tasks != null && tasks.FirstOrDefault(x => x._id == id) != null)
      {
        task = tasks.FirstOrDefault(x => x._id == id);
      }
      else
      {
				if (tasks != null)
				{
					foreach (Task t in tasks)
					{
						GetConcreteTaskFromRootRecursive(t.Tasks, id, ref task);
					}
				}
      }
    }

    private void GetTaskFromHierarchy(IList<Task> tasks, ObjectId id, IList<Task> findedTasks, string name)
    {
      if (tasks != null)
      {
        foreach (Task t in tasks)
        {
          if (String.IsNullOrEmpty(name)) name = t.Name;
          else name += ", " + t.Name;
          if (t._id == id)
          {
            findedTasks.Add(t);
          }
          else GetTaskFromHierarchy(t.Tasks, id, findedTasks, name);
        }
      }
    }

    #endregion

    #region Resources

    public DataSourceResult SelectResourcesForTask([DataSourceRequest]DataSourceRequest request, string taskId)
    {
      foreach (Kendo.Mvc.SortDescriptor sd in request.Sorts) sd.Member = ConvertTaskViewModel(sd.Member);
      foreach (Kendo.Mvc.FilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.FilterDescriptor))
      {
        fd.Member = ConvertTaskViewModel(fd.Member);
      }
      foreach (Kendo.Mvc.CompositeFilterDescriptor fd in request.Filters.Where(x => x is Kendo.Mvc.CompositeFilterDescriptor)) ConvertTaskFilters(fd);
      ObjectId id = ObjectId.Empty;
      if (ObjectId.TryParse(taskId, out id))
      {
        IList<Task> tasks = GetConcreteTask(id);
        if (tasks.Any() && tasks[0].Resources != null)
        {
          tasks[0].Resources.Each(x => x.ParentTaskIdString = taskId);
          return tasks[0].Resources.ToDataSourceResult(request);
        }
      }
      return new DataSourceResult();
    }

		#endregion
	}
}