using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class Condition : ISqlCondition, ICloneable
    {
        public Condition() { }

        public Condition(ISqlField field1, ComparisonOperator oper, ISqlField field2 = null)
        {
            validate(field1, oper, field2);

            Field1 = field1;
            Field2 = field2;
            Operator = oper;
        }

        public ISqlField Field1 { get; set; }

        public ISqlField Field2 { get; set; }

        public ComparisonOperator Operator { get; set; }

        static void validate(ISqlField field1, ComparisonOperator oper, ISqlField field2)
        {
            switch (oper)
            {
                case ComparisonOperator.IsNotNull:
                case ComparisonOperator.IsNull:
                    if (field2 != null)
                        throw new InvalidOperationException("Field2 should be null for selected operator");
                    break;
                case ComparisonOperator.Exists:
                case ComparisonOperator.NotExists:
                    if (field2 != null)
                        throw new InvalidOperationException("Field2 should be null for selected operator");
                    if (!(field1 is ISqlTable))
                        throw new InvalidOperationException("Field2 should be query or table for selected operator");
                    break;
                case ComparisonOperator.Between:
                case ComparisonOperator.NotBetween:
                    if (!(field2 is BetweenField))
                        throw new InvalidOperationException("Field2 should be BetweenField for selected operator");
                    break;
            }
        }

        public void Validate()
        {
            validate(Field1, Operator, Field2);
        }

        public void CopyTo(Condition other)
        {
            other.Field1 = Field1;
            other.Field2 = Field2;
            other.Operator = Operator;
        }

        public virtual void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }

        public object Clone()
        {
            Condition condition = new Condition();
            CopyTo(condition);
            return condition;
        }
    }
}
