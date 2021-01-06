using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class TableJoin : ISqlComponent
    {
        public ISqlTable Table { get; set; }

        public ISqlTable ForeignTable { get; set; }

        public virtual void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
