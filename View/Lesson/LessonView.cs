using System.ComponentModel;
using View.Person;

namespace View.Lesson;

[DisplayName("Lesson")]
public record LessonView
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public PersonView Trainer { get; init; } = null!;
    public int DurationMinute { get; init; }
    public float BasePrice { get; init; }
}