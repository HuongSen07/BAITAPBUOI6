using Lab_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyID)
        {
            StudentModels context = new StudentModels();
            return context.Majors.Where(s => s.FacultyID == facultyID).ToList();
        }
    }
}
