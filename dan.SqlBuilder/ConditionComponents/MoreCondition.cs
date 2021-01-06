using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class MoreCondition : ISqlComponent
    {
        public MoreCondition(LogicalOperator logicalOperator, ISqlCondition condition)
        {
            LogicalOperator = logicalOperator;
            Condition = condition;
        }

        public MoreCondition(LogicalOperator logicalOperator, ISqlField field1, ComparisonOperator oper, ISqlField field2 = null)
        {
            LogicalOperator = logicalOperator;
            Condition = new Condition(field1, oper, field2);
        }

        public LogicalOperator LogicalOperator { get; set; }

        public ISqlCondition Condition { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
