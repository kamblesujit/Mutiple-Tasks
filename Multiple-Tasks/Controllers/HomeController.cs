using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Multiple_Tasks.Models;
using QRCoder;
using System.Drawing;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;

namespace Multiple_Tasks.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public PayloadGenerator.Payload QRcodeText { get; private set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           
            return View();
        }
        public List<Product_Model> GetDetails()
        {
            List<Product_Model> productModels = new List<Product_Model>
            {
                new Product_Model { Name="Mouse",Description="Laptop Mouse",Price = 300},
                new Product_Model { Name="Key Board",Description="Laptop KeyBoard",Price = 250},
                new Product_Model { Name="HeadSets",Description="Laptop HeadSets",Price = 270},
                new Product_Model { Name="Speaker",Description="Laptop Speaker",Price = 500},
                new Product_Model { Name="Printer",Description="Laptop Printer",Price = 2200},

            };
            return productModels;
        }
        public async Task<IActionResult> GetData()
        {
            List<Product_Model> productModels = new List<Product_Model>();
            productModels = GetDetails();

            string emailtemplatepath = Path.Combine(Directory.GetCurrentDirectory(), "ExcelFiles//ProductReports.cshtml");
            string htmldata = System.IO.File.ReadAllText(emailtemplatepath);

            string excelstring = "";
            foreach (Product_Model prod in productModels)
            {
                excelstring += "<tr><td>" + prod.Name + "</td><td>" + prod.Description + "</td><td>" + prod.Price + "</td></tr>";
            }
            htmldata = htmldata.Replace("@@ActualData", excelstring);

            string StoredFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ExcelFileStored", DateTime.Now.Ticks.ToString() + ".xlsx");
            System.IO.File.AppendAllText(StoredFilePath, htmldata);


            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(StoredFilePath, out var contenttype))
            {
                contenttype = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(StoredFilePath);
            return File(bytes, contenttype, Path.Combine(StoredFilePath));
            
        }
        [HttpGet("GenerateQRcodeData")]
        public IActionResult GenerateQRcodeData(string inputText)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);


            System.Drawing.Image qrCodeImage = qrCode.GetGraphic(20);

            var bytes = ImageToByteArray(qrCodeImage);
            return File(bytes, "image/bmp");

            //using (MemoryStream ms = new MemoryStream())
            //{
            //    QRCodeGenerator qRodeGenerator = new QRCodeGenerator();
            //    QRCodeData qRCodeData = qRodeGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
            //    QRCode qRCode = new QRCode(qRCodeData);
            //    using (Bitmap oBitmap = qRCode.GetGraphic(20))
            //    {
            //        oBitmap.Save(ms, ImageFormat.Png);
            //        ViewBag.QrCode = "data:Image/png;base64," + Convert.ToBase64String(ms.ToArray());
            //    }
            //}

            //return View();


        }


        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
