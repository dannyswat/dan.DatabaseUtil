using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public class StringField : ISqlField
    {
        public StringField(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public static implicit operator StringField(string value)
        {
            return new StringField(value);
        }

        public void Accept(ISqlComponentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
