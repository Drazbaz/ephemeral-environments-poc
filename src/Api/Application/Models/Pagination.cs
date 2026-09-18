namespace Api.Application.Models
{
    public sealed class Pagination
    {
        public int Offset { get; init; } = 0;
        public int Limit { get; init; } = 25;
        public int QueryLimit => Limit + 1;
    }
}
