using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class AggregateField : ISqlField
    {
        public AggregateFunction Function { get; set; }

        public ISqlField Field { get; set; }

        public bool IsDistinct { get; set; }

        public List<SortField> PartitionBy { get; set; }

        public List<SortField> OrderBy { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
