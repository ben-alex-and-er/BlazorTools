using Microsoft.AspNetCore.Components;

namespace Tables.Components.Internal
{
	public partial class TablePagination
	{
		[Parameter, EditorRequired]
		public int Page { get; set; }

		[Parameter, EditorRequired]
		public int TotalPages { get; set; }

		[Parameter, EditorRequired]
		public int TotalCount { get; set; }


		[Parameter, EditorRequired]
		public EventCallback OnPrev { get; set; }

		[Parameter, EditorRequired]
		public EventCallback OnNext { get; set; }


		private Task Prev()
			=> OnPrev.InvokeAsync();

		private Task Next()
			=> OnNext.InvokeAsync();
	}
}