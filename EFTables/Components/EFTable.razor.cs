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


		private IEnumerable<TItem> items = [];


		protected override async Task OnParametersSetAsync()
		{
			var query = Query;

			if (ReadOnly)
				query = query.AsNoTracking();

			items = query.AsEnumerable();
		}
	}
}