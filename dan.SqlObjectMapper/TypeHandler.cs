using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlObjectMapper
{
    public class TypeHandler<TClr, TDb> : ITypeHandler
    {
        public TypeHandler(Func<TClr, TDb> writeToDb, Func<TDb, TClr> readFromDb)
        {
            WriteToDatabase = writeToDb;
            ReadFromDatabase = readFromDb;
        }

        public Type Type { get; } = typeof(TClr);

        public Type DbType { get; } = typeof(TDb);

        public Func<TClr, TDb> WriteToDatabase { get; }

        public Func<TDb, TClr> ReadFromDatabase { get; }

        object ITypeHandler.ReadValue(object dbValue)
        {
            return ReadFromDatabase((TDb)dbValue);
        }

        object ITypeHandler.SetParameterValue(object value)
        {
            return WriteToDatabase((TClr)value);
        }
    }
}
