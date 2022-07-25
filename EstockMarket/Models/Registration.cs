using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EstockMarket.Models
{
    public class Registration
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CEO { get; set; }
        public double TurnOver { get; set; }
        public string Website { get; set; }
        public string StockType { get; set; }
        public double lastStockPrice { get; set; }
    }
}
public class Status
{
    public string Result { set; get; }
    public string Message { set; get; }
}
public class Stock
{
    [BsonRepresentation(BsonType.ObjectId)]
    public String Id { get; set; }
    public string Code { set; get; }
    public double Price { set; get; }
    public DateTime Date { set; get; }


}