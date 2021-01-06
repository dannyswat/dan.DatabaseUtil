using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlObjectMapper
{
    public interface ITypeHandler
    {
        object SetParameterValue(object value);

        object ReadValue(object dbValue);
    }
}
