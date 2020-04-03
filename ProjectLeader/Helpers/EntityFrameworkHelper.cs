using MongoBaseRepository.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectLeader.Helpers
{
    public static class EntityFrameworkHelper
    {
        public static void AddRange<T>(this IList<T> instance, IEnumerable<T> collection)
        {
            if (instance != null)
            {
                if (collection != null)
                    foreach (T item in collection)
                        instance.Add(item);
            }
        }
    }
}
