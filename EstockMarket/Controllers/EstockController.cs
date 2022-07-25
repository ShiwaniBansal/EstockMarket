using System;
using System.Linq;
using System.Web.Http;
using MongoDB.Driver;
using MongoDB.Bson;
using EstockMarket.Models;
using System.Configuration;

namespace EstockMarket.Controllers
{
    public class EstockController : ApiController
    {

        [HttpPost]
        [Route("api/v1.0/market/company/register")]
        public object AddCompany(Registration objVM)
        {
            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("EStock");
                var collection = DB.GetCollection<Registration>("Registration");

                var comp = collection.Find(Builders<Registration>.Filter.Where(s => s.Code == objVM.Code)).FirstOrDefault();
                if (comp != null && !string.IsNullOrEmpty(comp.Code))
                {
                    throw new Exception("Company code already exists");
                }
                else if (objVM.TurnOver <= 10)
                {
                    throw new Exception("Turn over must be greater than 10 crores.");
                }

                collection.InsertOne(objVM);
                return new Status
                { Result = "Success", Message = "Company Details Insert Successfully" };
            }
            catch (Exception ex)
            {
                return new Status
                { Result = "Error", Message = ex.Message.ToString() };
            }

        }

        [HttpPost]
        [Route("api/v1.0/market/stock/add")]
        public object AddStock(Stock stock)
        {
            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("EStock");

                var regCollection = DB.GetCollection<Registration>("Registration");
                var comp = regCollection.Find(Builders<Registration>.Filter.Where(s => s.Code == stock.Code)).FirstOrDefault();
                if (comp == null || string.IsNullOrEmpty(comp.Code))
                {
                    throw new Exception("Company code doesnot exist. Please enter valid company code.");
                }

                stock.Date = DateTime.Now;

                var collection = DB.GetCollection<Stock>("Stock");
                collection.InsertOne(stock);

                var update = regCollection.FindOneAndUpdateAsync(Builders<Registration>.Filter.Eq("Code", stock.Code), Builders<Registration>.Update.Set("lastStockPrice", stock.Price));

                return new Status
                { Result = "Success", Message = "Stock Details Insert Successfully" };

            }
            catch (Exception ex)
            {
                return new Status
                { Result = "Error", Message = ex.Message.ToString() };
            }
        }
        [HttpGet]
        [Route("api/v1.0/market/company/delete")]

        public object DeleteCompany(string code)
        {
            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("EStock");
                var collection = DB.GetCollection<Registration>("Registration");
                var stockCollection = DB.GetCollection<Stock>("Stock");
                var DeleteStock = stockCollection.DeleteMany(
                               Builders<Stock>.Filter.Eq("Code", code));
                var DeleteRecored = collection.DeleteOne(
                               Builders<Registration>.Filter.Eq("Code", code));
                return new Status
                { Result = "Success", Message = "Company Details Deleted  Successfully" };

            }
            catch (Exception ex)
            {
                return new Status
                { Result = "Error", Message = ex.Message.ToString() };
            }
        }
        [Route("api/v1.0/market/company/getall")]
        [HttpGet]
        public object GetAllCompanies()
        {
            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var db = Client.GetDatabase("EStock");
                var collection = db.GetCollection<Registration>("Registration").Find(new BsonDocument()).ToList();
                return Json(collection);
            }
            catch (Exception ex)
            {
                return new Status
                { Result = "Error", Message = ex.Message.ToString() };
            }
        }
        [Route("api/v1.0/market/company/info")]
        [HttpGet]
        public object GetCompanyByCode(string code)
        {
            try
            {
                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("EStock");
                var collection = DB.GetCollection<Registration>("Registration");
                var plant = collection.Find(Builders<Registration>.Filter.Where(s => s.Code == code)).FirstOrDefault();
                return Json(plant);
            }
            catch (Exception ex)
            {
                return new Status
                { Result = "Error", Message = ex.Message.ToString() };
            }

        }

        [Route("api/v1.0/market/stock/get")]
        [HttpGet]
        public object GetStock(string code, string startDate, string endDate)
        {
            try
            {
                DateTime dStartDate = DateTime.Parse(startDate);
                DateTime dEndDate = DateTime.Parse(endDate);

                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("EStock");
                var collection = DB.GetCollection<Stock>("Stock");
                var plant = collection.Find(Builders<Stock>.Filter.Where(s => s.Code == code && s.Date >= dStartDate && s.Date <= dEndDate)).ToList();
                return Json(plant);
            }
            catch (Exception ex)
            {
                return new Status
                { Result = "Error", Message = ex.Message.ToString() };
            }
        }
    }
}