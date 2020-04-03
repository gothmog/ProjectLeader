using MongoBaseRepository.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLeader.Classes
{
  public class Comment: BaseEntity
  {
    public string Text { get; set; }
    public string Author { get; set; }
  }
}