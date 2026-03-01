using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Tables.Components.Internal
{
	public abstract class TableBase<TItem> : ComponentBase
	{
		[Parameter]
		public int Page { get; set; } = 1;

		[Parameter]
		public int PageSize { get; set; } = int.MaxValue;

		public int TotalCount { get; protected set; }

		public int TotalPages => TotalCount == 0
			? 1
			: (int)Math.Ceiling((double)TotalCount / PageSize);


		protected IEnumerable<Expression<Func<TItem, bool>>?> currentFilters = [];
		protected Expression<Func<TItem, object?>>? currentSort;
		protected bool currentSortDescending;


		protected override async Task OnParametersSetAsync()
		{
			await LoadData();
		}


		protected abstract Task LoadData();

		protected async Task SetPage(int newPage)
		{
			if (newPage < 1 || newPage > TotalPages)
				return;

			Page = newPage;
			await LoadData();
		}

		protected async Task OnSort((Expression<Func<TItem, object?>>, bool) obj)
		{
			(currentSort, currentSortDescending) = obj;
			await SetPage(1);
		}

		protected async Task OnFilter(IEnumerable<Expression<Func<TItem, bool>>?> filters)
		{
			currentFilters = filters;
			await SetPage(1);
		}
	}
}
