
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SchoolManagement
{
    public partial class LopHoc : Form
    {
        private bool isAdding = false;
        public LopHoc()
        {
            InitializeComponent();
        }
        private void LoadData()
        {
            string queryLop =
                "SELECT lh.MaLop, lh.TenLop, gv.HoTen AS TenGVCN, lh.MaGVCN " +
                "FROM LopHoc lh LEFT JOIN GiaoVien gv ON lh.MaGVCN = gv.MaGV";
            dgvLopHoc.DataSource = DBHelper.ExecuteQuery(queryLop);
            string queryGV = "SELECT MaGV, HoTen FROM GiaoVien";
            DataTable dtGV = DBHelper.ExecuteQuery(queryGV);
            cboGVCN.DataSource = dtGV;
            cboGVCN.DisplayMember = "HoTen";
            cboGVCN.ValueMember = "MaGV";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            isAdding = true;
            txtMaLop.Clear();
            txtTenLop.Clear();
            cboGVCN.SelectedIndex = -1;
            txtMaLop.ReadOnly = false;
            txtMaLop.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLop.Text))
            {
                MessageBox.Show("Vui lòng chọn một lớp học để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isAdding = false;
            txtMaLop.ReadOnly = true;
            MessageBox.Show("Bạn đang ở chế độ Sửa. Hãy thay đổi thông tin và nhấn Lưu.", "Thông báo");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLop.Text)) return;
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa lớp học này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string query = $"DELETE FROM LopHoc WHERE MaLop = '{txtMaLop}'";
                if (DBHelper.ExecuteNonQuery(query) > 0)
                {
                    MessageBox.Show("Xóa lớp học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    txtMaLop.Clear();
                    txtTenLop.Clear();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Có thể lớp học này đang được sử dụng trong các bảng khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnLuu_CLick(object sender, EventArgs e)
        {
            string maLop = txtMaLop.Text.Trim();
            string tenLop = txtTenLop.Text.Trim();
            string maGVCN = cboGVCN.SelectedValue?.ToString() ?? "NULL";
            string query = "";
            string action = "";
            if (isAdding)
            {
                action = "thêm";
                query = $"INSERT INTO LopHoc (MaLop, TenLop, MaGVCN) VALUES ('{maLop}', '{tenLop}', '{maGVCN}')";
            }
            else
            {
                action = "cập nhật";
                query = $"UPDATE LopHoc SET TenLop = '{tenLop}', MaGVCN = '{maGVCN}' WHERE MaLop = '{maLop}'";
            }
            if (DBHelper.ExecuteNonQuery(query) > 0)
            {
                MessageBox.Show($"{action} lớp học thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show($"{action} lớp học thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvLopHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLopHoc.Rows[e.RowIndex];
                txtMaLop.Text = row.Cells["MaLop"].Value.ToString();
                txtTenLop.Text = row.Cells["TenLop"].Value.ToString();

                if (row.Cells["GVCN"].Value != DBNull.Value)
                {
                    cboGVCN.SelectedValue = row.Cells["MaGVCN"].Value.ToString();
                }
                else
                {
                    cboGVCN.SelectedIndex = -1;
                }
                isAdding = false;
                txtMaLop.ReadOnly = true;
            }

        }
    }
}

