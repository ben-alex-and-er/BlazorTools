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
		public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);


		private IQueryable<TItem> newQuery;
		private IQueryable<TItem>? sortedQuery;


		protected override async Task OnParametersSetAsync()
		{
			await LoadData(Query);
		}


		private async Task LoadData(IQueryable<TItem> query)
		{
			newQuery = ReadOnly
				? query.AsNoTracking()
				: query;

			// Pagination
			TotalCount = await newQuery.CountAsync();

			newQuery = newQuery.Skip((Page - 1) * PageSize).Take(PageSize);

			await InvokeAsync(StateHasChanged);
		}

		private async Task SetPage(int newPage)
		{
			if (newPage < 1 || newPage > TotalPages)
				return;

			Page = newPage;
			await LoadData(sortedQuery ?? Query);
		}

		private async Task OnSort((Expression<Func<TItem, object?>>, bool) obj)
		{
			var (field, descending) = obj;

			sortedQuery = descending
				? Query.OrderByDescending(field)
				: Query.OrderBy(field);

			await SetPage(1);
		}
	}
}