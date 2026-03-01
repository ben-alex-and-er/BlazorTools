using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace EFTables.Components
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class EFTable<TItem> : ComponentBase
	{
		[EditorRequired]
		[Parameter]
		public IQueryable<TItem> Query { get; set; }

		[Parameter]
		public bool ReadOnly { get; set; } = true;

		[Parameter]
		public RenderFragment? ChildContent { get; set; }


		// Pagination
		[Parameter]
		public int Page { get; set; } = 1;

		[Parameter]
		public int PageSize { get; set; } = int.MaxValue;

		public int TotalCount { get; private set; }
		public int TotalPages => TotalCount == 0
			? 1
			: (int)Math.Ceiling((double)TotalCount / PageSize);

		private IQueryable<TItem> newQuery = Enumerable.Empty<TItem>().AsQueryable();
		private Expression<Func<TItem, bool>>? currentFilter;
		private Expression<Func<TItem, object?>>? currentSort;
		private bool currentSortDescending;


		protected override async Task OnParametersSetAsync()
		{
			await LoadData();
		}


		private async Task LoadData()
		{
			var query = ReadOnly
				? Query.AsNoTracking()
				: Query;

			// Filter
			if (currentFilter != null)
				query = query.Where(currentFilter);

			// Sort
			if (currentSort != null)
				query = currentSortDescending
					? query.OrderByDescending(currentSort)
					: query.OrderBy(currentSort);

			// Count
			TotalCount = await query.CountAsync();

			// Paginate
			newQuery = query
				.Skip((Page - 1) * PageSize)
				.Take(PageSize);

			await InvokeAsync(StateHasChanged);
		}

		private async Task SetPage(int newPage)
		{
			if (newPage < 1 || newPage > TotalPages)
				return;

			Page = newPage;
			await LoadData();
		}

		private async Task OnSort((Expression<Func<TItem, object?>>, bool) obj)
		{
			(currentSort, currentSortDescending) = obj;
			await SetPage(1);
		}

		private async Task OnFilter(Expression<Func<TItem, bool>> expression)
		{
			currentFilter = expression;
			await SetPage(1);
		}
	}
}