using ApplicationCore.Helpers;

namespace ApplicationCore.Paging;
public class PagedList<T>
{
	private IEnumerable<T> _list;
	private int _pageNumber = 1;
	private int _pageSize = 999;

	public PagedList(IEnumerable<T> list, int pageNumber = 1, int pageSize = -1, string sortBy = "", bool desc = true)
	{

		_pageNumber = pageNumber > 0 ? pageNumber : 1;
		_pageSize = pageSize > 0 ? pageSize : 999;

		_list = list;

		SortBy = sortBy;
		Desc = desc;

	}
	public void GoToPage(int page)
	{
		if (page > 0 && page <= TotalPages) _pageNumber = page;
	}
	protected void SetList(IEnumerable<T> list) => _list = list;

	public List<T> List => _list.GetPaged(PageNumber, PageSize).ToList();

	public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
	public int TotalItems => _list.Count();
	public int PageNumber => _pageNumber;
	public int PageSize => _pageSize;


	public bool HasPreviousPage => PageNumber > 1;
	public bool HasNextPage => PageNumber < TotalPages;

	public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;
	public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;

	public string SortBy { get; }
	public bool Desc { get; }

	public IPagingHeader GetHeader()
		=> new PagingHeader(TotalItems, PageNumber, PageSize, TotalPages);
}

public class PagedList<T, V> : PagedList<T>
{
	private List<V> _viewlist = new List<V>();
	public PagedList(IEnumerable<T> list, int pageNumber = 1, int pageSize = -1, string sortBy = "", bool desc = true)
		: base(list, pageNumber, pageSize, sortBy, desc)
	{

	}

	public List<V> ViewList => _viewlist;
	public void SetViewList(List<V> viewlist, bool clearList = true)
	{
		_viewlist = viewlist;

		if (clearList) SetList(new List<T>());
	}
}

