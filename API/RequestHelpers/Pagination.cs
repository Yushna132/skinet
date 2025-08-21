using System;

namespace API.RequestHelpers;

public class Pagination<T>(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
{
    public int PageIndex { get; set; } = pageIndex;
    public int PageSize { get; set; } = pageSize;
    //total count of possible items in the list of products
    //this need to happen after fitering but before pagination
    public int Count { get; set; } = count;
    public IReadOnlyList<T> Data { get; set; } = data;
}
