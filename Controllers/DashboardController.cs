using Microsoft.AspNetCore.Mvc;
using MVP_DashBoard.Models;
using System;
using System.Configuration;
using MVP_DashBoard.AppData;

namespace MVP_DashBoard.Controllers
{
    public class DashboardController : Controller
    {
        private static IConfiguration _configuration;
        private readonly DataAcess _dataAcess;
        public readonly MongoController mongo;
        //public DashboardController(IConfiguration configuration, DataAcess dataAcess)
        //{
        //    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        //    _dataAcess = dataAcess ?? throw new ArgumentNullException(nameof(dataAcess));
        //    mongo = new MongoController(_configuration, _dataAcess);
        //}



        [HttpGet]
        public IActionResult Index()
        {
            DateModel model = new DateModel();
            model.Date = DateTime.Now.AddDays(-1).Date;
            return Index(model);
        }

        [HttpPost]
        public IActionResult Index(DateModel model)
        {
            model.Date = model.Date.AddDays(1).AddTicks(-1);
            //List<ReceptionResult> recpResult = mongo.GetRecpData(model.Date);

            //var chartData = new
            //{
            //    label1 = "From DC's",
            //    label2 = "Other DC's",
            //    label3 = "Total Quantity",

            //    chartData1 = recpResult.Where(r => r.RouteUnion == 1).Select(r => new AxisLabelData { x = r.Date.Date.ToString("D"), y = r.TotalQuantity }),
            //    chartData2 = recpResult.Where(r => r.RouteUnion == 0).Select(r => new AxisLabelData { x = r.Date.Date.ToString("D"), y = r.TotalQuantity }),
            //    chartData3 = recpResult.GroupBy(r => r.Date.Date).Select(g => new AxisLabelData { x = g.Key.Date.Date.ToString("D"), y = g.Sum(r => r.TotalQuantity) })

            //};
            var chartData = new
            {
                label1 = "Dataset_1",
                label2 = "Dataset_2",
                label3 = "Dataset_3",
                chartData1 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 860 },
        new AxisLabelData { x = "India", y = 1140 },
        new AxisLabelData { x = "Pakistan", y = 1060 },
        new AxisLabelData { x = "Germany", y = 1070 },
        new AxisLabelData { x = "Australia", y = 1110 },
        new AxisLabelData { x = "Italy", y = 1330 },
        new AxisLabelData { x = "United Kingdom", y = 2210 },
        new AxisLabelData { x = "Saudi Arabia", y = 2210 },
        new AxisLabelData { x = "Russia", y = 2210 }
    },
                chartData2 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 1600 },
        new AxisLabelData { x = "India", y = 1700 },
        new AxisLabelData { x = "Pakistan", y = 1700 },
        new AxisLabelData { x = "Germany", y = 1900 },
        new AxisLabelData { x = "Australia", y = 2000 },
        new AxisLabelData { x = "Italy", y = 2700 },
        new AxisLabelData { x = "United Kingdom", y = 4000 },
        new AxisLabelData { x = "Saudi Arabia", y = 5000 },
        new AxisLabelData { x = "Russia", y = 6000 }
    },
                chartData3 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 300 },
        new AxisLabelData { x = "India", y = 700 },
        new AxisLabelData { x = "Pakistan", y = 2000 },
        new AxisLabelData { x = "Germany", y = 5000 },
        new AxisLabelData { x = "Australia", y = 10000 },
        new AxisLabelData { x = "Italy", y = 4000 },
        new AxisLabelData { x = "United Kingdom", y = 2000 },
        new AxisLabelData { x = "Saudi Arabia", y = 1000 },
        new AxisLabelData { x = "Russia", y = 200 }
    }
            };

            ViewBag.chartData = chartData;

            var chartData2 = new
            {
                label4 = "Dataset_4",
                label5 = "Dataset_5",
                label6 = "Dataset_6",
                chartData4 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 860 },
        new AxisLabelData { x = "India", y = 140 },
        new AxisLabelData { x = "Pakistan", y = 160 },
        new AxisLabelData { x = "Germany", y = 170 },
        new AxisLabelData { x = "Australia", y = 110 },
        new AxisLabelData { x = "Italy", y = 133 },
        new AxisLabelData { x = "United Kingdom", y = 210 },
        new AxisLabelData { x = "Saudi Arabia", y = 21 },
        new AxisLabelData { x = "Russia", y = 220 }
    },
                chartData5 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 160 },
        new AxisLabelData { x = "India", y = 170 },
        new AxisLabelData { x = "Pakistan", y = 100 },
        new AxisLabelData { x = "Germany", y = 190 },
        new AxisLabelData { x = "Australia", y = 200 },
        new AxisLabelData { x = "Italy", y = 270 },
        new AxisLabelData { x = "United Kingdom", y = 400 },
        new AxisLabelData { x = "Saudi Arabia", y = 500 },
        new AxisLabelData { x = "Russia", y = 600 }
    },
                chartData6 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 300 },
        new AxisLabelData { x = "India", y = 700 },
        new AxisLabelData { x = "Pakistan", y = 200 },
        new AxisLabelData { x = "Germany", y = 500 },
        new AxisLabelData { x = "Australia", y = 100 },
        new AxisLabelData { x = "Italy", y = 400 },
        new AxisLabelData { x = "United Kingdom", y = 200 },
        new AxisLabelData { x = "Saudi Arabia", y = 100 },
        new AxisLabelData { x = "Russia", y = 20 }
    }
            };

            ViewBag.chartData2 = chartData2;
            var chartData3 = new
            {
                label7 = "Dataset_7",
                label8 = "Dataset_8",
                label9 = "Dataset_9",
                chartData7 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 80 },
        new AxisLabelData { x = "India", y = 114 },
        new AxisLabelData { x = "Pakistan", y = 106 },
        new AxisLabelData { x = "Germany", y = 107 },
        new AxisLabelData { x = "Australia", y = 110 },
        new AxisLabelData { x = "Italy", y = 133 },
        new AxisLabelData { x = "United Kingdom", y = 20 },
        new AxisLabelData { x = "Saudi Arabia", y = 210 },
        new AxisLabelData { x = "Russia", y = 220 }
    },
                chartData8 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 16 },
        new AxisLabelData { x = "India", y = 17 },
        new AxisLabelData { x = "Pakistan", y = 10 },
        new AxisLabelData { x = "Germany", y = 19 },
        new AxisLabelData { x = "Australia", y = 20 },
        new AxisLabelData { x = "Italy", y = 27 },
        new AxisLabelData { x = "United Kingdom", y = 40 },
        new AxisLabelData { x = "Saudi Arabia", y = 50 },
        new AxisLabelData { x = "Russia", y = 60 }
    },
                chartData9 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 30},
        new AxisLabelData { x = "India", y = 70 },
        new AxisLabelData { x = "Pakistan", y = 20 },
        new AxisLabelData { x = "Germany", y = 50 },
        new AxisLabelData { x = "Australia", y = 10 },
        new AxisLabelData { x = "Italy", y = 40 },
        new AxisLabelData { x = "United Kingdom", y = 20 },
        new AxisLabelData { x = "Saudi Arabia", y = 10 },
        new AxisLabelData { x = "Russia", y = 20 }
    }
            };

            ViewBag.chartData3 = chartData3;

            var chartData4 = new
            {
                label10 = "Dataset_10",
                label11 = "Dataset_11",
                label12 = "Dataset_12",
                chartData10 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 8 },
        new AxisLabelData { x = "India", y = 14 },
        new AxisLabelData { x = "Pakistan", y = 16 },
        new AxisLabelData { x = "Germany", y = 12 },
        new AxisLabelData { x = "Australia", y = 19 },
        new AxisLabelData { x = "Italy", y = 14},
        new AxisLabelData { x = "United Kingdom", y = 26 },
        new AxisLabelData { x = "Saudi Arabia", y = 27 },
        new AxisLabelData { x = "Russia", y = 23 }
    },
                chartData11 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 16 },
        new AxisLabelData { x = "India", y = 14 },
        new AxisLabelData { x = "Pakistan", y = 65 },
        new AxisLabelData { x = "Germany", y = 45 },
        new AxisLabelData { x = "Australia", y = 72 },
        new AxisLabelData { x = "Italy", y = 21 },
        new AxisLabelData { x = "United Kingdom", y = 72 },
        new AxisLabelData { x = "Saudi Arabia", y = 99 },
        new AxisLabelData { x = "Russia", y = 57 }
    },
                chartData12 = new List<AxisLabelData>
    {
        new AxisLabelData { x = "South Korea", y = 35 },
        new AxisLabelData { x = "India", y = 29 },
        new AxisLabelData { x = "Pakistan", y = 77 },
        new AxisLabelData { x = "Germany", y = 86 },
        new AxisLabelData { x = "Australia", y = 13 },
        new AxisLabelData { x = "Italy", y = 24 },
        new AxisLabelData { x = "United Kingdom", y = 11 },
        new AxisLabelData { x = "Saudi Arabia", y = 36 },
        new AxisLabelData { x = "Russia", y = 84 }
    }
            };

            ViewBag.chartData4 = chartData4;

            ViewBag.chartData = chartData;
            if (ModelState.IsValid)
            {
                DateTime inputDate = model.Date;
                Console.WriteLine(inputDate.ToString("yyyy-MM-dd"));

                return View(model);
            }
            return View(model);
        }

        public IActionResult RecDisp()
        {
            return View();
        }

        public IActionResult Utilities()
        {
            return View();
        }

        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult Security()
        {
            return View();
        }
    }
}
