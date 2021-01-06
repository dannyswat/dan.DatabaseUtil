using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public partial class SelectField : ISqlComponent
    {
        public SelectField() { }

        public SelectField(ISqlField field, string alias = null)
        {
            Field = field;
            Alias = alias;
        }

        public SelectField(string dbField, string alias = null)
        {
            Field = new DbField(dbField);
            Alias = alias;
        }

        public ISqlField Field { get; set; }

        public string Alias { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
