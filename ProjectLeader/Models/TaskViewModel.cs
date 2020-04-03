using MongoBaseRepository;
using MongoBaseRepository.Classes;
using MongoDB.Bson;
using ProjectLeader.Classes;
using ProjectLeader.Classes.Enums;
using ProjectLeader.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectLeader.Models
{
  public class TaskViewModel
  {
    public ObjectId Id { get; set; }
    [DisplayName("Název úkolu")]
    public string Name { get; set; }
    [DisplayName("Popis")]
    public string Description { get; set; }
    [DisplayName("Stav")]
    public StateEnum StateEnum { get; set; }
    [DisplayName("Priorita")]
    public PriorityEnum PriorityEnum { get; set; }
    [DisplayName("Typ")]
    public TypeEnum TypeEnum { get; set; }
    [DisplayName("Uživatel")]
    [Description("MongoUser")]
    public string UserId { get; set; }
    public string Username { get; set; }
    public string ParentTaskId { get; set; }
    public string ProjectId { get; set; }
    public string IdString { get; set; }
		[DisplayName("Začátek")]
		public DateTime? Start { get; set; }
		[DisplayName("Konec")]
		public DateTime? End { get; set; }

		[DisplayName("Procenta")]
    public int Percent { get; set; }

    public IList<Comment> Comments { get; set; }
    public IList<Attachment> Attachments { get; set; }

    public static IDictionary<string, string> ExportColumns
    {
      get
      {
        IDictionary<string, string> exp = new Dictionary<string, string>();
        exp.Add("Name", "Úkol");
        exp.Add("Description", "Popis");
        exp.Add("StateEnum", "Stav");
        exp.Add("PriorityEnum", "Priorita");
        exp.Add("TypeEnum", "Typ");
        exp.Add("UserId", "Uživatel");
        exp.Add("Percent", "Procenta");
				exp.Add("Start", "Začátek");
				exp.Add("End", "Konec");
				return exp;
      }
    }


    public TaskViewModel()
    {

    }

    public TaskViewModel(Task task)
    {
      MongoUser user = DependencyResolver.Current.GetService<IMongoRepository>().GetItem<MongoUser>(x => x._id == task.UserId);
      Id = task._id;
      Name = task.Name;
      Description = task.Description;
      StateEnum = task.StateEnum;
      PriorityEnum = task.PriorityEnum;
      TypeEnum = task.TypeEnum;
      UserId = task.UserId.ToString();
      Username = user != null ? user.UserName : String.Empty;
      ProjectId = task.ProjectId.ToString();
      ParentTaskId = task.ParentTaskId.ToString();
      IdString = task._id.ToString();
			Start = task.Start;
			End = task.End;
      Attachments = task.Attachments ?? new List<Attachment>();
      Comments = task.Comments ?? new List<Comment>();
      Percent = task.Tasks != null && task.Tasks.Any() ? task.Tasks.Sum(x => x.Percent) / task.Tasks.Count : task.Percent;
    }
  }
}