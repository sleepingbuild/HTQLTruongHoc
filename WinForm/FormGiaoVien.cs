using System;
using System.Data;
using System.Windows.Forms;

namespace SchoolManagement
{
    public partial class FormGiaoVien : Form
    {
        private bool isAdding = false;
        public FormGiaoVien()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            string queryGV =
               "SELECT gv.MaGV, gv.HoTen, gv.NgaySinh, gv.GioiTinh, gv.DiaChi, bm.TenBoMon, gv.MaBM, gv.NgayBatDauCongTac " +
               "FROM GiaoVien gv LEFT JOIN BoMon bm ON gv.MaBM = bm.MaBM";
            dgvGiaoVien.DataSource = DBHelper.ExecuteQuery(queryGV);


            string queryBM = "SELECT MaBM, TenBoMon FROM BoMon";
            DataTable dtBM = DBHelper.ExecuteQuery(queryBM);

            cboBoMon.DataSource = dtBM;
            cboBoMon.DisplayMember = "TenBoMon";
            cboBoMon.ValueMember = "MaBM";

           
            cboGioiTinh.Items.Clear();
            cboGioiTinh.Items.Add("Nam");
            cboGioiTinh.Items.Add("Nữ");
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            isAdding = true; 
            txtMaGV.Clear();
            txtHoTen.Clear();
            txtDiaChi.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            dtpNgayBatDau.Value = DateTime.Now;
            cboGioiTinh.SelectedIndex = -1;
            cboBoMon.SelectedIndex = -1;

            txtMaGV.ReadOnly = false; 
            txtMaGV.Focus();
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaGV.Text))
            {
                MessageBox.Show("Vui lòng chọn một giáo viên để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isAdding = false; 
            txtMaGV.ReadOnly = true; 

            MessageBox.Show("Bạn đang ở chế độ Sửa. Hãy thay đổi thông tin và nhấn Lưu.", "Thông báo");
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaGV.Text))
            {
                MessageBox.Show("Vui lòng chọn một giáo viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maGV = txtMaGV.Text;

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa giáo viên có mã {maGV} không? Thao tác này không thể hoàn tác!",
                                                "Xác nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                
                string query = $"DELETE FROM GiaoVien WHERE MaGV='{maGV}'";

                if (DBHelper.ExecuteNonQuery(query) > 0)
                {
                    MessageBox.Show("Xóa giáo viên thành công!", "Thành công");
                    LoadData();
                    
                    txtMaGV.Clear(); txtHoTen.Clear(); 
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Có thể giáo viên này đang làm chủ nhiệm lớp hoặc phụ trách môn học (lỗi ràng buộc).",
                                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
          
            string maGV = txtMaGV.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string ngaySinh = dtpNgaySinh.Value.ToString("yyyy-MM-dd");
            string gioiTinh = cboGioiTinh.Text;
            string diaChi = txtDiaChi.Text;
            string maBM = cboBoMon.SelectedValue?.ToString() ?? "";
            
            string ngayBD = dtpNgayBatDau.Value.ToString("yyyy-MM-dd");
            string query = "";
            string action = "";

            if (isAdding)
            {
                action = "Thêm";
                query = $"INSERT INTO GiaoVien (MaGV, HoTen, NgaySinh, GioiTinh, DiaChi, MaBM, NgayBatDauCongTac) " +
                        $"VALUES ('{maGV}', N'{hoTen}', '{ngaySinh}', N'{gioiTinh}', N'{diaChi}', '{maBM}', '{ngayBD}')";
            }
            else
            {
                action = "Cập nhật";
                query = $"UPDATE GiaoVien SET HoTen=N'{hoTen}', NgaySinh='{ngaySinh}', GioiTinh=N'{gioiTinh}', " +
                        $"DiaChi=N'{diaChi}', MaBM='{maBM}', NgayBatDauCongTac='{ngayBD}' WHERE MaGV='{maGV}'";
            }

            if (DBHelper.ExecuteNonQuery(query) > 0)
            {
                MessageBox.Show(action + " giáo viên thành công!");
                LoadData();
            }
            else
            {
                MessageBox.Show(action + " giáo viên thất bại!");
            }
        }
        private void dgvGiaoVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvGiaoVien.Rows[e.RowIndex];

                txtMaGV.Text = row.Cells["MaGV"].Value.ToString();
                txtHoTen.Text = row.Cells["HoTen"].Value.ToString();

               
                if (DateTime.TryParse(row.Cells["NgaySinh"].Value.ToString(), out DateTime ngaySinh))
                {
                    dtpNgaySinh.Value = ngaySinh;
                }
                if (DateTime.TryParse(row.Cells["NgayBatDauCongTac"].Value.ToString(), out DateTime ngayBD))
                {
                    dtpNgayBatDau.Value = ngayBD;
                }

                cboGioiTinh.Text = row.Cells["GioiTinh"].Value.ToString();
                txtDiaChi.Text = row.Cells["DiaChi"].Value.ToString();

                
                string maBM = row.Cells["MaBM"].Value.ToString();
                cboBoMon.SelectedValue = maBM;

                isAdding = false;
                txtMaGV.ReadOnly = true;
            }
        }

       
    }
}
