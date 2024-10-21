using Lab_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_BUS
{
    public class StudentServices
    {
        public List<Student> GetAll()
        {
            StudentModels context = new StudentModels();
            return context.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            StudentModels context = new StudentModels();
            return context.Students.Where(s => s.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentModels context = new StudentModels();
            return context.Students.Where(s => s.MajorID == null && s.FacultyID == facultyID).ToList();
        }
        public Student FindByID(int id)
        {
            StudentModels context = new StudentModels();
            return context.Students.FirstOrDefault(s => s.StudentID == id);
        }
        public void InsertUpdate(Student student)
        {
            StudentModels context = new StudentModels();
            context.Students.Add(student);
            context.SaveChanges();
        }
    }
}
