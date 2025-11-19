using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SchoolManagement
{
    public partial class MonHoc : Form
    {  
        private bool isAdding = false;
        public MonHoc()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string queryMon =
                "SELECT mh.MaMon,mh.TenMon, mh.HoTen AS TenGVPhuTrach, mh.MaGVPhuTrach " +
                "FROM MonHoc mh LEFT JOIN GiaoVien gv ON mh.MaGVPhuTrach = gv.MaGV";
            dgvMonHoc.DataSource = DBHelper.ExecuteQuery(queryMon);

            string queryGV = "SELECT MaGV, HoTen FROM GiaoVien";
            DataTable dtGV = DBHelper.ExecuteQuery(queryGV);

            cboGVPT.DataSource = dtGV;
            cboGVPT.DisplayMember = "HoTen";
            cboGVPT.ValueMember = "MaGV";
        }
        private void MonHoc_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            isAdding = true;
            txtMaMon.Clear();
            txtTenMon.Clear();
            cboGVPT.SelectedIndex = -1;
            txtMaMon.ReadOnly = false;
            txtMaMon.Focus();

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaMon.Text))
            {
                MessageBox.Show("Vui lòng chọn một môn học để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isAdding = false;
            txtMaMon.ReadOnly = true;
            MessageBox.Show("Bạn đang ở chế độ Sửa. Hãy thay đổi thông tin và nhấn Lưu.", "Thông báo");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaMon.Text)) return;

            string maMon = txtMaMon.Text;
            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa môn học {maMon}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(dr == DialogResult.Yes)
            {
                string query = $"DELETE FROM MonHoc WHERE MaMon = '{maMon}'";
                if (DBHelper.ExecuteNonQuery(query) > 0)
                {
                    MessageBox.Show("Xóa môn học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    txtMaMon.Clear();
                    txtTenMon.Clear();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Có thể môn học này đang được sử dụng trong các bảng khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            string maMon = txtMaMon.Text.Trim();
            string tenMon = txtTenMon.Text.Trim();
            string maGVPT = cboGVPT.SelectedValue?.ToString() ?? "Null";
            string query = "";
            string action = "";
            if (string.IsNullOrEmpty(maMon) || string.IsNullOrEmpty(tenMon))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin môn học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (isAdding)
            {
                action = "Thêm";                    
                query = $"INSERT INTO MonHoc (MaMon, TenMon, MaGVPhuTrach) VALUES ('{maMon}', '{tenMon}', '{maGVPT}')";
            }
            else
            {
                action = "Cập nhật";
                query = $"UPDATE MonHoc SET TenMon = '{tenMon}', MaGVPhuTrach = '{maGVPT}' WHERE MaMon = '{maMon}'";
            }
            if (DBHelper.ExecuteNonQuery(query) > 0)
            {
                MessageBox.Show(action + "môn học thành công");
                LoadData();
            }
            else
            {
                MessageBox.Show(action + "môn học thất bại");
            }
        }
        private void dgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMonHoc.Rows[e.RowIndex];
                txtMaMon.Text = row.Cells["MaMon"].Value.ToString();
                txtTenMon.Text = row.Cells["TenMon"].Value.ToString();
                
                if(row.Cells["MaGVPhuTrach"].Value != DBNull.Value)
                {
                    cboGVPT.SelectedValue = row.Cells["MaGVPhuTrach"].Value.ToString();
                }
                else
                {
                    cboGVPT.SelectedIndex = -1;
                }
                isAdding = false;
                txtMaMon.ReadOnly = true;
            }
        }
        
    }
}

