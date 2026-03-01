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

		// Pagination
		[Parameter]
		public int Page { get; set; } = 1;

		[Parameter]
		public int PageSize { get; set; } = int.MaxValue;

		public int TotalCount { get; private set; }
		public int TotalPages => TotalCount == 0
			? 1
			: (int)Math.Ceiling((double)TotalCount / PageSize);


		private IEnumerable<TItem> newItems = [];

		private IEnumerable<Expression<Func<TItem, bool>>?> currentFilters = [];
		private Func<TItem, object?>? currentSort;
		private bool currentSortDescending;


		protected override void OnParametersSet()
		{
			LoadData();
		}


		private void LoadData()
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
				items = currentSortDescending
					? items.OrderByDescending(currentSort)
					: items.OrderBy(currentSort);

			// Count
			TotalCount = items.Count();

			// Paginate
			newItems = items
				.Skip((Page - 1) * PageSize)
				.Take(PageSize);


			StateHasChanged();
		}

		private void SetPage(int newPage)
		{
			if (newPage < 1 || newPage > TotalPages)
				return;

			Page = newPage;
			LoadData();
		}

		private void OnSort((Expression<Func<TItem, object?>>, bool) obj)
		{
			var (currentSortExpression, currentSortDescending) = obj;

			this.currentSortDescending = currentSortDescending;

			currentSort = currentSortExpression.Compile();

			SetPage(1);
		}

		private async Task OnFilter(IEnumerable<Expression<Func<TItem, bool>>?> expressions)
		{
			currentFilters = expressions;
			SetPage(1);
		}
	}
}