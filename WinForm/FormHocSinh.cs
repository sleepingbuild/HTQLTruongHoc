using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SchoolManagement
{
    public partial class FormHocSinh : Form
    {   
        private bool isAdding = false;
        public FormHocSinh()
        {
            InitializeComponent();
        }
        public void LoadData() 
        {
            string queryHS =
                "SELECT MaHS , Hoten,NgaySinh,GioiTinh,DiaChi,SDT,MaLop " + 
                "FROM HocSinh hs JOIN LopHoc l ON hs.Malop = l.MaLop";
            dgvHocSinh.DataSource = DBHelper.ExecuteQuery(queryHS);

            string queryLop ="SELECT MaLop, TenLop FROM LopHoc";
            DataTable dtLop = DBHelper.ExecuteQuery(queryLop);

            cboLopHoc.DataSource = dtLop;
            cboLopHoc.DisplayMember = "TenLop";
            cboLopHoc.ValueMember = "MaLop";
        }

        private void FormHocSinh_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void btnThem_Click(object sender, EventArgs e) 
        {
            isAdding = true;
           
            txtMaHS.Clear();
            txtHoTen.Clear();
            txtDiaChi.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboGioiTinh.SelectedIndex = -1;
            cboLopHoc.SelectedIndex = -1;

            txtMaHS.ReadOnly = false;
            txtMaHS.Focus();

        }
        private void btnLuu_Click(object sender, EventArgs e) 
        {
            string maHS = txtMaHS.Text; 
            string hoTen = txtHoTen.Text;
            string ngaySinh = dtpNgaySinh.Value.ToString("yyyy-MM-dd");
            string gioiTinh = cboGioiTinh.Text;
            string diaChi = txtDiaChi.Text;
            string maLop = cboLopHoc.SelectedValue.ToString();

            if (string.IsNullOrEmpty(maHS) || string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(maLop))
            {
                MessageBox.Show("ma hoc sinh , ho ten va lop hoc khong duoc de trong!", "Loi nhap lieu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = "";
            string action = "";

            if (isAdding)
            {
                action = "Them";
                query = $"INSERT INTO HocSinh (MaHS, HoTen, NgaySinh, GioiTinh, DiaChi, MaLop) " +
                        $"VALUES ('{maHS}', '{hoTen}', '{ngaySinh}', '{gioiTinh}', '{diaChi}', '{maLop}')";
            }
            else
            {
                action = "Cap nhat";
                query = $"UPDATE HocSinh SET " +
                        $"HoTen = '{hoTen}', " +
                        $"NgaySinh = '{ngaySinh}', " +
                        $"GioiTinh = '{gioiTinh}', " +
                        $"DiaChi = '{diaChi}', " +
                        $"MaLop = '{maLop}' " +
                        $"WHERE MaHS = '{maHS}'";
            }

            int result = DBHelper.ExecuteNonQuery(query);
            if (result > 0)
            {
                MessageBox.Show( action +"hoc sinh thanh cong!","Thanh cong", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   
                LoadData();
                isAdding = false;
                txtMaHS.ReadOnly = false;
            }
            else
            {
                MessageBox.Show($"{action} hoc sinh that bai! Ma hoc sinh co the da ton tai hoac loi CSDL",
                    "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSua_Click(object sender, EventArgs e) 
        {
           if(string.IsNullOrEmpty(txtMaHS.Text))
            {
                MessageBox.Show("Vui long chon hoc sinh de sua!","Thong bao",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            isAdding = false;
            txtMaHS.ReadOnly = true;

            MessageBox.Show("Ban dang o che do sua.Hay thay doi thong tin va nhan Luu.", "Thong bao");

        }
        private void btnXoa_Click(object sender , EventArgs e)
        {
            if(string.IsNullOrEmpty(txtMaHS.Text))
            {
                MessageBox.Show("Vui long chon hoc sinh de xoa!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maHS = txtMaHS.Text;
            DialogResult dr = MessageBox.Show($"Ban co chac muon xoa hoc sinh {maHS}?\n"+
                "Luu y: hanh dong nay khong the hoan tac!", 
                "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dr == DialogResult.Yes)
            {
                string query = $"DELETE FROM HocSinh WHERE MaHS = '{maHS}'";
                int result = DBHelper.ExecuteNonQuery(query);
                if(result > 0)
                {
                    MessageBox.Show("Xoa hoc sinh thanh cong!");
                    LoadData();
                    txtMaHS.Clear();txtHoTen.Clear();
                }
                else
                {
                    MessageBox.Show("Xoa hoc sinh that bai!Ma hoc sinh khong ton tai hoac loi CSDL",
                        "Loi",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
       
    }
}
