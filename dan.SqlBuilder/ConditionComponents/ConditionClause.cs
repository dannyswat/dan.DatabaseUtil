using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public class ConditionClause : ISqlCondition
	{
		public ConditionClause(ISqlField field1, ComparisonOperator oper, ISqlField field2 = null)
		{
			Condition = new Condition(field1, oper, field2);
		}
		public ConditionClause(Condition condition, params MoreCondition[] others)
		{
			Condition = condition;
			if (others.Length > 0)
				OtherConditions.AddRange(others);

		}

		public ISqlCondition Condition { get; set; }

		public List<MoreCondition> OtherConditions { get; set; } = new List<MoreCondition>();

		public ConditionClause Or(ConditionClause subClause)
		{
			OtherConditions.Add(new MoreCondition(LogicalOperator.Or, subClause));
			return this;
		}

		public ConditionClause And(ConditionClause subClause)
		{
			OtherConditions.Add(new MoreCondition(LogicalOperator.And, subClause));
			return this;
		}

		public ConditionClause Or(ISqlField field1, ComparisonOperator oper, ISqlField field2 = null)
		{
			OtherConditions.Add(new MoreCondition(LogicalOperator.Or, field1, oper, field2));
			return this;
		}

		public ConditionClause And(ISqlField field1, ComparisonOperator oper, ISqlField field2 = null)
		{
			OtherConditions.Add(new MoreCondition(LogicalOperator.And, field1, oper, field2));
			return this;
		}

		public void Accept(ISqlComponentVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
