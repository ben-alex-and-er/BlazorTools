using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Tables.Components.Internal;


namespace EFTables.Components
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class EFTable<TItem> : TableBase<TItem>
	{
		[Parameter, EditorRequired]
		public IQueryable<TItem> Query { get; set; }

		[Parameter]
		public bool ReadOnly { get; set; } = true;

		[Parameter]
		public RenderFragment? ChildContent { get; set; }


		private IQueryable<TItem> newQuery = Enumerable.Empty<TItem>().AsQueryable();


		protected override async Task LoadData()
		{
			var query = ReadOnly
				? Query.AsNoTracking()
				: Query;

			// Filter
			foreach (var filter in currentFilters)
			{
				if (filter != null)
				{
					query = query.Where(filter);
				}
			}

			// Sort
			if (currentSort != null)
			{
				query = currentSortDescending
					? query.OrderByDescending(currentSort)
					: query.OrderBy(currentSort);
			}

			// Count
			TotalCount = await query.CountAsync();

			// Paginate
			newQuery = query
				.Skip((Page - 1) * PageSize)
				.Take(PageSize);

			await InvokeAsync(StateHasChanged);
		}
	}
}