using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        Like,
        NotLike,
        Between,
        NotBetween,
        In,
        NotIn,
        Exists,
        NotExists,
        IsNull,
        IsNotNull
    }
}
