using dan.SqlObjectMapper;
using System.Text.Json;

namespace dan.UnitTest.TypeHandlers
{
    public class JsonTypeHandler<TClr> : TypeHandler<TClr, string>
	{
		public JsonTypeHandler() : base(writeToDb, readFromDb)
		{
		}

		static string writeToDb(TClr obj)
		{
			if (obj == null)
				return null;
			return JsonSerializer.Serialize(obj);
		}

		static TClr readFromDb(string json)
		{
			return JsonSerializer.Deserialize<TClr>(json);
		}
	}
}
