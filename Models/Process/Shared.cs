using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLMB.Models.Process
{
    public class Shared
    {
        public static int CreateID(database db, string roll)
        {
            List<NhanVien> nhanViens = db.NhanViens.Where(s => s.MaChucVu.Trim() == roll.Trim()).ToList();

            int STT = 0;
            int CharLength = roll.Trim().Length;

            foreach (NhanVien nhanVien in nhanViens)
            {
                string MaNV = nhanVien.MaNV.Trim().Substring(CharLength);
                if (int.Parse(MaNV) != STT)
                    break;

                STT++;
            }

            return STT;
        }
    }
}