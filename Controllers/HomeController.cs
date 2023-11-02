using QLMB.Models;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace QLMB.Controllers
{
    public class ViewModel
    {
        public IEnumerable<MatBang> IE_MBList { get; set; }
        public IEnumerable<SuKienUuDai> IE_SKUDList { get; set; }
    }
    public class HomeController : Controller
    {
        private database db = new database();

        public ActionResult Index()
        {
            ViewModel viewModel = new ViewModel();  
            viewModel.IE_SKUDList = db.SuKienUuDais.OrderByDescending(s => s.NgayBatDau);
            viewModel.IE_MBList = db.MatBangs.Where(k => k.MATT == 7).ToList();

            return View(viewModel);
        }
    }
}