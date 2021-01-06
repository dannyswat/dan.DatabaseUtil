using System;
using System.Collections.Generic;
using System.Text;

namespace dan.SqlObjectMapper.Internal
{
    internal sealed class TypeMappingCache : StaticCache<Type, TypeMapping>
    {
        public static TypeMappingCache Instance { get; } = new TypeMappingCache();

        protected override TypeMapping CreateInstance(Type key)
        {
            return new TypeMapping(key);
        }
    }
}
