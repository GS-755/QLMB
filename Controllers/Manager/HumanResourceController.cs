using QLMB.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace QLMB.Controllers.Manager
{
    public class HumanResourceController : Controller
    {
        private database db = new database();

        public ActionResult Main(string NameSearch)
        {
            try
            {
                //Kiểm tra hợp lệ
                if (checkRole())
                {
                    var data = db.NhanViens.ToList();

                    //Xử lý tìm kiếm
                    if (NameSearch != null)
                    {
                        data = data.Where(s => s.MaNV.ToString().Contains(NameSearch) ||
                                               s.ChucVu.TenCV.ToUpper().Contains(NameSearch) ||
                                               s.CMND.Trim().Contains(NameSearch) ||
                                               s.ThongTinND.HoTen.ToUpper().Contains(NameSearch.ToUpper()) ||
                                               s.TinhTrang.TenTT.ToUpper().Contains(NameSearch.ToUpper())).ToList();
                    }

                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EmployeeMain";
                    return View(data);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        public ActionResult Detail(string CMND)
        {
            try
            {
                //Kiểm tra hợp lệ
                if (checkRole())
                {
                    if (Session["Page"] == null)
                        return RedirectToAction("Main");

                    ThongTinND info;

                    if (CMND == null && Session["HumanResourceTemp"] != null)
                    {
                        info = (ThongTinND)Session["HumanResourceTemp"];
                    }
                    else
                    {
                        info = db.ThongTinNDs.Where(s => s.CMND == CMND).FirstOrDefault();
                        Session["HumanResourceTemp"] = info;
                    }

                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EmployeeDetail";
                    return View(info);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(NhanVien info, string btn)
        {
            NhanVien update = db.NhanViens.Where(S => S.CMND == info.CMND).First();
            switch (btn)
            {
                case "Fired":
                    update.MATT = 5;
                    break;

                case "Hired":
                    update.MATT = 6;
                    update.MatKhau = SHA256.ToSHA256("123456");
                    break;

                case "ResetPassword":
                    update.MatKhau = SHA256.ToSHA256("123456");
                    break;
            }
            

            db.Entry(update).State = EntityState.Modified;

            db.SaveChanges();

            return View(update);
        }



        //Đăng ký nhân viên

        //Trang đăng ký
        public ActionResult Register()
        {
            try
            {
                //Kiểm tra hợp lệ
                if (checkRole())
                {
                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EmployeeRegister";
                    return View();
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        //Xử lý thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ThongTinND thongTin, ChucVu chucVu)
        {
            try
            {
                if (checkInfoEmployee(thongTin, chucVu))
                {
                    bool exist = db.ThongTinNDs.Any(s => s.CMND == thongTin.CMND || s.HoTen == thongTin.HoTen.ToUpper());

                    if (!exist)
                    {
                        AddDatabaseEmployee(thongTin, chucVu);
                        TempData["msg"] = "<script>alert('Đăng ký thành công');</script>";
                        return RedirectToAction("Main", "HumanResource");
                    }
                    ModelState.AddModelError("TrungCMND", "* Người này đã có trên hệ thống !");
                }
                return View();
            }
            catch
            {
                ViewBag.CitizenStatus = "Xác minh thất bại! - Xin hãy thử lại !";
                return View();
            }
        }


        //Chọn vị trí
        public ActionResult SelectRole()
        {
            ChucVu selected = new ChucVu();
            selected.ListChucVu = db.ChucVus.ToList<ChucVu>();
            return PartialView(selected);
        }


        //Kiểm tra dữ liệu
        private bool checkInfoEmployee(ThongTinND thongTin, ChucVu chucVu)
        {
            (bool, string) CMND = Validation.ExistCMND(thongTin.CMND);
            (bool, string) NgayCap = Validation.NgayCap(thongTin.NgayCap);
            (bool, string) HoTen = Validation.HoTen(thongTin.HoTen);
            (bool, string) GioiTinh = Validation.Gender(thongTin.GioiTinh);
            (bool, string) NgaySinh = Validation.Birthday_25(thongTin.NgaySinh);
            (bool, string) DiaChi = Validation.Address(thongTin.DiaChi);
            (bool, string) ChucVu = Validation.Role(chucVu.MaChucVu);

            bool check = CMND.Item1 && NgayCap.Item1 && HoTen.Item1 && GioiTinh.Item1 && NgaySinh.Item1 && DiaChi.Item1 && ChucVu.Item1;

            if (check)
                return true;

            //CMND
            if (!CMND.Item1)
                ModelState.AddModelError("CMND", CMND.Item2);

            //Ngày cấp
            if (!NgayCap.Item1)
                ModelState.AddModelError("NgayCapCMND", NgayCap.Item2);

            //Họ tên
            if (!HoTen.Item1)
                ModelState.AddModelError("HoTen", HoTen.Item2);


            //Giới tính
            if (!GioiTinh.Item1)
                ModelState.AddModelError("GioiTinh", GioiTinh.Item2);


            //Ngày sinh
            if (!NgaySinh.Item1)
                ModelState.AddModelError("NgaySinhND", NgaySinh.Item2);


            //Địa chỉ
            if (!DiaChi.Item1)
                ModelState.AddModelError("DiaChi", DiaChi.Item2);

            //Chức vụ
            if (!ChucVu.Item1)
                ModelState.AddModelError("ChonChucVu", ChucVu.Item2);

            return false;
        }


        //Thêm dữ liệu
        private void AddDatabaseEmployee(ThongTinND thongTin, ChucVu chucVu)
        {
            string authTmp = SHA256.ToSHA256("123456");
            int soNV = db.NhanViens.Where(s => s.MaChucVu == chucVu.MaChucVu.Trim()).Count();

            ThongTinND info = new ThongTinND();
            info.CMND = thongTin.CMND.Trim();
            info.NgayCap = thongTin.NgayCap;

            info.HoTen = thongTin.HoTen.ToString();
            info.GioiTinh = thongTin.GioiTinh.ToString();
            info.NgaySinh = thongTin.NgaySinh;
            info.DiaChi = thongTin.DiaChi.ToString();

            NhanVien account = new NhanVien();
            account.CMND = thongTin.CMND.Trim();
            account.MaNV = chucVu.MaChucVu.Trim() + soNV.ToString();
            account.MatKhau = authTmp.Trim();
            account.MaChucVu = chucVu.MaChucVu.Trim();
            account.MATT = 6;

            db.ThongTinNDs.Add(info);
            db.NhanViens.Add(account);
            db.SaveChanges();
        }


        //Kiểm tra hợp lệ
        private bool checkRole()
        {
            //Nếu EmployeeInfo == null --> Chưa đăng nhập
            if (Session["EmployeeInfo"] == null)
                return false;

            //Đúng Role --> Vào
            if (((NhanVien)Session["EmployeeInfo"]).MaChucVu.Trim() == "NS")
                return true;

            return false;
        }
    }
}

/*
Mã tình trạng (* = Được sử dụng)
    1: Đang chờ duyệt
    2: Được duyệt
    3: Bị từ chối
    4: Đang làm   *
    5: Nghỉ việc  *
    6: Được tuyển *
 */