using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CumulativeProject.Models.ViewModels
{
    public class ShowTeacherVeiw
    {
        public Teacher Teacher { get; set; }
        public List<TeacherClass> TeacherClasses { get; set; }
    }
}