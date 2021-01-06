using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	partial class SortField
	{
		public static implicit operator SortField(DbField value)
		{
			return new SortField(value);
		}

		public static implicit operator SortField(RowNumberField value)
		{
			return new SortField(value);
		}

		public static implicit operator SortField(CaseWhenField value)
		{
			return new SortField(value);
		}

		public static implicit operator SortField(CaseField value)
		{
			return new SortField(value);
		}

		public static implicit operator SortField(IsNullField value)
		{
			return new SortField(value);
		}
	}
}
