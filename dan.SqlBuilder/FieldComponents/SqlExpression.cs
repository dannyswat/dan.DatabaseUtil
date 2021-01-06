using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class SqlExpression : ISqlField
    {
        public SqlExpression(string expr)
        {
            Expression = expr;
        }

        public string Expression { get; set; }
        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
