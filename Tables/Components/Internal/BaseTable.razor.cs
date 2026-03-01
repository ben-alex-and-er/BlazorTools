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
		public EventCallback<IEnumerable<Expression<Func<TItem, bool>>?>> OnFilter { get; set; }


		public string? SortColumn { get; private set; }
		public bool SortDescending { get; private set; }


		private readonly Dictionary<Column<TItem>, Expression<Func<TItem, bool>>?> columnExpressions = [];


		internal void RegisterColumn(Column<TItem> column)
		{
			columnExpressions.Add(column, null);

			StateHasChanged();
		}


		internal async Task NotifySortChanged(Expression<Func<TItem, object?>> field, bool descending, string title)
		{
			await OnSort.InvokeAsync((field, descending));

			SortDescending = descending;
			SortColumn = title;

			await InvokeAsync(StateHasChanged);
		}

		internal async Task NotifyFilterChanged(Column<TItem> column, Expression<Func<TItem, bool>> expression)
		{
			if (!columnExpressions.TryAdd(column, expression))
			{
				columnExpressions[column] = expression;
			}

			var expressions = columnExpressions.Values;

			await OnFilter.InvokeAsync(expressions);

			await InvokeAsync(StateHasChanged);
		}
	}
}