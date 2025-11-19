using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SchoolManagement
{
    public partial class FormBaoCao : Form
    {
        public FormBaoCao()
        {
            InitializeComponent();
        }

        private void FormBaoCao_Load(object sender, EventArgs e)
        {
            LoadDanhSachBaoCao();
            LoadDanhSachLoc();
            HienThiThamSo(0); 
        }

       

        private void LoadDanhSachBaoCao()
        {
            cboLoaiBaoCao.Items.Clear();
            cboLoaiBaoCao.Items.Add("1. Danh sách Học sinh theo Lớp học");
            cboLoaiBaoCao.Items.Add("2. Danh sách Giáo viên phụ trách Môn học"); 
            cboLoaiBaoCao.Items.Add("3. Danh sách Học sinh đạt điểm cao (Top N) theo Môn học");
            cboLoaiBaoCao.Items.Add("4. Thống kê số HS, GVCN trong từng Lớp học");
            cboLoaiBaoCao.Items.Add("5. Thống kê số lượng Học sinh theo từng Độ tuổi");
            cboLoaiBaoCao.Items.Add("6. Tìm kiếm Giáo viên có thâm niên công tác từ 5 năm trở lên");
            cboLoaiBaoCao.Items.Add("7. Top 5 Học sinh có Điểm TB Chung cao nhất"); 
            cboLoaiBaoCao.Items.Add("8. Quản lý điểm và Xếp loại Học sinh theo Môn");
            cboLoaiBaoCao.SelectedIndex = 0;
        }

        private void LoadDanhSachLoc()
        {
            
            LoadComboBox(cboLopLoc, "SELECT MaLop, TenLop FROM LopHoc", "TenLop", "MaLop");
            
            LoadComboBox(cboMonLoc, "SELECT MaMon, TenMon FROM MonHoc", "TenMon", "MaMon");
        }

        private void LoadComboBox(ComboBox cbo, string query, string display, string value)
        {
            DataTable dt = DBHelper.ExecuteQuery(query);
            cbo.DataSource = dt;
            cbo.DisplayMember = display;
            cbo.ValueMember = value;
        }

        private void HienThiThamSo(int index)
        {
            cboLopLoc.Visible = cboMonLoc.Visible = txtThamSo.Visible = false;
            

            switch (index)
            {
                case 0: 
                    cboLopLoc.Visible = true;
                    break;
                case 1: 
                    cboMonLoc.Visible = true;
                    break;
                case 2: 
                    cboMonLoc.Visible = txtThamSo.Visible = true; 
                    break;
                case 4: 
                    txtThamSo.Visible = true; 
                    break;
                case 7: 
                    cboLopLoc.Visible = true;
                    cboMonLoc.Visible = true;
                    break;
                   
            }
        }

      

        #region Implement Reports

       
        private void BaoCao_DanhSachHocSinhTheoLop(string maLop)
        {
            string query =
                "SELECT MaHS, HoTen, NgaySinh, GioiTinh, DiaChi, MaLop " +
                $"FROM HocSinh WHERE MaLop = '{maLop}' ORDER BY HoTen";

            HienThiKetQua(query, "Mã HS", "Họ Tên", "Ngày Sinh", "Giới Tính", "Địa Chỉ", "Mã Lớp");
        }

      
        private void BaoCao_GiaoVienTheoMonHoc(string maMon)
        {
            string query =
                "SELECT gv.MaGV, gv.HoTen, mh.TenMon, gv.NgayBatDauCongTac " +
                "FROM GiaoVien gv JOIN MonHoc mh ON gv.MaGV = mh.MaGVPhuTrach " +
                $"WHERE mh.MaMon = '{maMon}' " +
                "ORDER BY mh.TenMon, gv.HoTen";

            HienThiKetQua(query, "Mã GV", "Họ Tên", "Môn Phụ Trách", "Ngày Công Tác");
        }

        
        private void BaoCao_HocSinhDatDiemCaoTheoMon(string maMon, int topN)
        {
            
            string query =
                "SELECT MaHS, HoTen, DiemSo, TenMon " +
                "FROM ( " +
                "    SELECT hs.MaHS, hs.HoTen, ds.DiemSo, mh.TenMon, " +
                "           RANK() OVER (ORDER BY ds.DiemSo DESC) as RankDiem " +
                "    FROM HocSinh hs " +
                "    JOIN DiemSo ds ON hs.MaHS = ds.MaHS " +
                "    JOIN MonHoc mh ON ds.MaMon = mh.MaMon " +
                $"    WHERE ds.MaMon = '{maMon}' " +
                ") AS RankedData " +
                $"WHERE RankDiem <= {topN} " +
                "ORDER BY DiemSo DESC";

            HienThiKetQua(query, "Mã HS", "Họ Tên", "Điểm Số", "Tên Môn", "Rank");
        }

        
        private void ThongKe_HocSinhVaGVCNTheoLop()
        {
            string query =
                "SELECT lh.TenLop, COUNT(hs.MaHS) AS SoLuongHS, gv.HoTen AS TenGVCN " +
                "FROM LopHoc lh " +
                "LEFT JOIN HocSinh hs ON lh.MaLop = hs.MaLop " +
                "LEFT JOIN GiaoVien gv ON lh.GVCN = gv.MaGV " +
                "GROUP BY lh.MaLop, lh.TenLop, gv.HoTen " +
                "ORDER BY lh.TenLop";

            HienThiKetQua(query, "Tên Lớp", "Số Lượng HS", "GV Chủ nhiệm");
        }

        
        private void ThongKe_HocSinhTheoTuoi()
        {
            string query =
                "SELECT (YEAR(CURDATE()) - YEAR(NgaySinh)) AS Tuoi, " +
                "COUNT(MaHS) AS SoLuongHS " +
                "FROM HocSinh " +
                "GROUP BY Tuoi " +
                "ORDER BY Tuoi";

            HienThiKetQua(query, "Độ Tuổi", "Số Lượng HS");
        }

        
        private void BaoCao_GiaoVienThamNienCao()
        {
            string query =
                "SELECT HoTen, NgayBatDauCongTac, " +
                "TRUNCATE(DATEDIFF(CURDATE(), NgayBatDauCongTac) / 365.25, 1) AS SoNamCongTac " +
                "FROM GiaoVien " +
                "HAVING SoNamCongTac >= 5 " +
                "ORDER BY NgayBatDauCongTac ASC";

            HienThiKetQua(query, "Họ Tên", "Ngày Bắt Đầu", "Số Năm CT");
        }

        
        private void BaoCao_TopHocSinhDiemTBChung()
        {
            string query =
                "SELECT hs.MaHS, hs.HoTen, lh.TenLop, AVG(ds.DiemSo) AS DiemTBChung " +
                "FROM HocSinh hs " +
                "JOIN LopHoc lh ON hs.MaLop = lh.MaLop " +
                "JOIN DiemSo ds ON hs.MaHS = ds.MaHS " +
                "GROUP BY hs.MaHS, hs.HoTen, lh.TenLop " +
                "ORDER BY DiemTBChung DESC " +
                "LIMIT 5"; 

            HienThiKetQua(query, "Mã HS", "Họ Tên", "Lớp", "Điểm TB Chung");
        }

        
        private void BaoCao_DiemVaXepLoaiTheoMon(string maLop, string maMon)
        {
            string query =
                "SELECT hs.MaHS, hs.HoTen, AVG(ds.DiemSo) AS DiemTB, " +
                "CASE " +
                "    WHEN AVG(ds.DiemSo) >= 9.0 THEN N'Giỏi' " +
                "    WHEN AVG(ds.DiemSo) >= 7.0 THEN N'Khá' " +
                "    WHEN AVG(ds.DiemSo) >= 5.0 THEN N'Trung Bình' " +
                "    ELSE N'Yếu' " +
                "END AS XepLoai " +
                "FROM HocSinh hs " +
                $"LEFT JOIN DiemSo ds ON hs.MaHS = ds.MaHS AND ds.MaMon = '{maMon}' " +
                $"WHERE hs.MaLop = '{maLop}' " +
                "GROUP BY hs.MaHS, hs.HoTen " +
                "ORDER BY DiemTB DESC";

            HienThiKetQua(query, "Mã HS", "Họ Tên", "Điểm TB Môn", "Xếp Loại");
        }

        #endregion



        private void HienThiKetQua(string query, params string[] headers)
        {
            try
            {
                DataTable dtBaoCao = DBHelper.ExecuteQuery(query);
                dgvKetQuaBaoCao.DataSource = dtBaoCao;

                
                for (int i = 0; i < headers.Length; i++)
                {
                    if (i < dgvKetQuaBaoCao.Columns.Count)
                    {
                        dgvKetQuaBaoCao.Columns[i].HeaderText = headers[i];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thực thi báo cáo. Vui lòng kiểm tra kết nối CSDL và cú pháp SQL: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboLoaiBaoCao_SelectedIndexChanged(object sender, EventArgs e)
        {
            HienThiThamSo(cboLoaiBaoCao.SelectedIndex);
            dgvKetQuaBaoCao.DataSource = null; 
        }

        private void btnXemBaoCao_Click(object sender, EventArgs e)
        {
            int index = cboLoaiBaoCao.SelectedIndex;

            switch (index)
            {
                case 0: 
                    if (cboLopLoc.SelectedValue == null) { MessageBox.Show("Chọn Lớp."); return; }
                    BaoCao_DanhSachHocSinhTheoLop(cboLopLoc.SelectedValue.ToString());
                    break;
                case 1: 
                    if (cboMonLoc.SelectedValue == null) { MessageBox.Show("Chọn Môn học."); return; }
                    BaoCao_GiaoVienTheoMonHoc(cboMonLoc.SelectedValue.ToString());
                    break;
                case 2: 
                    if (cboMonLoc.SelectedValue == null || !int.TryParse(txtThamSo.Text, out int topN) || topN <= 0)
                    { MessageBox.Show("Chọn Môn và nhập Top N hợp lệ."); return; }
                    BaoCao_HocSinhDatDiemCaoTheoMon(cboMonLoc.SelectedValue.ToString(), topN);
                    break;
                case 3: 
                    ThongKe_HocSinhVaGVCNTheoLop();
                    break;
                case 4: 
                    ThongKe_HocSinhTheoTuoi();
                    break;
                case 5: 
                    BaoCao_GiaoVienThamNienCao();
                    break;
                case 6: 
                    BaoCao_TopHocSinhDiemTBChung();
                    break;
                case 7: 
                    if (cboLopLoc.SelectedValue == null || cboMonLoc.SelectedValue == null)
                    { MessageBox.Show("Chọn Lớp và Môn."); return; }
                    BaoCao_DiemVaXepLoaiTheoMon(cboLopLoc.SelectedValue.ToString(), cboMonLoc.SelectedValue.ToString());
                    break;
                default:
                    MessageBox.Show("Vui lòng chọn loại báo cáo.");
                    break;
            }
        }
    }
}
