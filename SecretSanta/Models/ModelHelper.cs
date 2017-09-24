using System;
using System.Linq.Expressions;

namespace SecretSanta.Models
{
    public static class ModelHelper
    {
        public static Expression<Func<TModel, string>>[] SelectProperties<TModel>(params Expression<Func<TModel, string>>[] selectors) => selectors;
    }
}