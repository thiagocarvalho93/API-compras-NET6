using ApiDotnet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiDotnet.Infra.Data.Repositories
{
    public static class PageBasedResponseHelper
    {
        public static async Task<TResponse> GetResponseAsync<TResponse, T>(IQueryable<T> query, PageBasedRequest request)
        where TResponse : PageBasedReponse<T>, new()
        {
            var response = new TResponse();
            var count = await query.CountAsync();
            response.TotalPages = (int)Math.Abs((double)count / request.PageSize);
            response.TotalRegisters = count;
            if (string.IsNullOrEmpty(request.OrderByProperty))
                response.Data = await query.ToListAsync();
            else
                response.Data = query.OrderByDynamic(request.OrderByProperty)
                                    .Skip((request.Page - 1) * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToList();
            return response;
        }

        // TODO Tratar para quando a propertyName não existe
        private static IEnumerable<T> OrderByDynamic<T>(this IEnumerable<T> query, string propertyName)
        {
            return query.OrderBy(x => x.GetType().GetProperty(propertyName).GetValue(x, null));
        }
    }

}