using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public class IntField : ISqlField
	{
		public IntField() { }

		public IntField(int value)
		{
			Value = value;
		}

		public int Value { get; set; }

		public void Accept(ISqlComponentVisitor visitor)
		{
			visitor.Visit(this);
		}

		public static implicit operator IntField(int value)
		{
			return new IntField(value);
		}
	}
}
