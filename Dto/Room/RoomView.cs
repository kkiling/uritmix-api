namespace Dto.Room;

public record RoomView
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}