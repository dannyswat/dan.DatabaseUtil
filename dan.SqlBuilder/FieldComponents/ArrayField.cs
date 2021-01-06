using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
	public class ArrayField : ISqlField, ISqlTable
	{
		public ArrayField(params int[] data)
		{
			foreach (var item in data)
				Data.Add(item);

			DataType = typeof(int);
		}

		public ArrayField(params string[] data)
		{
			Data.AddRange(data);

			DataType = typeof(string);
		}

		public ArrayField(params Guid[] data)
		{
			foreach (var item in data)
				Data.Add(item);

			DataType = typeof(Guid);
		}

		public List<object> Data = new List<object>();

		public Type DataType { get; private set; }

		string ISqlTable.Alias { get => null; set { } }

		public void Accept(ISqlComponentVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
