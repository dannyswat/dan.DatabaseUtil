using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class DbField : ISqlField
    {
        public DbField() { }

        public DbField(string fieldName)
        {
            FieldName = fieldName;
        }

        public DbField(string fieldName, string tableName)
        {
            FieldName = fieldName;
            TableName = tableName;
        }

        public string FieldName { get; set; }

        public string TableName { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
