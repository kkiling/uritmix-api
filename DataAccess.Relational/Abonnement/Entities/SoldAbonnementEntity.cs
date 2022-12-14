using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Relational.Auth.Entities;
using DataAccess.Relational.Lesson.Entities;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Abonnement.Entities;

[Table("sold_abonnement")]
public class SoldAbonnementEntity : IHasId
{
    [Key] [Column("id")] public long Id { get; set; }
    [Column("person_id")] public long PersonId { get; init; }
    public PersonEntity Person { get; init; } = null!;
    [Column("active")] public bool Active { get; set; }
    [Column("date_sale")] public long DateSale { get; set; }
    [Column("date_expiration")] public long DateExpiration { get; set; }
    [Column("price_sold")] public float PriceSold { get; set; }
    [Column("discount")] public byte Discount { get; set; }
    [Column("visit_counter")] public int VisitCounter { get; set; }

    // Слепок состояния абонимента при продаже
    [Column("name")] public string Name { get; set; } = null!;
    [Column("validity")] public byte Validity { get; set; }
    [Column("number_of_visits")] public byte MaxNumberOfVisits { get; set; }
    [Column("base_price")] public float BasePrice { get; set; }
    public IEnumerable<LessonEntity> Lessons { get; init; } = null!;
}