using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public class DateField : ISqlField
	{
		public DateField(DateTime dateTime)
		{
			Value = dateTime;
		}

		public bool HasTime => Value.Hour != 0 || Value.Minute != 0 || Value.Second != 0 || Value.Millisecond != 0;

		public DateTime Value { get; set; }

		public void Accept(ISqlComponentVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
