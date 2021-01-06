using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public class ArrayField : ISqlField, ISqlTable
	{
		public ArrayField(IEnumerable<int> data)
		{
			foreach (var item in data)
				Data.Add(item);
		}

		public ArrayField(IEnumerable<string> data)
		{
			foreach (var item in data)
				Data.Add(item);
		}

		public List<object> Data = new List<object>();

		string ISqlTable.Alias { get => null; set { } }

		public static implicit operator ArrayField(int[] data)
		{
			return new ArrayField(data);
		}

		public static implicit operator ArrayField(string[] data)
		{
			return new ArrayField(data);
		}

		public void Accept(ISqlComponentVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
