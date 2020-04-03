using System.Collections.Generic;
using MongoBaseRepository.Classes;
using ProjectLeader.Classes.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System;

namespace ProjectLeader.Classes
{
  public class Task: BaseNamedEntity
  {
    public string Description { get; set; }
    public MongoDB.Bson.ObjectId ProjectId { get; set; }
    public MongoDB.Bson.ObjectId ParentTaskId { get; set; }
    public MongoDB.Bson.ObjectId UserId { get; set; }
    public IList<Task> Tasks { get; set; }
    public IList<Resource> Resources { get; set; }
    public IList<History> Histories { get; set; }
    public IList<Comment> Comments { get; set; }
    public IList<Attachment> Attachments { get; set; }
    public StateEnum StateEnum { get; set; }
    public PriorityEnum PriorityEnum { get; set; }
    public TypeEnum TypeEnum { get; set; }
		public DateTime? Start { get; set; }
		public DateTime? End { get; set; }
    public int Percent { get; set; }
    [BsonIgnore]
    public MongoUser User { get; set; }
  }
}