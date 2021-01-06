using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlBuilder
{
    public interface ISqlGenerator : ISqlComponentVisitor
    {
        string GenerateSql();
    }
}
