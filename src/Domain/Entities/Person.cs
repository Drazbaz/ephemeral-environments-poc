namespace Domain.Entities
{
    public sealed class Person
    {
        public required Guid Id { get; init; } = Guid.NewGuid();
        public required string Name { get; set; }
    }
}