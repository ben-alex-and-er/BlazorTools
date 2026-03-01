using Microsoft.AspNetCore.Components;
using Tables.Components.Internal;


namespace Tables.Components
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class Table<TItem> : TableBase<TItem>
	{
		[Parameter]
		public IEnumerable<TItem> Items { get; set; } = [];

		[Parameter]
		public RenderFragment? ChildContent { get; set; }


		private IEnumerable<TItem> newItems = [];


		protected override async Task LoadData()
		{
			var items = Items;

			// Filter
			foreach (var filter in currentFilters)
			{
				if (filter != null)
				{
					items = items.Where(filter.Compile());
				}
			}

			// Sort
			if (currentSort != null)
			{
				items = currentSortDescending
					? items.OrderByDescending(currentSort.Compile())
					: items.OrderBy(currentSort.Compile());
			}

			// Count
			TotalCount = items.Count();

			// Paginate
			newItems = items
				.Skip((Page - 1) * PageSize)
				.Take(PageSize);

			await InvokeAsync(StateHasChanged);
		}
	}
}