using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public interface ISqlTable : ISqlComponent
    {
        string Alias { get; set; }
    }
}
