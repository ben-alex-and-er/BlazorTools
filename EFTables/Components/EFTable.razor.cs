using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;


namespace EFTables.Components
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class EFTable<TItem>
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


		protected override async Task OnParametersSetAsync()
		{
			await LoadData();
		}


		private async Task LoadData()
		{
			if (ReadOnly)
				newQuery = Query.AsNoTracking();

			// Pagination
			TotalCount = await newQuery.CountAsync();

			newQuery = newQuery.Skip((Page - 1) * PageSize).Take(PageSize);
		}

		private async Task SetPage(int newPage)
		{
			if (newPage < 1 || newPage > TotalPages)
				return;

			Page = newPage;
			await LoadData();
			StateHasChanged();
		}
	}
}