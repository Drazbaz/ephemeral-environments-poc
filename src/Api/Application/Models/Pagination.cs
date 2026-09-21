using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Application.Models
{
    public abstract class Pagination
    {
        public int Offset { get; init; } = 0;
        public int Limit { get; init; } = 25;
        [BindNever]
        public int QueryLimit => Limit + 1;
    }
}
