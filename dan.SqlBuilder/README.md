# dan.SqlBuilder
Utility to build a SQL query from C# code
The library uses the visitor pattern to build the SQL
The following code is to show how to use the utility

    Query query = new Query();
    query.From = new Table("Products");
    MsSqlGenerator.GenerateSql(query);
    
The next milestone is to make the library more user friendly with builder pattern

    var query = new QueryBuilder()
		.From(new Table("Products", "p1"))
		.InnerJoin(new Table("p1"), new Table("Variants"), new DbField("Id", "p1"), new DbField("ProductId", "Variants"))
		.Where(new DbField("Id", "p1"), ComparisonOperator.In, new ArrayField(new int[] { 1, 3, 6, 7, 8 }))
		.Select(
			new DbField("Name", "p1"),
			new DbField("Id", "p1"),
			new SelectField(new RowNumberField().Order(new SortField(new DbField("Id", "p1"))), "RowNum")
		)
		.Take(100)
		.Build();
	MsSqlGenerator.GenerateSql(query);