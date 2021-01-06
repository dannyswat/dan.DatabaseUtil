using NUnit.Framework;
using dan.SqlBuilder;

namespace dan.UnitTest
{
	public class SqlBuilderTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Test1()
		{
			var query = new QueryBuilder()
				.From(new Table("Products", "p1"))
				.InnerJoin(new Table("p1"), new Table("Variants"), new DbField("Id", "p1"), new DbField("ProductId", "Variants"))
				.Where(new DbField("Id", "p1"), ComparisonOperator.In, new ArrayField(1, 3, 6, 7, 8))
				.Select(
					new DbField("Name", "p1"),
					new DbField("Id", "p1"),
					new RowNumberField().Order(new DbField("Id", "p1"))
				)
				.Take(100)
				.Build();
			Assert.AreEqual("SELECT TOP 100 [p1].[Name], [p1].[Id], ROW_NUMBER() OVER (ORDER BY [p1].[Id] ASC) AS [RowNum] FROM [dbo].[Products] AS [p1] INNER JOIN [dbo].[Variants] ON [p1].[Id] = [Variants].[ProductId] WHERE ([p1].[Id] IN (1, 3, 6, 7, 8))", MsSqlGenerator.GenerateSql(query));
		}
	}
}