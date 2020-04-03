using MongoBaseRepository.Classes;
using ProjectLeader.Classes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLeader.Classes
{
  public class Attachment : BaseNamedEntity
  {
    public AttachmentType AttachmentType { get; set; }
  }
}