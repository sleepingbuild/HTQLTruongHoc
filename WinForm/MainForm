using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SchoolManagement
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void OpenMdiChildForm(Form frm)
        {
            foreach(Form f in this.MdiChildren)
            {
                if(f.GetType()== frm.GetType())
                {
                    f.Activate();
                    return;
                }
            }
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }
        private void mnuHocSinh_Click(object sender, EventArgs e)
        {
            OpenMdiChildForm(new FormHocSinh());
        }
        private void mnuGiaoVien_Click(object sender, EventArgs e)
        {
            OpenMdiChildForm(new FormGiaoVien());
        }
        private void mnuLopHoc_Click(object sender, EventArgs e)
        {
            OpenMdiChildForm(new LopHoc());
        }
        private void mnuMonHoc_Click(object sender, EventArgs e)
        {
            OpenMdiChildForm(new MonHoc());
        }
        private void mnuQuanLyDiem_Click(object sender, EventArgs e)
        {
            OpenMdiChildForm(new Diem());
        }
        private void mnuXemBaoCao_Click(object sender, EventArgs e)
        {
            OpenMdiChildForm(new FormBaoCao());
        }
        private void mnuThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
