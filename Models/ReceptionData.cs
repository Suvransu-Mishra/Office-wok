using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MVP_DashBoard.Models
{
    [BsonIgnoreExtraElements]
    public class ReceptionData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Vehicle_ID")]
        public string Id { get;set; } = String.Empty;
        [BsonElement("Vehicle_Number")]
        public string VehicleNumber { get; set; } = String.Empty;
        [BsonElement("Route_ID")]
        public int RouteId {  get; set; }
        [BsonElement("Route_Name")]
        public string RouteName { get; set; } = String.Empty;
        [BsonElement("Route_Union")]
        public int RouteUnion { get; set; }
        [BsonElement("Supplier_Name")]
        public string SupplierName {  get; set; } = String.Empty;
        [BsonElement("Product_Group")]
        public string ProductGroup {  get; set; } = String.Empty;
        [BsonElement("Product_Name")]
        public string ProductName { get; set; } = String.Empty;
        [BsonElement("Quantity")]
        public double Qty { set; get; }
        [BsonElement("Date_In")]
        public DateTime DateIn { set; get; }
        [BsonElement("Date_Out")]
        public DateTime DateOut { set; get; }
        [BsonElement("Updated_On")]
        public DateTime UpdatedOn { set; get; }
    }

    public class ReceptionResult    
    {
        public DateTime Date { get; set; }
        public string? RouteName { get; set; }
        public int? RouteUnion { get; set; }
        public double TotalQuantity { get; set; }
    }

}
