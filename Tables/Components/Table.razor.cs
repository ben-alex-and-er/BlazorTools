using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;


namespace Tables.Components
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class Table<TItem> : ComponentBase
	{
		[Parameter]
		public IEnumerable<TItem> Items { get; set; } = [];

		[Parameter]
		public RenderFragment? ChildContent { get; set; }


		private void OnSort((Expression<Func<TItem, object?>>, bool) obj)
		{
			var (field, descending) = obj;

			Items = descending
				? Items.OrderByDescending(field.Compile())
				: Items.OrderBy(field.Compile());
		}
	}
}