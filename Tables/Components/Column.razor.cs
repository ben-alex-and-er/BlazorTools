using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using Tables.Components.Internal;
using Tables.Data.Enums;
using Tables.Helpers;


namespace Tables.Components
{
	public partial class Column<TItem>
	{
		[CascadingParameter]
		public BaseTable<TItem> Table { get; set; } = default!;

		[Parameter]
		public string Title { get; set; } = string.Empty;

		[Parameter]
		public Expression<Func<TItem, object?>>? Field { get; set; }

		[Parameter]
		public RenderFragment<TItem>? ChildContent { get; set; }

		[Parameter]
		public bool Sortable { get; set; } = false;

		[Parameter]
		public bool Filterable { get; set; } = false;


		private bool IsSorted => Table.SortColumn == Title;
		private bool IsDescending => Table.SortDescending;


		private string filter = string.Empty;

		private FilterType filterType = FilterType.Contains;

		private CancellationTokenSource? cancellationTokenSource;


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

		private async Task OnSortClick()
		{
			if (!Sortable)
				return;

			var descending = IsSorted && !IsDescending;
			await Table.NotifySortChanged(Field, descending, Title);
		}

		private async Task OnInputAsync(ChangeEventArgs e)
		{
			filter = e.Value?.ToString();

			await ApplyFilterAsync();
		}

		private async Task OnFilterTypeChanged(FilterType value)
		{
			filterType = value;

			await ApplyFilterAsync();
		}

		private async Task ApplyFilterAsync()
		{
			// Cancel previous request
			cancellationTokenSource?.Cancel();
			cancellationTokenSource = new CancellationTokenSource();

			try
			{
				await OnFilter();
			}
			catch (TaskCanceledException)
			{
				// Will cancel operation when typing fast
			}
		}

		private async Task OnFilter()
		{
			if (!Filterable)
				return;

			var filterExpression = ExpressionHelper.OnFilter<Func<TItem, bool>>(Field.Body, Field.Parameters[0], filter, filterType);

			await Table.OnFilter.InvokeAsync(filterExpression);
		}
	}
}