using MongoBaseRepository.Classes;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;

namespace ProjectLeader.Classes
{
  public class Resource: BaseNamedEntity
  {
    [DisplayName("Popis")]
    public string Description { get; set; }
    [DisplayName("Cena")]
    public decimal Price { get; set; }
    [DisplayName("Počet")]
    public decimal Count { get; set; }

    [BsonIgnore]
    public string ParentTaskIdString { get; set; }

    [BsonIgnore]
    public decimal Sum
    {
      get
      {
        return Price * Count;
      }
    }

    [BsonIgnore]
    public string IdString
    {
      get
      {
        return _id.ToString();
      }
    }
  }
}