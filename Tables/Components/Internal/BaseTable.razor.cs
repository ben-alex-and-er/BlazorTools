using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;


namespace Tables.Components.Internal
{
	[CascadingTypeParameter(nameof(TItem))]
	public partial class BaseTable<TItem>
	{
		[Parameter]
		public IEnumerable<TItem> Items { get; set; } = [];

		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		[EditorRequired]
		[Parameter]
		public EventCallback<(Expression<Func<TItem, object?>>, bool)> OnSort { get; set; }

		[EditorRequired]
		[Parameter]
		public EventCallback<Expression<Func<TItem, bool>>> OnFilter { get; set; }


		private readonly List<Column<TItem>> columns = [];

		public string? SortColumn { get; private set; }
		public bool SortDescending { get; private set; }

		internal event Func<string, bool, Task>? OnSortChanged;


		internal void RegisterColumn(Column<TItem> column)
		{
			columns.Add(column);

			StateHasChanged();
		}


		internal async Task NotifySortChanged(Expression<Func<TItem, object?>> field, bool descending, string title)
		{
			await OnSort.InvokeAsync((field, descending));

			SortDescending = descending;
			SortColumn = title;

			StateHasChanged();
		}
	}
}