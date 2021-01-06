using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class CommonTableExpression : Query
    {
        public string Name { get; set; }

        public override void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
