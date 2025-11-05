namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

internal sealed class PagingParams
{
    private PagingParams(int pageSize, int skipValue)
    {
        PageSize = pageSize;
        SkipValue = skipValue;
    }

    public int PageSize { get;  }
    
    public int SkipValue { get; }
    
    public static PagingParams CreateValid(IPagingParameters pagingParams)
    { 
        var pageSize = pagingParams.ItemsPerPage <= 0
            ? 10
            : (pagingParams.ItemsPerPage > 50 ? 50 : pagingParams.ItemsPerPage);
        var pageIndex = pagingParams.CurrentPage <= 0 ? 1 : pagingParams.CurrentPage;
        var skip = (pageIndex - 1) * pageSize;
        
        return new PagingParams(pageSize, skip);
    }
}
