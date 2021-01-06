using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class Query : ISqlTable, ISqlField
    {
        public List<SelectField> Select { get; set; } = new List<SelectField>();

        public bool SelectDistinct { get; set; } = false;

        public ISqlTable From { get; set; }

        public List<TableJoin> JoinTables { get; set; } = new List<TableJoin>();

        public ConditionClause Where { get; set; }

        public List<ISqlField> GroupBy { get; set; } = new List<ISqlField>();

        public ConditionClause Having { get; set; }

        public List<SortField> OrderBy { get; set; } = new List<SortField>();

        public int? Offset { get; set; }

        public int? PageSize { get; set; }

        string ISqlTable.Alias { get; set; }

        public virtual void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
