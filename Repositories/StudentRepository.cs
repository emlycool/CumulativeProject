using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CumulativeProject.Repositories
{
    public class StudentRepository : BaseRepository
    {
        protected override string Table { get { return "students"; } }
    }
}