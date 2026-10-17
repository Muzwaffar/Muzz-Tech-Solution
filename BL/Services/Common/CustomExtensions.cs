using DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public static partial class CustomExtensions
    {
        public static IQueryable Query(this NPFContext context, string entityName) =>context.Query(context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod = typeof(NPFContext).GetMethod(nameof(NPFContext.Set), Type.EmptyTypes);

        public static IQueryable Query(this NPFContext context, Type entityType) =>(IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
    }
}
