using QLMB.Models;
using System.Linq;
using System.Web.Mvc;
using QLMB.Models.Process;

namespace QLMB.Controllers
{
    public class HumanResourceController : Controller
    {
        private database db = new database();

        //Trang chủ
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
            catch { return RedirectToAction("Index", "SkillIssue");}
        }



        //Trang chi tiết
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
                        Session["HumanResourceEmployeeTemp"] = db.NhanViens.Where(s => s.CMND == CMND).FirstOrDefault();
                        Session.Remove("TempRole");
                    }

                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EmployeeDetail";
                    return View(info);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch { return RedirectToAction("Index", "SkillIssue");}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(ThongTinND info, ListChucVu roll , string btn)
        {
            NhanVien employee = (NhanVien)Session["HumanResourceEmployeeTemp"];

            if (btn != null)
            {
                if (Edit.EmployeeStatus(db, employee, btn))
                    return RedirectToAction("Detail", "HumanResource", new { CMND = info.CMND.Trim() });

                ModelState.AddModelError("editStatus", "Đổi thông tin thất bại");
            }

            string currentCMND = ((NhanVien)Session["HumanResourceEmployeeTemp"]).CMND.Trim();

            if (checkEdit(info, roll, currentCMND))
            {
                NhanVien User = (NhanVien)Session["EmployeeInfo"];
                NhanVien Current = (NhanVien)Session["HumanResourceEmployeeTemp"];

                //Xử lý việc tự edit cho chính mình
                if (User.MaChucVu.Trim() == Current.MaChucVu.Trim() && Current.MaChucVu.Trim() == "NS")
                    roll.MaChucVu = "NS";

                (bool, string) saveDetail = Edit.EmployeeInfo(db, info, (NhanVien)Session["HumanResourceEmployeeTemp"], roll, currentCMND);
                if (saveDetail.Item1)
                {
                    TempData["msg"] = $"<script>alert('{saveDetail.Item2}');</script>";
                    Session.Remove("TempRole");
                    return RedirectToAction("Detail", "HumanResource", new { CMND = info.CMND.Trim() });
                }
                ModelState.AddModelError("editStatus", saveDetail.Item2);
            }
            return View(info);
        }




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
                    Session.Remove("TempRole");
                    return View();
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch { return RedirectToAction("Index", "SkillIssue");}
        }

        //Xử lý thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(ThongTinND info, ListChucVu role)
        {
            if (checkInfoEmployee(info, role))
            {
                (bool, string) checkAccount = Validation.ExistAccount(db, info.CMND, info.HoTen);
                if (checkAccount.Item1)
                {
                    (bool, string) checkRegister = Create.Employee(db, info, role);
                    if (checkRegister.Item1)
                    {
                        TempData["msg"] = $"<script>alert('{checkRegister.Item2}');</script>";
                        return RedirectToAction("Register", "HumanResource");
                    }
                    ModelState.AddModelError("TrungCMND", checkRegister.Item2);
                }
                ModelState.AddModelError("TrungCMND", checkAccount.Item2);
            }
            return View();
        }



        //Chọn Chức vụ
        public ActionResult SelectRole(string CMND)
        {
            if (checkRole() && Session["Page"] != null)
            {
                ListChucVu roll = new ListChucVu();
                roll.SettingList(db.ChucVus.ToList<ChucVu>());


                if (Session["TempRole"] != null)
                    roll.CurrentEmployee = Session["TempRole"].ToString();

                else if (!Validation.CMND(CMND).Item1 || Session["Page"].ToString() == "EmployeeRegister")
                    roll.CurrentEmployee = "Default";

                else
                {
                    NhanVien employee = db.NhanViens.Where(s => s.CMND.Trim() == CMND.Trim()).FirstOrDefault();
                    roll.CurrentEmployee = employee.MaChucVu;
                }


                return PartialView(roll);
            }
            //ViewBag.DepartmentID = new SelectList(chucVu.List, "MaChucVu", "TenCV", MaChucVu);

            //Không thoả --> Về trang xử lý chuyển trang
            return RedirectToAction("Manager", "Account");
        }


        //*-- Xử lý --*//

        //Kiểm tra thông tin đăng ký
        private bool checkInfoEmployee(ThongTinND thongTin, ListChucVu chucVu)
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

            Session["TempRole"] = chucVu.MaChucVu;

            return false;
        }



        //Kiểm tra thông tin edit
        private bool checkEdit(ThongTinND info, ListChucVu roll, string currentCMND)
        {
            NhanVien User = (NhanVien)Session["EmployeeInfo"];
            NhanVien Current = (NhanVien)Session["HumanResourceEmployeeTemp"];

            (bool, string) HoTen = Validation.HoTen(info.HoTen);
            (bool, string) NgaySinh = Validation.Birthday_25(info.NgaySinh);
            (bool, string) CMND = Validation.CMNDEdit(currentCMND, info.CMND);
            (bool, string) NgayCap = Validation.NgayCap(info.NgayCap);
            (bool, string) DiaChi = Validation.Address(info.DiaChi);

            (bool, string) ChucVu = Validation.RoleEdit(roll.MaChucVu, User.MaChucVu, Current.MaChucVu);

            if (HoTen.Item1 && NgaySinh.Item1 && CMND.Item1 && NgayCap.Item1 && DiaChi.Item1 && ChucVu.Item1)
                return true;

            if (!HoTen.Item1)
                ModelState.AddModelError("editName", HoTen.Item2);

            if (!NgaySinh.Item1)
                ModelState.AddModelError("editNgaySinh", NgaySinh.Item2);

            if (!CMND.Item1)
                ModelState.AddModelError("editCMND", CMND.Item2);


            if (!NgayCap.Item1)
                ModelState.AddModelError("editNgayCap", NgayCap.Item2);

            if (!DiaChi.Item1)
                ModelState.AddModelError("editAddress", DiaChi.Item2);

            if (!ChucVu.Item1)
                ModelState.AddModelError("editRole", ChucVu.Item2);

            Session["TempRole"] = roll.MaChucVu;

            return false;
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