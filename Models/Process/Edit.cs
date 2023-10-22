using System.Data.Entity;
using System.Linq;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace QLMB.Models.Process
{
    public class Edit
    {
        private static string DefaultPassword = "123456";
        public static bool EmployeeInfo(database db,ThongTinND info,NhanVien employee, ListChucVu roll, string currentCMND)
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
                return true;
            }
            catch {return false;}
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
    }
}