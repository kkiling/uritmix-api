using Dto.Lesson;

namespace Dto.Abonnement;

public class SoldAbonnementView
{
    public long Id { get; init; }
    
    public bool Active { get; init; }
    public DateTime DateSale { get; init; }
    public DateTime DateExpiration { get; init; }
    public float PriceSold { get; init; }
    public int VisitCounter { get; init; }
    // Слепок состояния абонимента при продаже
    public string Name { get; init; } = null!;
    public AbonnementValidityView Validity { get; init; }
    public byte NumberOfVisits { get; init; }
    public float BasePrice { get; init; }
    public DiscountView Discount { get; init; }
    // public byte DaysOfFreezing { get; init; }
    public IEnumerable<LessonView> Lessons { get; init; } = null!;
}