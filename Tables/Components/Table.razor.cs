using Microsoft.AspNetCore.Components;


namespace Tables.Components
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class Table<TItem>
	{
		[Parameter]
		public IQueryable<TItem>? Query { get; set; }

		[Parameter]
		public IEnumerable<TItem> Items { get; set; } = [];

		[Parameter]
		public RenderFragment? ChildContent { get; set; }


		private List<Column<TItem>> columns = new();


		protected override async Task OnParametersSetAsync()
		{
			if (Query is not null)
			{
				Items = Query.AsEnumerable();
			}
		}

		internal void RegisterColumn(Column<TItem> column)
		{
			columns.Add(column);

			StateHasChanged();
		}
	}
}