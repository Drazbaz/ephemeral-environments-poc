namespace Api.Application.Models
{
    public class PagedResult<T>
    {
        public required T[] Items { get; init; } = [];
        public required bool HasMore { get; init; }
    }
}
