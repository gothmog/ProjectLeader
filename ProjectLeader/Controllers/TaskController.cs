using Kendo.Mvc.UI;
using MongoDB.Bson;
using ProjectLeader.Classes;
using ProjectLeader.Models;
using ProjectLeader.Service;
using ProjectLeader.Service.Iface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ProjectLeader.Controllers
{
    public class TaskController : BaseController
    {
        IHornbachParser _hornbachParser;
        public TaskController(IHornbachParser hornbachParser = null)
        {
            _hornbachParser = hornbachParser;
        }

        [HttpPost]
        public ActionResult SelectTasksForProject([DataSourceRequest]DataSourceRequest request, string projectId, string taskId)
        {
            DataSourceResult result = taskService.SelectTasksForProject(request, projectId, taskId);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectTasksForProjectByUser([DataSourceRequest]DataSourceRequest request, string projectId, string userId)
        {
            DataSourceResult result = taskService.SelectTasksForProjectByUser(request, projectId, userId);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectResourcesForTask([DataSourceRequest]DataSourceRequest request, string taskId)
        {
            DataSourceResult result = taskService.SelectResourcesForTask(request, taskId);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddTask(TaskInsertViewModel task)
        {
            if (!String.IsNullOrEmpty(task.ProjectIdIns))
            {
                db.AddItem<Task>(new Task()
                {
                    Name = task.NameIns,
                    PriorityEnum = task.PriorityEnumIns,
                    StateEnum = task.StateEnumIns,
                    TypeEnum = task.TypeEnumIns,
                    Description = task.DescriptionIns,
                    ProjectId = ObjectId.Parse(task.ProjectIdIns),
                    UserId = ObjectId.Parse(task.UserIdIns),
                    Percent = task.PercentIns,
                    Start = task.StartInsert,
                    End = task.EndInsert
                }, AuthUserId);
                return RedirectToAction("ProjectIndex", "Home", new { @uid = task.ProjectIdIns });
            }
            else
            {
                ObjectId id = ObjectId.Parse(task.ParentTaskIdIns);
                Task rootTask = taskService.GetRootTaskForId(id);
                Task taskForAdd = rootTask._id != id ? taskService.GetConcreteTaskFromRoot(rootTask, id) : rootTask;
                if (taskForAdd.Tasks == null) taskForAdd.Tasks = new List<Task>();
                taskForAdd.Tasks.Add(new Task()
                {
                    _id = ObjectId.GenerateNewId(),
                    Name = task.NameIns,
                    PriorityEnum = task.PriorityEnumIns,
                    StateEnum = task.StateEnumIns,
                    TypeEnum = task.TypeEnumIns,
                    Description = task.DescriptionIns,
                    ParentTaskId = rootTask._id,
                    UserId = ObjectId.Parse(task.UserIdIns),
                    Percent = task.PercentIns,
                    Created = DateTime.Now,
                    CreatorId = AuthUserId,
                    Start = task.StartInsert,
                    End = task.EndInsert
                });
                db.UpdateItem(rootTask);
                return RedirectToAction("UpsertTask", "Task", new { taskId = id });
            }
        }

        [HttpGet]
        public ActionResult UpsertTask(string taskId)
        {
            db.InsertCacheItem<Task>(new Task());
            ObjectId oId = ObjectId.Empty;
            if (ObjectId.TryParse(taskId, out oId))
            {
                IList<Task> tasks = taskService.GetConcreteTask(oId);
                if (tasks != null && tasks.Any())
                {
                    Project project = taskService.GetProjectIdForTask(tasks.FirstOrDefault());
                    return View(new TaskEditViewModel(GenerateViewModel(project != null ? project.Name : null)) { Task = new TaskViewModel(tasks.FirstOrDefault()) });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpsertTaskSave(TaskViewModel model, bool withCols = false)
        {
            ObjectId id = ObjectId.Parse(model.IdString);
            Task rootTask = taskService.GetRootTaskForId(id);
            UpdateTask(model, rootTask._id != id ? taskService.GetConcreteTaskFromRoot(rootTask, id) : rootTask, withCols);
            db.UpdateItem(rootTask);
            return model.ProjectId != null && !model.ProjectId.StartsWith("0000") ? RedirectToAction("ProjectIndex", "Home", new { uid = model.ProjectId }) :
              RedirectToAction("UpsertTask", "Task", new { taskId = model.ParentTaskId });
        }

        private void UpdateTask(TaskViewModel model, Task task, bool withCols)
        {
            History hist = new History() { Created = DateTime.Now, CreatorId = AuthUserId };
            task.Description = model.Description;
            if (withCols)
            {
                task.Comments = model.Comments;
                task.Attachments = model.Attachments;
            }
            task.Name = model.Name;
            task.Percent = model.Percent;
            if (task.PriorityEnum != model.PriorityEnum)
            {
                hist.PriorityValuePast = task.PriorityEnum;
                task.PriorityEnum = model.PriorityEnum;
                hist.IsChanged = true;
            }
            if (task.TypeEnum != model.TypeEnum)
            {
                hist.TypeValuePast = task.TypeEnum;
                task.TypeEnum = model.TypeEnum;
                hist.IsChanged = true;
            }
            if (task.StateEnum != model.StateEnum)
            {
                hist.StateValuePast = task.StateEnum;
                task.StateEnum = model.StateEnum;
                hist.IsChanged = true;
            }
            if (task.UserId.ToString() != model.UserId)
            {
                hist.UserIdPast = task.UserId.ToString();
                task.UserId = ObjectId.Parse(model.UserId);
                hist.IsChanged = true;
            }
            if (task.Start != model.StartingDate)
            {
                hist.StartPast = task.Start;
                task.Start = model.StartingDate;
                hist.IsChanged = true;
            }
            if (task.End != model.EndingDate)
            {
                hist.EndPast = task.End;
                task.End = model.EndingDate;
                hist.IsChanged = true;
            }

            if (hist.IsChanged)
            {
                if (task.Histories == null) task.Histories = new List<History>();
                task.Histories.Add(hist);
            }
        }

        public ActionResult DeleteTask(string taskId)
        {
            ObjectId oId = ObjectId.Empty;
            if (ObjectId.TryParse(taskId, out oId))
            {
                db.RemoveItem<Task>(x => x._id == oId);
            }
            return RedirectToAction("Index", "Home");
        }

        #region Resources

        [HttpPost]
        public ActionResult UpsertResourceToTask(Resource model)
        {
            ObjectId parentId = ObjectId.Empty;
            if (ObjectId.TryParse(model.ParentTaskIdString, out parentId))
            {
                Task rootTask = taskService.GetRootTaskForId(parentId);
                Task concreteTask = taskService.GetConcreteTaskFromRoot(rootTask, parentId);
                model._id = ObjectId.GenerateNewId();
                model.Created = DateTime.Now;
                model.CreatorId = AuthUserId;
                if (concreteTask.Resources == null)
                {
                    concreteTask.Resources = new List<Resource>();
                    concreteTask.Resources.Add(model);
                }
                else
                {
                    Resource oldRes = concreteTask.Resources.FirstOrDefault(x => x.ResourceName == model.ResourceName);
                    if(oldRes != null)
                    {
                        model.CreatorId = oldRes.CreatorId;
                        model.Created = oldRes.Created;
                        concreteTask.Resources.Remove(oldRes);
                    }
                    concreteTask.Resources.Add(model);
                }
                db.UpdateItem(rootTask);
            }
            return RedirectToAction("UpsertTask", "Task", new { taskId = model.ParentTaskIdString });
        }

        [HttpGet]
        public JsonResult FindResource(string url)
        {
            return Json(_hornbachParser.GetResource(url), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteResource(string taskId, string resId)
        {
            ObjectId parentId = ObjectId.Empty;
            if (ObjectId.TryParse(taskId, out parentId))
            {
                Task rootTask = taskService.GetRootTaskForId(parentId);
                Task concreteTask = taskService.GetConcreteTaskFromRoot(rootTask, parentId);
                Resource res = concreteTask.Resources.First(x => x.ResourceIdString == resId);
                if (res != null) concreteTask.Resources.Remove(res);
                db.UpdateItem(rootTask);
            }
            return RedirectToAction("UpsertTask", "Task", new { taskId = taskId });
        }

        #endregion

        #region Comments

        [HttpPost]
        public ActionResult AddCommentToTask(string taskId, string text)
        {
            ObjectId parentId = ObjectId.Empty;
            if (ObjectId.TryParse(taskId, out parentId))
            {
                IList<Task> tasks = taskService.GetConcreteTask(parentId);
                if (tasks != null && tasks.Any())
                {
                    TaskViewModel task = new TaskViewModel(tasks.FirstOrDefault());
                    if (task.Comments == null) task.Comments = new List<Comment>();
                    task.Comments.Add(new Comment()
                    {
                        Author = task.Username,
                        Text = text,
                        _id = ObjectId.GenerateNewId(),
                        Created = DateTime.Now,
                        CreatorId = AuthUserId
                    });
                    return UpsertTaskSave(task, true);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Attachments

        public ActionResult AddAttachmentToTask(FormCollection col)
        {
            if (col.GetValue("TaskAttachmentId") != null)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string ext = Path.GetExtension(fileName);
                        Attachment att = new Attachment() { _id = ObjectId.GenerateNewId(), Created = DateTime.Now, CreatorId = AuthUserId, Name = fileName };
                        switch (ext)
                        {
                            case ".docx":
                            case ".doc": att.AttachmentType = Classes.Enums.AttachmentType.Doc; break;
                            case ".pdf": att.AttachmentType = Classes.Enums.AttachmentType.Pdf; break;
                            case ".gif":
                            case ".jpeg":
                            case ".png":
                            case ".bmp":
                            case ".jpg": att.AttachmentType = Classes.Enums.AttachmentType.Image; break;
                        }
                        var path = Path.Combine(Server.MapPath("~/TaskResources/"), fileName);
                        file.SaveAs(path);
                        var val = col.GetValue("TaskAttachmentId");
                        string id = col["TaskAttachmentId"];
                        ObjectId parentId = ObjectId.Empty;
                        if (ObjectId.TryParse(id, out parentId))
                        {
                            IList<Task> tasks = taskService.GetConcreteTask(parentId);
                            if (tasks != null && tasks.Any())
                            {
                                TaskViewModel task = new TaskViewModel(tasks.FirstOrDefault());
                                if (task.Attachments == null) task.Attachments = new List<Attachment>();
                                task.Attachments.Add(att);
                                UpsertTaskSave(task, true);
                                return RedirectToAction("UpsertTask", "Task", new { taskId = id });
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}