using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class CompositeQuery : ISqlTable
    {
        public List<CommonTableExpression> CommonTableExpression { get; set; } = new List<CommonTableExpression>();

        public Query Query { get; set; }

        public List<UnionQuery> UnionQueries { get; set; } = new List<UnionQuery>();

        string ISqlTable.Alias { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
