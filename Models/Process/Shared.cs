using System.Collections.Generic;
using System.Linq;

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

        public static List<SuKienUuDai> listSKUD(database db, string NameSearch, string type)
        {

            List<SuKienUuDai> data = db.SuKienUuDais.Where(s => s.MaDM.Trim() == type.ToUpper()).ToList();

            if(NameSearch == null || NameSearch == "")
                return data;

            NameSearch = NameSearch.ToUpper();

            data = data.Where(s => s.MaDon.Contains(NameSearch) ||
                                   s.NgayLamDon.ToString().Contains(NameSearch) ||

                                   s.NguoiThue.ThongTinND.HoTen.ToUpper().Contains(NameSearch) ||
                                   s.TenDangNhap.ToUpper().Contains(NameSearch) ||

                                   s.TieuDe.ToUpper().Contains(NameSearch) ||

                                   s.NgayBatDau.ToString().Contains(NameSearch) ||
                                   s.NgayKetThuc.ToString().Contains(NameSearch) ||


                                   
                                   s.TieuDe.ToUpper().Contains(NameSearch.ToUpper()) ||


                                   (s.MaNV != null && s.MaNV.ToUpper().Contains(NameSearch)) ||
                                   (s.MaNV != null && s.NhanVien.ThongTinND.HoTen.ToUpper().Contains(NameSearch)) ||
                                   (s.NgayDuyet != null && s.NgayDuyet.ToString().Contains(NameSearch)) ||

                                   s.TinhTrang.TenTT.ToUpper().Contains(NameSearch)).ToList();
            return data;
        } 
    }
}