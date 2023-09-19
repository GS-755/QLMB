using Microsoft.Ajax.Utilities;
using QLMB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace QLMB.Controllers.NewFolder1
{
    public class RegisterController : Controller
    {
        private database db = new database();
        private ThongTinND info = new ThongTinND();


        //Thông tin: Người thuê
        public ActionResult rentalInfo()
        {
            return View();
        }

        //Xử lý thông tin: Người thuê
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RentalInfo([Bind(Include = "CMND,NgayCap,HoTen,GioiTinh,NgaySinh,DiaChi,TenDangNhap,MatKhau,NhapLaiMatKhau")] ThongTinND thongTin)
        {
            try
            {    
                if (checkInfo(thongTin))
                {
                    var exist = db.ThongTinNDs.Any(s => s.CMND == thongTin.CMND || s.HoTen == thongTin.HoTen.ToUpper());
                    if (!exist)
                    {
                        exist = db.NguoiThues.Any(s => s.TenDangNhap == thongTin.TenDangNhap);
                        if (!exist)
                        {
                            AddDatabase(thongTin);
                            TempData["msg"] = "<script>alert('Đăng ký thành công');</script>";
                            return RedirectToAction("LoginPage", "Login");
                        }
                        else
                        {
                            ModelState.AddModelError("TrungTaiKhoan", "* Tài khoản đã tồn tại");
                            return View();
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError("TrungCMND", "* Bạn đã đăng ký tài khoản !");
                        return View();
                    }
                }
                else
                {
                    
                    return View();
                }

            }
            catch
            {
                ViewBag.CitizenStatus = "Xác minh thất bại! - Xin hãy thử lại !";
                return View();
            }
        }

        private bool checkInfo(ThongTinND thongTin)
        {
            int error = 0;
            
            //CMND chưa nhập
            if(thongTin.CMND == null)
            {
                ModelState.AddModelError("CMND", "* Xin hãy điền CMND/CCCD");
                error++;
            }

            //CMND Chưa đủ 12 số
            else if (thongTin.CMND.Trim().Length != 12)
            {
                ModelState.AddModelError("CMND", "* CMND/CCCD phải đủ 12 số");
                error++;
            }
            else
            {
                //CMND có chứa chữ
                char[] numberCheck = thongTin.CMND.Trim().ToCharArray();

                for (int i = 0; i < thongTin.CMND.Trim().Length; i++)
                {
                    if (!char.IsNumber(numberCheck[i]))
                    {
                        error++;
                        ModelState.AddModelError("CMND", "* CMND/CCCD không hợp lệ");
                        break;
                    }
                }
            }


            //Ngày cấp
            if (thongTin.NgayCap > DateTime.Now || thongTin.NgayCap.Year < 1900) 
            {
                ModelState.AddModelError("NgayCapCMND", "* Ngày cấp không hợp lệ");
                error++;
            }


            //Họ tên
            if (thongTin.HoTen == null)
            {
                ModelState.AddModelError("HoTen", "* Xin hãy điền họ tên");
                error++;
            }


            //Giới tính
            if (thongTin.GioiTinh == null)
            {
                ModelState.AddModelError("GioiTinh", "* Xin hãy chọn giới tính");
                error++;
            }


            //Ngày sinh
            if (thongTin.NgaySinh.Year >= DateTime.Now.Year - 25)
            {
                ModelState.AddModelError("NgaySinhND", "* Bạn chưa đủ tuổi để đăng ký");
                error++;
            }
            
            //Ngày sinh không hợp lệ
            else if (thongTin.NgaySinh.Year < 1800)
            {
                ModelState.AddModelError("NgaySinhND", "* Ngày sinh không hợp lệ");
                error++;
            }


            //Địa chỉ
            if(thongTin.DiaChi == null)
            {
                ModelState.AddModelError("DiaChi", "* Xin hãy nhập địa chỉ");
                error++;
            }


            //Tên đăng nhập
            if (thongTin.TenDangNhap == null)
            {
                ModelState.AddModelError("TenDangNhap", "* Xin hãy điền tên đăng nhập");
                error++;
            }
            else if (thongTin.TenDangNhap.Trim().Length < 8)
            {
                ModelState.AddModelError("TenDangNhap", "* Tên đăng nhập phải dài hơn 8 ký tự");
                error++;
            }



            //Mật khẩu
            if (thongTin.MatKhau == null)
            {
                ModelState.AddModelError("MatKhau", "* Xin hãy điền mật khẩu");
                error++;
            }


            //Nhập lại mật khẩu
            if (thongTin.NhapLaiMatKhau == null)
            {
                ModelState.AddModelError("MatKhauLai", "* Xin hãy điền lại mật khẩu");
                error++;
            }
            else if(thongTin.MatKhau != thongTin.NhapLaiMatKhau)
            {
                ModelState.AddModelError("MatKhauLai", "* Mật khẩu không khớp - Xin hãy điền lại");
                error++;
            }


            if (error == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddDatabase(ThongTinND thongTin)
        {
            string authTmp = SHA256.ToSHA256(thongTin.MatKhau);
           
            ThongTinND info = new ThongTinND();
            info.CMND = thongTin.CMND.Trim();
            info.NgayCap = thongTin.NgayCap;
            
            info.HoTen = thongTin.HoTen.ToString();
            info.GioiTinh = thongTin.GioiTinh.ToString();
            info.NgaySinh = thongTin.NgaySinh;
            info.DiaChi = thongTin.DiaChi.ToString();

            NguoiThue account = new NguoiThue();
            account.CMND = thongTin.CMND.Trim();
            account.TenDangNhap = thongTin.TenDangNhap.Trim();
            account.MatKhau = authTmp.Trim();

            db.ThongTinNDs.Add(info);
            db.NguoiThues.Add(account);
            db.SaveChanges();
        }

        //Nhân viên
        public ActionResult SelectRole()
        {
            ChucVu selected = new ChucVu();
            selected.ListChucVu = db.ChucVus.ToList<ChucVu>();
            return PartialView(selected);
        }

        public ActionResult employeeInfo()
        {
            return View();
        }

        //Xử lý thông tin: Người thuê
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult employeeInfo([Bind(Include = "CMND,NgayCap,HoTen,GioiTinh,NgaySinh,DiaChi,ChucVu")] ThongTinND thongTin,ChucVu chucVu)
        {
            Session["Testing"] = chucVu.MaChucVu;
            try
            {
                if (checkInfoEmployee(thongTin,chucVu))
                {
                    var exist = db.ThongTinNDs.Any(s => s.CMND == thongTin.CMND || s.HoTen == thongTin.HoTen.ToUpper());
                    if (!exist)
                    {
                            AddDatabaseEmployee(thongTin, chucVu);
                            TempData["msg"] = "<script>alert('Đăng ký thành công');</script>";
                            return RedirectToAction("HumanResourceMain", "HumanResource");
                    }
                    else
                    {
                        ModelState.AddModelError("TrungCMND", "* Người này đã có trên hệ thống !");
                        return View();
                    }
                }
                else
                {
                    return View();
                }

            }
            catch
            {
                ViewBag.CitizenStatus = "Xác minh thất bại! - Xin hãy thử lại !";
                return View();
            }

        }

        private bool checkInfoEmployee(ThongTinND thongTin, ChucVu chucVu)
        {
            int error = 0;

            //CMND chưa nhập
            if (thongTin.CMND == null)
            {
                ModelState.AddModelError("CMND", "* Xin hãy điền CMND/CCCD");
                error++;
            }

            //CMND Chưa đủ 12 số
            else if (thongTin.CMND.Trim().Length != 12)
            {
                ModelState.AddModelError("CMND", "* CMND/CCCD phải đủ 12 số");
                error++;
            }
            else
            {
                //CMND có chứa chữ
                char[] numberCheck = thongTin.CMND.Trim().ToCharArray();

                for (int i = 0; i < thongTin.CMND.Trim().Length; i++)
                {
                    if (!char.IsNumber(numberCheck[i]))
                    {
                        error++;
                        ModelState.AddModelError("CMND", "* CMND/CCCD không hợp lệ");
                        break;
                    }
                }
            }


            //Ngày cấp
            if (thongTin.NgayCap > DateTime.Now || thongTin.NgayCap.Year < 1900)
            {
                ModelState.AddModelError("NgayCapCMND", "* Ngày cấp không hợp lệ");
                error++;
            }


            //Họ tên
            if (thongTin.HoTen == null)
            {
                ModelState.AddModelError("HoTen", "* Xin hãy điền họ tên");
                error++;
            }


            //Giới tính
            if (thongTin.GioiTinh == null)
            {
                ModelState.AddModelError("GioiTinh", "* Xin hãy chọn giới tính");
                error++;
            }


            //Ngày sinh
            if (thongTin.NgaySinh.Year >= DateTime.Now.Year - 25)
            {
                ModelState.AddModelError("NgaySinhND", "* Bạn chưa đủ tuổi để đăng ký");
                error++;
            }

            //Ngày sinh không hợp lệ
            else if (thongTin.NgaySinh.Year < 1800)
            {
                ModelState.AddModelError("NgaySinhND", "* Ngày sinh không hợp lệ");
                error++;
            }


            //Địa chỉ
            if (thongTin.DiaChi == null)
            {
                ModelState.AddModelError("DiaChi", "* Xin hãy nhập địa chỉ");
                error++;
            }

            //Chức vụ
            if (chucVu.MaChucVu == null)
            {
                ModelState.AddModelError("ChonChucVu", "* Xin hãy chọn chức vụ");
                error++;
            }

            if (error == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


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

    }
}