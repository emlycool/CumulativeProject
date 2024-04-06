using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CumulativeProject.Repositories.Utils
{
    public class QueryConstraint
    {
        protected string Column;
        
        protected string Value;

        protected string Comparison = "=";

        public QueryConstraint(string Column, string Value) 
        {
            this.Column = Column;
            this.Value = Value;
        }

        public QueryConstraint(string column, string comparison, string value)
        {
            this.Column = column;
            this.Value = value;
            this.Comparison = comparison;
        }
    }
}