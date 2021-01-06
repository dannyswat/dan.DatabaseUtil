using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public class NumberField : ISqlField
	{
		public NumberField() { }

		public NumberField(decimal value)
		{
			Value = RemoveTrailingZero(value);
		}

		public decimal Value { get; set; }

		public static decimal RemoveTrailingZero(decimal value)
		{
			return value / 1.000000000000000000000000000000000m;
		}

		public void Accept(ISqlComponentVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
