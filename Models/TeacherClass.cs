using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CumulativeProject.Models
{
    public class TeacherClass
    {
        public int Id { get; set; }

        public string ClassCode { get; set; }
        public int TeacherId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string ClassName { get; set; }

    }
}