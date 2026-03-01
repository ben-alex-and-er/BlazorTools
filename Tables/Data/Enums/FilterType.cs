namespace Tables.Data.Enums
{
	public enum FilterType
	{
		None = 0,
		Equals = 1,
		DoesNotEqual = 2,
		Contains = 3
	}

	public static class FilterTypeExtensions
	{
		public static string GetLabel(this FilterType type) => type switch
		{
			FilterType.None => "No Filter",
			FilterType.Equals => "Equals",
			FilterType.DoesNotEqual => "Does not equal",
			FilterType.Contains => "Contains",
			_ => type.ToString()
		};
	}
}
