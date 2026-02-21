using Microsoft.AspNetCore.Components;


namespace Tables.Components
{
	public partial class Column<TItem>
	{
		[CascadingParameter]
		public Table<TItem> Table { get; set; } = default!;

		[Parameter]
		public string Title { get; set; } = "";

		[Parameter]
		public Func<TItem, object?>? Field { get; set; }

		[Parameter]
		public RenderFragment<TItem>? ChildContent { get; set; }


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
				builder.AddContent(1, Field(item)?.ToString());
			}
		};
	}
}