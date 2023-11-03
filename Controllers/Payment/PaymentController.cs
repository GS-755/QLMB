using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using QLMB.Models;
using QLMB.Models.VNPay;

namespace QLMB.Controllers.Payment
{
    public class PaymentController : Controller
    {
        private database db = new database();

        // GET: Payment
        public ActionResult Index(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MatBang matBang = db.MatBangs.Find(id);
                if (matBang == null)
                {
                    return HttpNotFound();
                }

                return View(matBang);
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        public ActionResult PaymentSuccess() 
        {
            VNPayReturn vNPayReturn = new VNPayReturn();
            vNPayReturn.ProcessResponses();

            if(vNPayReturn != null)
            {
                return View(vNPayReturn);
            }
            return RedirectToAction("Index", "SkillIssue");
        }
        public ActionResult PaymentFailure() 
        {
            return View();  
        }
        [HttpPost]
        public ActionResult Payment(MatBang matBang)
        {
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Get the input object
            matBang = db.MatBangs.FirstOrDefault(k => k.MaMB == matBang.MaMB);

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (matBang.TienThue * 100).ToString()); // Nhân cho 100 để thêm 2 số 0 :) 
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + matBang.MaMB);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã Website (Terminal ID)

            //Add Params of 2.1.0 Version
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            Response.Redirect(paymentUrl);

            return View();
        }
    }
}