using System;
using System.Data.Entity;
using System.Linq;

namespace QLMB.Models.Process
{
    public class Edit
    {
        private static string DefaultPassword = "123456";
        //Nhân sự
        public static (bool,string) EmployeeInfo(database db,ThongTinND info,NhanVien employee, ListChucVu roll, string currentCMND)
        {
            try
            {
                string newCMND = info.CMND.Trim();

                if (currentCMND != newCMND)
                    UpdateCMND(db, currentCMND, newCMND);


                string currentRole = employee.MaChucVu.Trim();
                string newRole = roll.MaChucVu.Trim();

                if (currentRole != newRole)
                {
                    string currentMaNV = employee.MaNV.Trim();
                    string newMaNV = newRole + Shared.CreateID(db, newRole).ToString();
                    UpdateMaNV(db, currentMaNV, newMaNV, newRole);
                }

                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                return (true, "Đổi thông tin thành công");
            }
            catch {return (false, "Đổi thông tin thất bại"); }
        }

        private static void UpdateCMND(database db, string currentCMND, string newCMND)
        {
            currentCMND = currentCMND.Trim();
            newCMND = newCMND.Trim();

            string query = $"UPDATE ThongTinND SET CMND = '{newCMND}' WHERE CMND = '{currentCMND}'";
            db.Database.ExecuteSqlCommand(query);
        }

        private static void UpdateMaNV(database db, string currentMANV, string newMANV, string newRole)
        {
            currentMANV = currentMANV.Trim();
            newMANV = newMANV.Trim();

            string query = $"UPDATE NhanVien SET MaNV = '{newMANV}', MaChucVu = '{newRole}' WHERE MaNV = '{currentMANV}'";
            db.Database.ExecuteSqlCommand(query);
        }

        public static bool EmployeeStatus(database db,NhanVien employee, string type)
        {
            employee = db.NhanViens.Where(s => s.MaNV == employee.MaNV.Trim()).FirstOrDefault();
            try
            {
                switch (type)
                {
                    case "Fired":
                        employee.MATT = 5;
                        break;

                    case "Hired":
                        employee.MATT = 6;
                        employee.MatKhau = SHA256.ToSHA256(DefaultPassword);
                        break;

                    case "ResetPassword":
                        employee.MatKhau = SHA256.ToSHA256(DefaultPassword);
                        break;
                }
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch { return false; }
        }

        public static bool EployeeFirstLogin(database db, NhanVien employee)
        {
            try
            {
                NhanVien update = db.NhanViens.Where(s => employee.MaNV.Trim() == s.MaNV.Trim()).FirstOrDefault();
                
                update.MATT = 4;
                update.MatKhau = SHA256.ToSHA256(employee.MatKhau);

                //Lưu vào database
                db.Entry(update).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch { return false; }
        }

        public static (bool,string) EmployeeProfile(database db, ThongTinND info)
        {
            try
            {
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();

                return (true, "Đổi thông tin thành công");
            }
            catch { return (false, "* Đổi thông tin thất bại, vui lòng thử lại sau"); }
        }

        public static (bool, string) EmployeePassword(database db, NhanVien employee)
        {
            try
            {
                employee.MatKhau = SHA256.ToSHA256(employee.MatKhau);
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();

                return (true, null);
            }
            catch { return (false, "* Đổi mật khẩu thất bại, vui lòng thử lại sau"); }
        }

        //Sự kiện - Ưu đãi
        public static (bool, string, SuKienUuDai) EventVerified(database db, string MaDon, string NguoiDuyet, string type)
        {
            SuKienUuDai info = db.SuKienUuDais.Where(s => s.MaDon.Trim() == MaDon.Trim()).FirstOrDefault();
            
            try
            {
                switch (type)
                {
                    case "Reset": info.MATT = 1;
                        break;
                    case "Accept": info.MATT = 2;
                        break;
                    case "Denied": info.MATT = 3;
                        break;
                }

                info.MaNV = NguoiDuyet.Trim();
                info.NgayDuyet = DateTime.Now;

                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                if(info.MATT == 1)
                    return (true, "Cài lại thành công", info);

                return (true, "Duyệt bài thành công", info);
            }
            catch { return (false, "* Duyệt bài thất bại ! Vui lòng thử lại sau", info); }
          
        }

        //Khách hàng
        public static (bool, string) CustomerPassword(database db, NguoiThue info)
        {
            try
            {
                string authTmp = SHA256.ToSHA256(info.MatKhau);
                info.MatKhau = authTmp;

                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();

                return (true, "Đổi mật khẩu thành công");
            }
            catch { return (false, "* Lỗi hệ thống - Xin vui lòng thử lại !"); }

        }
    }
}