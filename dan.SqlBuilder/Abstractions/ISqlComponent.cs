using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public interface ISqlComponent
	{
		void Accept(ISqlComponentVisitor visitor);
	}
}
