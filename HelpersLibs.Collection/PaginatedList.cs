using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace HelpersLibs.Collection;
public class PaginatedList<T> {
    [JsonProperty("PageIndex")]
    public int PageIndex { get; set; }
    [JsonProperty("TotalPages")]
    public int TotalPages { get; set; }
    [JsonProperty("Source")]
    public List<T> Source { get; set; }

    public PaginatedList() {

    }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize) {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        Source = items;
    }

    [JsonProperty("HasPreviousPage")]
    public bool HasPreviousPage {
        get {
            return (PageIndex > 1);
        }
    }

    [JsonProperty("HasNextPage")]
    public bool HasNextPage {
        get {
            return (PageIndex < TotalPages);
        }
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default) {
        var count = await source.CountAsync();
        var items = await source.Skip(
            (pageIndex - 1) * pageSize)
            .Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize) {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}