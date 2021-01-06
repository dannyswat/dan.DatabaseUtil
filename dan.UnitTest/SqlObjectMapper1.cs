using NUnit.Framework;
using dan.SqlObjectMapper;
using Microsoft.Data.Sqlite;
using System.Linq;
using dan.SqlBuilder;

namespace dan.UnitTest
{
    public class SqlObjectMapperTests
    {
        static string ConnectionString = "Data Source=TestDb;Mode=Memory;Cache=Shared";

        SqliteConnection conn;

        [SetUp]
        public void ConfigureDatabase()
        {
            conn = new SqliteConnection(ConnectionString);
            conn.Open();
            conn.Execute(@"CREATE TABLE Products (
ID INTEGER PRIMARY KEY,
Name TEXT NOT NULL,
Json TEXT NULL,
Desc TEXT NOT NULL)", null);

            conn.Execute(@"INSERT INTO Products (ID, Name,Json,Desc)
Values (1, 'Test', '{""LastName"":""ABC"", ""FirstName"":""DEF""}', 'Hello')", null);

            conn.Execute(@"INSERT INTO Products (ID, Name,Desc)
Values (2, 'Test 2', 'Hello too')", null);
        }

        [Test]
        public void BasicTest()
        {
            SimpleObject[] result;

            result = conn.Query<SimpleObject>("SELECT ID, Name FROM Products", null).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Test", result[0].Name);

            conn.Close();
        }

        [Test]
        public void TwoLevelObjectTest()
        {
            SimpleObject[] result;

            result = conn.Query<SimpleObject>("SELECT ID, Name, Desc AS [Detail.Name] FROM Products WHERE ID = @v1", new { v1 = 1 }).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("Test", result[0].Name);
            Assert.AreEqual("Hello", result[0].Detail.Name);

            conn.Close();
        }

        [Test]
        public void TwoLevelObjectWithBuilderTest()
        {
            Query query = new Query();
            query.Select.Add(new SelectField("ID"));
            query.Select.Add(new SelectField("Name"));
            query.From = new Table("Products");
            query.Where = new ConditionClause(new Condition { Field1 = new DbField("Name"), Field2 = new Variable("name"), Operator = ComparisonOperator.Equal });
            query.OrderBy.Add(new SortField("Name"));

            MsSqlGenerator sqlGenerator = new MsSqlGenerator();
            sqlGenerator.DefaultTableSchema = null;
            query.Accept(sqlGenerator);
            Assert.AreEqual("SELECT [ID], [Name] FROM [Products] WHERE ([Name] = @name) ORDER BY [Name] ASC", sqlGenerator.GenerateSql());

            SimpleObject[] result = conn.Query<SimpleObject>(sqlGenerator.GenerateSql(), new { name = "Test" }).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].ID);

            conn.Close();
        }

        [Test]
        public void BasicTypeConverterTest()
        {
            TypeHandlerManager.Add(new TypeHandlers.JsonTypeHandler<JsonObject>());

            SimpleObject[] result;

            result = conn.Query<SimpleObject>("SELECT ID, Name, Json FROM Products", null).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Test", result[0].Name);
            Assert.IsNotNull(result[0].Json);
            Assert.AreEqual(result[0].Json.LastName, "ABC");

            conn.Close();
        }

        class SimpleObject
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public JsonObject Json { get; set; }

            public SubObject Detail { get; set; }
        }

        class JsonObject
        {
            public string LastName { get; set; }

            public string FirstName { get; set; }
        }

        class SubObject
        {
            public string Name { get; set; }
        }
    }
}
