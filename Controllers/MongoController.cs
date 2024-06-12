using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Components.Routing;
using MVP_DashBoard.Models;
using MVP_DashBoard.AppData;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVP_DashBoard.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class MongoController : ControllerBase
    {
        private static IConfiguration _configuration;
        private IMongoCollection<BsonDocument> collection;
        private readonly IMongoDatabase database;
        private readonly DataAcess _dataAcess;

        public MongoController(IConfiguration configuration, DataAcess dataAcess)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dataAcess = dataAcess ?? throw new ArgumentNullException(nameof(dataAcess));

            MongoClient client = new MongoClient(GetNewDbCS());
            database = client.GetDatabase("UttamDairy_MVP");
            if (_dataAcess.receptionMongoResult == null)
                _dataAcess.receptionMongoResult = GetReceptionDataMongo();
            if (_dataAcess.dispatchMongoResult == null)
                _dataAcess.dispatchMongoResult = GetDispatchDataMongo();
        }
        private static string GetOldDbCS()
        {
            return _configuration.GetConnectionString("Uttam_OldDb") ?? throw new InvalidOperationException("Connection string 'Uttam_OldDb' not found.");
        }

        private static string GetNewDbCS()
        {
            return _configuration.GetConnectionString("Uttam_NewDbCS") ?? throw new InvalidOperationException("Connection string 'Uttam_NewDbCS' not found.");
        }

        // GET: api/<MongoController>
        //[HttpGet("GetMongoData")]
        private List<ReceptionData> GetReceptionDataMongo()
        {
            try
            {
                var collection = database.GetCollection<BsonDocument>("Reception");

                var startDate = new DateTime(2023, 4, 1, 0, 0, 1);
                var endDate = new DateTime(2024, 3, 31, 23, 59, 59);

                var sort = Builders<BsonDocument>.Sort.Ascending("Date_In");

                var documents = collection.Find(new BsonDocument()).Sort(sort).ToList();
                //var documents = collection.Find(filter).Limit(10).ToList();

                List<ReceptionData> receptionList = documents.Select(doc => new ReceptionData
                {
                    Id = doc.GetValue("Vehicle_ID").AsString,
                    VehicleNumber = doc.GetValue("Vehicle_Number").AsString,
                    RouteId = doc.GetValue("Route_ID").AsInt32,
                    RouteName = doc.GetValue("Route_Name").AsString,
                    RouteUnion = doc.GetValue("Route_Union").AsInt32,
                    SupplierName = doc.GetValue("Supplier_Name").AsString,
                    ProductGroup = doc.GetValue("Product_Group").AsString,
                    ProductName = doc.GetValue("Product_Name").AsString,
                    Qty = doc.GetValue("Quantity").AsDouble,
                    DateIn = doc.GetValue("Date_In").ToUniversalTime(),
                    DateOut = doc.GetValue("Date_Out").ToUniversalTime(),
                    UpdatedOn = doc.GetValue("Updated_On").ToUniversalTime()
                }).ToList();

                return receptionList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[HttpGet("GetMongoData")]
        private List<DispatchData> GetDispatchDataMongo()
        {
            try
            {
                var collection = database.GetCollection<BsonDocument>("Dispatch");

                var startDate = new DateTime(2023, 4, 1, 0, 0, 1);
                var endDate = new DateTime(2024, 3, 31, 23, 59, 59);

                var sort = Builders<BsonDocument>.Sort.Ascending("Date");

                var documents = collection.Find(new BsonDocument()).Sort(sort).ToList();
                //var documents = collection.Find(filter).Limit(10).ToList();

                List<DispatchData> dispatchList = documents.Select(doc => new DispatchData
                {
                    Date = doc.Contains("Date") ? doc.GetValue("Date").ToUniversalTime() : DateTime.MinValue,
                    Qty = doc.Contains("Qty") ? (doc["Qty"].IsInt32 ? Convert.ToDouble(doc["Qty"].AsInt32) : doc["Qty"].AsDouble) : 0.0,
                    ProductGrp = doc.Contains("Product_Grp") ? doc.GetValue("Product_Grp").AsString ?? "" : "",
                    ProductName = doc.Contains("Product_Name") ? doc.GetValue("Product_Name").AsString ?? "" : "",
                    DispatchType = doc.Contains("Dispatch_Type") ? doc.GetValue("Dispatch_Type").AsString ?? "" : ""
                }).ToList();

                return dispatchList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //[HttpGet("Reception_Last_7Days")]
        public List<ReceptionResult> GetRecpData(DateTime dt)
        {
            List<ReceptionData> receptionList = new List<ReceptionData>();

            if (_dataAcess.receptionMongoResult == null)
                receptionList = GetReceptionDataMongo();
            else
                receptionList = _dataAcess.receptionMongoResult;

            var result = receptionList
                .Where(r => r.DateIn <= dt && r.DateIn >= dt.AddDays(-7))
                .GroupBy(r => new { r.DateIn.Date, r.RouteUnion })
                .Select(g => new ReceptionResult { Date = g.Key.Date, TotalQuantity = g.Sum(r => r.Qty), RouteUnion = g.Key.RouteUnion})
                .OrderBy(r => r.Date)
                .Cast<ReceptionResult>()
                .ToList();

            return result;
        }

        public class DispatchResult
        {
            public DateTime Date { get; set; }
            public string? ProductGroup { get; set; }
            public double TotalQuantity { get; set; }
        }

        //[HttpGet("Dispatch_Last_7Days")]
        public List<DispatchResult> GetDispData(DateTime dt)
        {
            List<DispatchData> dispatchList = new List<DispatchData>();

            if (_dataAcess.dispatchMongoResult == null)
                dispatchList = GetDispatchDataMongo();
            else
                dispatchList = _dataAcess.dispatchMongoResult;

            var result = dispatchList
                .Where(r => r.Date <= dt && r.Date >= dt.AddDays(-7))
                .GroupBy(r => new { r.Date.Date, r.ProductGrp })
                .Select(g => new DispatchResult
                {
                    Date = g.Key.Date,
                    ProductGroup = g.Key.ProductGrp,
                    TotalQuantity = g.Sum(r => r.Qty)
                })
                .OrderBy(r => r.Date)
                .Cast<DispatchResult>()
                .ToList();

            return result;
        }

        //[HttpGet("GetReceptionTableData")]
        public List<ReceptionResult> GetReceptionTableData(DateTime dt)
        {
            DateTime dt_sameday_lastyr = dt.AddYears(-1);
            DateTime dt_prevday = dt.AddDays(-1);
            DateTime dt_prevday_lastyr = dt_prevday.AddYears(-1);
            //Dates for Current Month
            DateTime dt_MonthStart = new DateTime(dt.Year, dt.Month, 01);
            DateTime dt_MonthEnd = dt;
            //Dates for Previous Year same month
            DateTime dt_sameday_PrevYr_MonthStart = new DateTime(dt.AddYears(-1).Year, dt.Month, 01);
            DateTime dt_sameday_PrevYr_MonthEnd = dt.AddYears(-1);

            DateTime dt_currentYr_Start = (dt.Month >= 4) ? new DateTime(dt.Year, 04, 01, 00, 00, 00) : new DateTime(dt.AddYears(-1).Year, 04, 01, 00, 00, 00);
            DateTime dt_currentYr_End = (dt.Month >= 4) ? new DateTime(dt.AddYears(1).Year, 03, 31, 00, 00, 00) : new DateTime(dt.Year, 03, 31, 00, 00, 00);

            DateTime dt_prevYr_Start = dt_currentYr_Start.AddYears(-1);
            DateTime dt_prevYr_End = dt_currentYr_End.AddYears(-1);

            List<ReceptionData> receptionList = new List<ReceptionData>();

            if (_dataAcess.receptionMongoResult == null)
                receptionList = GetReceptionDataMongo();
            else
                receptionList = _dataAcess.receptionMongoResult;

            var specificDates = new List<DateTime> { dt.Date, dt_sameday_lastyr.Date, dt_prevday.Date, dt_prevday_lastyr.Date };

            var distinctRouteNames = receptionList
                .Where(r => specificDates.Contains(r.DateIn.Date))
                .Select(r => r.RouteName)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            List<ReceptionResult> result = new List<ReceptionResult>();

            foreach (string routes in distinctRouteNames)
            {
                List<ReceptionResult> temp_result = receptionList
                    .Where(r => specificDates.Contains(r.DateIn.Date) && routes == r.RouteName.Trim().ToString())
                    .GroupBy(r => new { r.DateIn.Date,  r.RouteUnion , r.RouteName })
                    .Select(g => new ReceptionResult
                    {
                        Date = g.Key.Date.Date,
                        RouteName = g.Key.RouteName,
                        RouteUnion = g.Key.RouteUnion,
                        TotalQuantity = g.Sum(r => r.Qty)
                    })
                    .OrderByDescending(r => r.Date)
                    .ThenBy(r => r.RouteUnion)
                    .Cast<ReceptionResult>()
                    .ToList();

                //For the BMC/CC whose transactions were not hapenned add an entry with 0 value
                if (temp_result.Count < specificDates.Count)
                {
                    foreach (DateTime date in specificDates)
                    {
                        if (!temp_result.Any(r => r.Date == date))
                        {
                            //int source = receptionList
                            //    .Where(r => r.RouteName.Trim() == routes)
                            //    .Select(r => r.RouteUnion )
                            //    .ToString(); // Default to "Other DCs" if no matching entry

                            //temp_result.Add(new ReceptionResult
                            //{
                            //    Date = date,
                            //    RouteName = routes,
                            //    RouteUnion = source, // Or some other default value
                            //    TotalQuantity = 0
                            //});
                        }
                    }
                }

                result = result.Concat(temp_result).ToList<ReceptionResult>();
            }

            List<ReceptionResult> additionalresults = receptionList
                .Where(r => r.DateIn >= dt_MonthStart && r.DateIn <= dt_MonthEnd)
                .GroupBy(r => new { Month = r.DateIn.Month, Year = r.DateIn.Year,  r.RouteUnion , r.RouteName })
                .Select(g => new ReceptionResult
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    RouteName = g.Key.RouteName,
                    RouteUnion = g.Key.RouteUnion,
                    TotalQuantity = g.Sum(r => r.Qty)
                })
                .OrderByDescending(r => r.Date)
                .ThenBy(r => r.RouteUnion)
                .Cast<ReceptionResult>()
                .ToList();

            //result = result.Concat(additionalresults).ToList<ReceptionResult>();

            List<ReceptionResult> returnvar = new List<ReceptionResult>();

            foreach (DateTime temp_dt in specificDates)
            {
                var temp_result = result.Where(r => r.Date == temp_dt.Date).Select(g => new ReceptionResult
                {
                    Date = g.Date.Date,
                    RouteName = g.RouteName,
                    RouteUnion = g.RouteUnion,
                    TotalQuantity = g.TotalQuantity
                }).OrderBy(r => r.RouteUnion).ThenBy(r => r.RouteName).ToArray();

                returnvar = returnvar.Concat(temp_result).ToList<ReceptionResult>();
            }

            return returnvar;
        }

        //[HttpGet("GetDispatchDatafromSQLServer")]
        private async Task<IEnumerable<dynamic>> GetDispatch()
        {
            using (var connection = new SqlConnection(GetOldDbCS()))
            {
                var query = @"DECLARE @ColumnNames NVARCHAR(MAX);
            SELECT @ColumnNames = STUFF(
                (SELECT ', ' + name
                 FROM sys.columns
                 WHERE object_id = OBJECT_ID('dbo.X_PouchDispatch')
                    AND name NOT IN ('Id', 'Dispatch_Date', 'VendorNo', 'VehicleId', 'VehicleNo', 'InvoiceNo', 'PartyName', 'TotalCrates', 'TotalQty')
                    AND name NOT LIKE 'C%'
                 FOR XML PATH('')), 
                1, 2, '');

            DECLARE @sql NVARCHAR(MAX);
            SELECT @sql = '
            SELECT * FROM (
                -- Bulk Dispatch
                SELECT 
                    BD.BMILKDISPx_TDate AS Date,
                    ISNULL(CAST(
                        (SELECT WEBRm_NetWt    
                         FROM dbo.M_WeighBridge AS WB    
                         WHERE WB.WEBRm_VehicleID = BD.BMILKDISPx_WEBRm_VehicleID
                        ) AS FLOAT), 0.0) AS Qty,
                    ''Milk'' AS Product_Grp,
                    ISNULL((SELECT PRDm_Name FROM dbo.M_Product AS P WHERE P.PRDm_Id = BD.BMILKDISPx_PRDm_Id), ''Milk'') AS Product_Name,
                    ''Bulk Milk'' AS Dispatch_Type
                FROM X_BULK_DISPATCH AS BD

                UNION ALL

                -- Pouch Dispatch
                SELECT 
                    Dispatch_Date AS Date,
                    SUM(Qty) AS Qty, 
                    CASE 
                        WHEN COLUMN_NAME LIKE ''%gold%'' OR COLUMN_NAME LIKE ''%taz%'' OR COLUMN_NAME LIKE ''%milk%'' OR COLUMN_NAME LIKE ''%snt%'' OR COLUMN_NAME LIKE ''%tsp%'' OR COLUMN_NAME LIKE ''%abf%'' OR COLUMN_NAME LIKE ''%acw%'' OR COLUMN_NAME LIKE ''%sha%''
                        THEN ''Milk''
                        WHEN COLUMN_NAME LIKE ''%Ghee%'' THEN ''Ghee''
                        WHEN COLUMN_NAME LIKE ''%pnr%'' OR COLUMN_NAME LIKE ''%pan%'' THEN ''Paneer''
                        WHEN COLUMN_NAME LIKE ''%dah%'' OR COLUMN_NAME LIKE ''%dahi%'' THEN ''Curd''
                        WHEN COLUMN_NAME LIKE ''%bm%'' OR COLUMN_NAME LIKE ''%abt%'' THEN ''Butter Milk''
                        WHEN COLUMN_NAME LIKE ''%butter%'' THEN ''Butter''
                        ELSE ''Others''
                    END AS Product_Grp,
                    CASE 
                        WHEN COLUMN_NAME LIKE ''%gold%'' OR COLUMN_NAME LIKE ''%taz%'' OR COLUMN_NAME LIKE ''%milk%'' OR COLUMN_NAME LIKE ''%snt%'' OR COLUMN_NAME LIKE ''%tsp%'' OR COLUMN_NAME LIKE ''%abf%'' OR COLUMN_NAME LIKE ''%acw%'' OR COLUMN_NAME LIKE ''%sha%''
                        THEN ''Milk''
                        WHEN COLUMN_NAME LIKE ''%Ghee%'' THEN ''Ghee''
                        WHEN COLUMN_NAME LIKE ''%pnr%'' OR COLUMN_NAME LIKE ''%pan%'' THEN ''Paneer''
                        WHEN COLUMN_NAME LIKE ''%dah%'' OR COLUMN_NAME LIKE ''%dahi%'' THEN ''Curd''
                        WHEN COLUMN_NAME LIKE ''%bm%'' OR COLUMN_NAME LIKE ''%abt%'' THEN ''Butter Milk''
                        WHEN COLUMN_NAME LIKE ''%butter%'' THEN ''Butter''
                        ELSE ''Others''
                    END AS Product_Name,
                    ''Pouch Dispatch'' AS Dispatch_Type
                FROM (
                    SELECT 
                        Dispatch_Date, 
                        Product AS COLUMN_NAME, 
                        Qty
                    FROM (
                        SELECT 
                            Dispatch_Date, '+@ColumnNames+' 
                        FROM 
                            X_PouchDispatch
                    ) AS p
                    UNPIVOT (Qty FOR Product IN ('+@ColumnNames+')) AS unpvt
                ) AS categorized
                GROUP BY 
                    Dispatch_Date, 
                    CASE 
                        WHEN COLUMN_NAME LIKE ''%gold%'' OR COLUMN_NAME LIKE ''%taz%'' OR COLUMN_NAME LIKE ''%milk%'' OR COLUMN_NAME LIKE ''%snt%'' OR COLUMN_NAME LIKE ''%tsp%'' OR COLUMN_NAME LIKE ''%abf%'' OR COLUMN_NAME LIKE ''%acw%'' OR COLUMN_NAME LIKE ''%sha%''
                        THEN ''Milk''
                        WHEN COLUMN_NAME LIKE ''%Ghee%'' THEN ''Ghee''
                        WHEN COLUMN_NAME LIKE ''%pnr%'' OR COLUMN_NAME LIKE ''%pan%'' THEN ''Paneer''
                        WHEN COLUMN_NAME LIKE ''%dah%'' OR COLUMN_NAME LIKE ''%dahi%'' THEN ''Curd''
                        WHEN COLUMN_NAME LIKE ''%bm%'' OR COLUMN_NAME LIKE ''%abt%'' THEN ''Butter Milk''
                        WHEN COLUMN_NAME LIKE ''%butter%'' THEN ''Butter''
                        ELSE ''Others''
                    END
            ) AS tbl 
            WHERE Date > ''2020-01-01 00:00:00''
            ORDER BY Date;'

            EXEC sp_executesql @sql;";
                return await connection.QueryAsync<dynamic>(query);
            }
        }

        //[HttpPost("InsertIntoMongoDBDispatch")]
        public async Task<IActionResult> InsertDataDispatch()
        {
            var data = await GetDispatch();
            collection = database.GetCollection<BsonDocument>("Dispatch");

            var documents = new List<BsonDocument>();
            foreach (var item in data)
            {
                var document = new BsonDocument();
                foreach (var property in (IDictionary<string, object>)item)
                {
                    document.Add(property.Key, BsonValue.Create(property.Value));
                }
                documents.Add(document);
            }

            await collection.InsertManyAsync(documents);
            return Ok($"{documents.Count} documents were inserted");
        }


        // POST api/<MongoController>
        //[HttpPost("InsertIntoMongoDBReception")]
        public async Task<IActionResult> InsertDataReception()
        {
            var data = await GetReception1();
            collection = database.GetCollection<BsonDocument>("Reception");

            var documents = new List<BsonDocument>();
            foreach (var item in data)
            {
                var document = new BsonDocument();
                foreach (var property in (IDictionary<string, object>)item)
                {
                    document.Add(property.Key, BsonValue.Create(property.Value));
                }
                documents.Add(document);
            }

            await collection.InsertManyAsync(documents);
            return Ok($"{documents.Count} documents were inserted");
        }

        //[HttpGet("GetReceptionDatafromSQLServer")]
        public async Task<IEnumerable<dynamic>> GetReception1()
        {
            using (var connection = new SqlConnection(GetOldDbCS()))
            {
                var query = @"SELECT
                    WB.WEBRm_VehicleID AS Vehicle_ID,
                    V.VEHICLEm_Number AS Vehicle_Number,
                    WB.WEBRm_ROUTm_Id as Route_ID,
                    R.ROUTm_Name AS Route_Name,
                    CASE 
                        WHEN WB.WEBRm_ROUTm_Id IN (7, 8, 9, 6, 68, 69) THEN 1
                        ELSE 0 
                    END AS Route_Union,
                    S.SUPPLIERm_Name AS Supplier_Name,
                    PG.PRDGm_Name AS Product_Group,
                    P.PRDm_Name AS Product_Name,
                    WB.WEBRm_NetWt AS Quantity,
                    WB.WEBRm_DateIn AS Date_In,
                    WB.WEBRm_DateOut AS Date_Out,
                    WB.WEBRm_UpdatedOn AS Updated_On
                FROM 
                    [UttamDairy].[dbo].[M_WeighBridge] AS WB
                LEFT JOIN 
                    [UttamDairy].[dbo].[M_Vehicle] AS V ON WB.WEBRm_VEHICLEm_Id = V.VEHICLEm_Id
                LEFT JOIN 
                    [UttamDairy].[dbo].[M_Route] AS R ON WB.WEBRm_ROUTm_Id = R.ROUTm_Id
                LEFT JOIN 
                    [UttamDairy].[dbo].[M_Supplier] AS S ON WB.WEBRm_SUPPLIERm_Id = S.SUPPLIERm_Id
                LEFT JOIN 
                    [UttamDairy].[dbo].[M_ProductGroup] AS PG ON WB.WEBRm_PRDGm_Id = PG.PRDGm_Code
                LEFT JOIN 
                    [UttamDairy].[dbo].[M_Product] AS P ON WB.WEBRm_PRDm_Id = P.PRDm_Id
                WHERE 
                    WEBRm_DateIn < '2023-01-01 00:00:00' and  WEBRm_DateIn > '2020-01-01 00:00:00'
                    and WB.WEBRm_NetWt > 0
                    and WB.WEBRm_Purpose = 'UnLoading'
                    and R.ROUTm_Name is not null
                    and WB.WEBRm_ROUTm_Id is not null
                    and S.SUPPLIERm_Name is not null
                    and PG.PRDGm_Name is not null
                    and P.PRDm_Name is not null
                    and WB.WEBRm_LOCATIONID = 1
                ORDER BY WB.WEBRm_DateIn ASC";

                return await connection.QueryAsync<dynamic>(query);
            }
        }
    }
}
