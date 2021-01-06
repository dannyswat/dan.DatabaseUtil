using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class IsNullField : ISqlField
    {
        public ISqlField Field { get; set; }

        public ISqlField WhenNull { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
