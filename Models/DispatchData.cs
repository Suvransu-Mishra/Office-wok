using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MVP_DashBoard.Models
{
    [BsonIgnoreExtraElements]
    public class DispatchData
    {
        [BsonElement("Date")]
        public DateTime Date { get; set; } = DateTime.Now;
        [BsonElement("Qty")]
        public double Qty { get; set; } 
        [BsonElement("Product_Grp")]
        public string? ProductGrp { get; set; }
        [BsonElement("Product_Name")]
        public string? ProductName { get; set; }
        [BsonElement("Dispatch_Type")]
        public string? DispatchType { get; set; }
    }
}
