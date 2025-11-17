using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SchoolManagement
{
    public partial class Diem : Form
    {
        public Diem()
        {
            InitializeComponent();
        }
        private void LoadDanhSach()
        {
            try
            {
                string queryLop = "SELECT MaLop, TenLop FROM LopHoc";
                DataTable dtLop = DBHelper.ExecuteQuery(queryLop);
                cboLopHoc.DataSource = dtLop;
                cboLopHoc.DisplayMember = "TenLop";
                cboLopHoc.ValueMember = "MaLop";

                string queryMon = "SELECT MaMon, TenMon FROM MonHoc";
                DataTable dtMon = DBHelper.ExecuteQuery(queryMon);
                cboMonHoc.DataSource = dtMon;
                cboMonHoc.DisplayMember = "TenMon";
                cboMonHoc.ValueMember = "MaMon";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách: " + ex.Message);
            }
        }

        private void FormDiem_Load(object sender, EventArgs e)
        {
            LoadDanhSach();
        }

        private void btnXemDanhSach_Click(object sender, EventArgs e)
        {
            if (cboLopHoc.SelectedValue == null || cboMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học và môn học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgvDiem.DataSource = null;
                return;
            }
            string maLop = cboLopHoc.SelectedValue.ToString();
            string maMon = cboMonHoc.SelectedValue.ToString();

            string query =
                "SELECT hs.MaHS, hs.HoTen, d.DiemSo,ds.MaMon" +
                " FROM HocSinh hs" +
                $"LEFT JOIN Diem ds ON hs.MaHS = ds.MaHS" +
                $"WHERE hs.MaLop = '{maLop}'" +
                "ORDER BY hs.HoTen";
            try
            {
                DataTable dtDiem = DBHelper.ExecuteQuery(query);
                dgvDiem.DataSource = dtDiem;

                dgvDiem.Columns["MaHS"].HeaderText = "Mã Học Sinh";
                dgvDiem.Columns["HoTen"].HeaderText = "Họ Tên";
                dgvDiem.Columns["DiemSo"].HeaderText = "Điểm Số";

                if (dgvDiem.Columns.Contains("MaMon")) dgvDiem.Columns["MaMon"].Visible = false;
                if (dgvDiem.Columns.Contains("MaHS")) dgvDiem.Columns["MaHS"].ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn dữ liệu điểm: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuuDiem_Click(object sender, EventArgs e)
        {
            if (dgvDiem.Rows.Count == 0 || cboMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Không có dữ liệu để lưu.", "Thông báo");
                return;
            }

            string maMon = cboMonHoc.SelectedValue.ToString();
            int successCount = 0;
            int errorCount = 0;

            foreach (DataGridViewRow row in dgvDiem.Rows)
            {
                if (row.IsNewRow || row.Cells["MaHS"].Value == null) continue;

                string maHS = row.Cells["MaHS"].Value.ToString();
                string diemText = row.Cells["DiemSo"].Value?.ToString();

                if (string.IsNullOrEmpty(diemText)) continue;
                if (!float.TryParse(diemText, out float diemSo) || diemSo < 0 || diemSo > 10)
                {
                    row.ErrorText = "Điểm phải là số và nằm trong khoảng 0-10.";
                    errorCount++;
                    continue;
                }

                bool diemDaCo = row.Cells["MaMon"].Value != null && row.Cells["MaMon"].Value != null;
                string query = "";


                try
                {
                    if (diemDaCo)
                    {
                        query = $"UPDATE DiemSo SET DiemSo = {diemSo} WHERE MaHS = '{maHS}' AND MaMon = '{maMon}'";
                    }
                    else
                    {
                        query = $"INSERT INTO DiemSo (MaHS, MaMon, DiemSo) VALUES ('{maHS}', '{maMon}', {diemSo})";
                    }
                    if (DBHelper.ExecuteNonQuery(query) > 0)
                    {
                        successCount++;
                        row.ErrorText = string.Empty;
                    }
                    else
                    {
                        errorCount++;
                        row.ErrorText = "Lưu CSDL thất bại.";
                    }

                }
                catch (Exception ex)
                {
                    errorCount++;
                    row.ErrorText = "Lỗi CSDL: " + ex.Message;
                }
            }
            MessageBox.Show($"Hoàn thành lưu điểm: {successCount} thành công, {errorCount} thất bại/lỗi nhập liệu.", "Kết quả Lưu");


            btnXemDanhSach_Click(sender, e);
        }
    }
}

