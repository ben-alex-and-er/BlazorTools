using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;


namespace Tables.Components
{
	public partial class Column<TItem>
	{
		[CascadingParameter]
		public BaseTable<TItem> Table { get; set; } = default!;

		[Parameter]
		public string Title { get; set; } = "";

		[Parameter]
		public Expression<Func<TItem, object?>>? Field { get; set; }

		[Parameter]
		public RenderFragment<TItem>? ChildContent { get; set; }

		[Parameter]
		public bool Sortable { get; set; } = false;


		private bool IsSorted => Table.SortColumn == Title;
		private bool IsDescending => Table.SortDescending;


		protected override void OnInitialized()
		{
			Table.RegisterColumn(this);
		}


		public RenderFragment RenderCell(TItem item) => builder =>
		{
			if (ChildContent is not null)
			{
				builder.AddContent(0, ChildContent(item));
			}
			else if (Field is not null)
			{
				var func = Field.Compile();
				builder.AddContent(1, func(item)?.ToString());
			}
		};

		private async Task OnHeaderClick()
		{
			if (!Sortable)
				return;

			var descending = IsSorted && !IsDescending;
			await Table.NotifySortChanged(Field, descending, Title);
		}
	}
}