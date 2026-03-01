using System.Linq.Expressions;
using Tables.Data.Enums;


namespace Tables.Helpers
{
	internal static class ExpressionHelper
	{
		public static Expression<T> OnFilter<T>(Expression property, ParameterExpression parameter, string filter, FilterType filterType)
		{
			if (property is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
				property = unary.Operand;

			var filterExpression = GetFilterExpression(property, filter, filterType);

			return Expression.Lambda<T>(filterExpression, parameter);
		}

		private static Expression GetFilterExpression(Expression property, string filter, FilterType filterType)
		{
			var propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

			var convertedValue = Convert.ChangeType(filter, propertyType);

			var constant = Expression.Constant(convertedValue, property.Type);

			Expression filterExpression;

			if (filterType == FilterType.Contains)
			{
				var toLower = Expression.Call(property, "ToLower", null);
				var constantLower = Expression.Constant(filter.ToLower());
				filterExpression = Expression.Call(toLower, "Contains", null, constantLower);
			}
			else if (filterType == FilterType.Equals)
			{
				filterExpression = Expression.Equal(property, constant);
			}
			else if (filterType == FilterType.DoesNotEqual)
			{
				filterExpression = Expression.NotEqual(property, constant);
			}
			else
			{
				filterExpression = Expression.Constant(true);
			}

			return filterExpression;
		}
	}
}
