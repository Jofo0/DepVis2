using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace DepVis.Core.Extensions;

public static class GeneralExtensions
{
    public static async Task<List<T>> ApplyOdata<T>(
        this ODataQueryOptions<T> odata,
        IQueryable<T> target
    ) => await ((IQueryable<T>)odata.ApplyTo(target)).ToListAsync();
}
