using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class Variable : ISqlField
    {
        public Variable(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
