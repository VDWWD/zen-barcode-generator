using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zen.Barcode;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //create the barcode
            var barcode = new ZenBarcode.BarcodeData()
            {
                type = BarcodeSymbology.CodeQr,
                height = 200,
                value = "van der Waal Webdesign - www.vanderwaal.eu"
            };

            //create the model
            var model = new Models.HomeViewModel()
            {
                barcode_base64 = "data:image/png;base64," + Convert.ToBase64String(barcode.Generate())
            };

            return View(model);
        }
    }
}