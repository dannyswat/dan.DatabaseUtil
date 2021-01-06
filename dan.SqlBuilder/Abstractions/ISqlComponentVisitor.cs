using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public interface ISqlComponentVisitor
	{
        void Visit(AggregateField component);
        void Visit(ArrayField component);
        void Visit(BetweenField component);
        void Visit(Condition component);
        void Visit(ConditionClause component);
        void Visit(MoreCondition component);
        void Visit(CaseField component);
        void Visit(CaseWhenField component);
        void Visit(RowNumberField component);
        void Visit(SelectField component);
        void Visit(SortField component);
        void Visit(Table component);
        void Visit(TableJoin component);
        void Visit(TableApply component);
        void Visit(TableRelation component);
        void Visit(Variable component);
        void Visit(SqlExpression component);
        void Visit(DbField component);
        void Visit(StringField component);
        void Visit(NumberField component);
        void Visit(IntField component);
        void Visit(DateField component);
        void Visit(IsNullField component);
        void Visit(CommonTableExpression component);
        void Visit(LocalForeignKey component);
        void Visit(Query component);
        void Visit(CompositeQuery component);
        void Visit(UnionQuery component);
    }
}
