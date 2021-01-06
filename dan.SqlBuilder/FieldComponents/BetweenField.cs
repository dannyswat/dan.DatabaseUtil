using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class BetweenField : ISqlField
    {
        public ISqlField From { get; set; }

        public ISqlField To { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
