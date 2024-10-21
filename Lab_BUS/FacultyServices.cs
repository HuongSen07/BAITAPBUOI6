using Lab_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_BUS
{
    public class FacultyServices
    {
        public List<Faculty> GetAll()
        {
            StudentModels context = new StudentModels();
            return context.Faculties.ToList();
        }
    }
}
