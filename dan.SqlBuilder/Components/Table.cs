using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class Table : ISqlTable
    {
        public Table() { }

        public Table(string table, string alias = null, string schema = null)
        {
            Name = table;
            Schema = schema;
            Alias = alias;
        }

        public string Name { get; set; }

        public string Schema { get; set; }

        public string Alias { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
