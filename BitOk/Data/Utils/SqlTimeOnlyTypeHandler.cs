﻿using System.Data;
using Dapper;

namespace BitOk.Data.Utils
{
    public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly time)
            => parameter.Value = time.ToString();

        public override TimeOnly Parse(object value)
            => TimeOnly.FromTimeSpan((TimeSpan)value);
    }
}