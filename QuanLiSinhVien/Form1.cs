using Lab_BUS;
using Lab_DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiSinhVien
{
    public partial class Form1 : Form
    {
        private readonly StudentServices studentService = new StudentServices();
        private readonly FacultyServices facultyService = new FacultyServices();
        public Form1()
        {
            InitializeComponent();
            dgvSinhVien.CellClick += dgvSinhVien_CellClick;

        }

        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cmbKhoa.DataSource = listFacultys;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try

            {
                setGridViewStyle(dgvSinhVien);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvSinhVien.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dgvSinhVien.Rows.Add();
                dgvSinhVien.Rows[index].Cells[0].Value = item.StudentID;
                dgvSinhVien.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dgvSinhVien.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvSinhVien.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dgvSinhVien.Rows[index].Cells[4].Value = item.Major.Name + "";
                //ShowAvatar(item.Avatar);
            }
        }

        private void setGridViewStyle(DataGridView dgvSinhVien)
        {
            dgvSinhVien.BorderStyle = BorderStyle.None;
            dgvSinhVien.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgvSinhVien.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgvSinhVien.BackgroundColor = Color.White;
            dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void chkDKChuyenNganh_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkDKChuyenNganh.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void ResetInputFields()
        {
            txtMaSV.Clear();
            txtTenSV.Clear();
            cmbKhoa.SelectedIndex = -1; // Bỏ chọn combobox
            txtDTB.Clear();
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy dòng được chọn
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];

                // Đổ dữ liệu từ dòng vào các TextBox
                txtMaSV.Text = row.Cells[0].Value.ToString();   // StudentID
                txtTenSV.Text = row.Cells[1].Value.ToString();  // FullName
                cmbKhoa.Text = row.Cells[2].Value.ToString();   // FacultyName
                txtDTB.Text = row.Cells[3].Value.ToString();    // AverageScore
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi chương trình? ",
                                               "Xác nhận thoát",
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            using (var db = new StudentModels())
            {
                string selectedFacultyName = cmbKhoa.Text;
                var faculty = db.Faculties.FirstOrDefault(f => f.FacultyName == selectedFacultyName);

                if (faculty != null)
                {
                    var newStudent = new Student
                    {
                        StudentID = int.Parse(txtMaSV.Text),
                        FullName = txtTenSV.Text,
                        FacultyID = faculty.FacultyID,
                        AverageScore = decimal.Parse(txtDTB.Text)
                    };
                    db.Students.Add(newStudent);
                    db.SaveChanges();

                    int index = dgvSinhVien.Rows.Add();
                    dgvSinhVien.Rows[index].Cells[0].Value = newStudent.StudentID;
                    dgvSinhVien.Rows[index].Cells[1].Value = newStudent.FullName;
                    dgvSinhVien.Rows[index].Cells[2].Value = faculty.FacultyName;
                    dgvSinhVien.Rows[index].Cells[3].Value = newStudent.AverageScore.ToString();

                    MessageBox.Show("Thêm sinh viên thành công", "Thông báo", MessageBoxButtons.OK);
                    ResetInputFields();
                }
                else
                {
                    MessageBox.Show("Khoa không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                ResetInputFields();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.SelectedRows.Count > 0)
            {
                // Lấy ID của sinh viên từ dòng được chọn
                int studentID = (int)dgvSinhVien.SelectedRows[0].Cells[0].Value;

                using (var db = new StudentModels())
                {
                    var student = db.Students.FirstOrDefault(s => s.StudentID == studentID);
                    if (student != null)
                    {
                        student.FullName = txtTenSV.Text;
                        string selectedFacultyName = cmbKhoa.Text;
                        var faculty = db.Faculties.FirstOrDefault(f => f.FacultyName == selectedFacultyName);
                        if (faculty != null)
                        {
                            student.FacultyID = faculty.FacultyID;
                        }
                        else
                        {
                            MessageBox.Show("Khoa không tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        student.AverageScore = decimal.Parse(txtDTB.Text);
                        db.SaveChanges();
                        var selectedRow = dgvSinhVien.SelectedRows[0];
                        selectedRow.Cells[1].Value = student.FullName;
                        selectedRow.Cells[2].Value = faculty.FacultyName;
                        selectedRow.Cells[3].Value = student.AverageScore.ToString();
                        MessageBox.Show("Cập nhật sinh viên thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.SelectedRows.Count > 0)
            {
                int studentID = (int)dgvSinhVien.SelectedRows[0].Cells[0].Value;
                var confirmResult = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?",
                                                     "Xác nhận xóa",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    using (var db = new StudentModels())
                    {
                        // Tìm sinh viên trong database
                        var student = db.Students.FirstOrDefault(s => s.StudentID == studentID);
                        if (student != null)
                        {
                            // Xóa sinh viên
                            db.Students.Remove(student);
                            db.SaveChanges();

                            // Xóa dòng khỏi DataGridView
                            dgvSinhVien.Rows.RemoveAt(dgvSinhVien.SelectedRows[0].Index);

                            MessageBox.Show("Đã xóa sinh viên thành công.");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sinh viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
