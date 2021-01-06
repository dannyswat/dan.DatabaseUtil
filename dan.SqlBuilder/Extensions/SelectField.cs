using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	partial class SelectField
	{
		public static implicit operator SelectField(DbField value)
		{
			return new SelectField(value);
		}
	}
}
