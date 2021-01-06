using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public partial class SortField : ISqlComponent
    {
        public SortField(string field, bool desc = false)
        {
            Field = new DbField(field);
            Descending = desc;
        }

        public SortField(ISqlField field, bool desc = false)
        {
            Field = field;
            Descending = desc;
        }

        public ISqlField Field { get; set; }

        public bool Descending { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
