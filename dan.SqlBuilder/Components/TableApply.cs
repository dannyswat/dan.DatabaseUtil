using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class TableApply : TableJoin
    {
        public TableApplyType JoinType { get; set; }

        public new Query ForeignTable { get => base.ForeignTable as Query; set => base.ForeignTable = value; }

        public override void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
