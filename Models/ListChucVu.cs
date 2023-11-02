namespace QLMB.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public partial class ListChucVu
    {
        public ListChucVu() { }
        public string MaChucVu { get; set; }
        public string TenCV { get; set; }

        //Mã chức vụ nhân viên hiện tại
        public string CurrentEmployee { get; set; }

        public List<ListChucVu> List{ get; internal set; }

        //Lưu danh sách chức vụ
        public void SettingList (List<ChucVu> chucVu) 
        {
            ListChucVu item = new ListChucVu();
            List<ListChucVu> list = new List<ListChucVu>();

            item.MaChucVu = "Default";
            item.TenCV = "--Chọn chức vụ--";
            list.Add(item);

            for(int i = 0; i < chucVu.Count; i++)
            {
                ListChucVu temp = new ListChucVu();
                temp.MaChucVu = chucVu.ElementAt(i).MaChucVu;
                temp.TenCV = chucVu.ElementAt(i).TenCV;
                list.Add(temp);
            }

            List = list;
        }
    }
}
