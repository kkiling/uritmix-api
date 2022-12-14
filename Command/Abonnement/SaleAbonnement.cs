using System.ComponentModel;
using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Auth;
using View.Abonnement;
using FluentValidation;
using Helpers.Core;
using Mapping.Enum.Abonnement;
using MediatR;
using Microsoft.Extensions.Localization;
using Model.Abonnement;

namespace Command.Abonnement;

public class SaleAbonnement
{
    [DisplayName("SaleAbonnement")]
    public record SaleAbonnementForm
    {
        public long PersonId { get; init; }
        public long AbonnementId { get; init; }
        public DiscountView Discount { get; init; }
    }

    [DisplayName("CreateAbonnementCommand")]
    public record Command(SaleAbonnementForm Create) : IRequest<ResultResponse<SoldAbonnementView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Create).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Create.Discount)
                    .NotNull();
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<SoldAbonnementView>>
    {
        private readonly IAbonnementRepository _abonnementRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IMapper _mapper;
        private readonly IPersonRepository _personRepository;
        private readonly ISoldAbonnementRepository _soldAbonnementRepository;

        public Handler(IMapper mapper, IStringLocalizer<Handler> localizer,
            IAbonnementRepository abonnementRepository,
            IPersonRepository personRepository,
            ISoldAbonnementRepository soldAbonnementRepository)
        {
            _mapper = mapper;
            _localizer = localizer;
            _abonnementRepository = abonnementRepository;
            _personRepository = personRepository;
            _soldAbonnementRepository = soldAbonnementRepository;
        }

        DateTime ToDateDateExpiration(AbonnementValidity validity)
        {
            var dateExpiration = DateTime.Now;
            switch (validity)
            {
                case AbonnementValidity.OneDay:
                    return dateExpiration.AddDays(1);
                case AbonnementValidity.OneMonth:
                    return dateExpiration.AddMonths(1);
                case AbonnementValidity.ThreeMonths:
                    return dateExpiration.AddMonths(3);
                case AbonnementValidity.HalfYear:
                    return dateExpiration.AddMonths(6);
                case AbonnementValidity.Year:
                    return dateExpiration.AddYears(1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(validity), validity, null);
            }
        }
        
        public async Task<ResultResponse<SoldAbonnementView>> Handle(Command message, CancellationToken ct)
        {
            var create = message.Create;

            var abonnement = await _abonnementRepository.Get(message.Create.AbonnementId);
            if (abonnement == null)
                return ResultResponse<SoldAbonnementView>.CreateError(_localizer["Abonnement not found"]);

            if (create.Discount > abonnement.MaxDiscount.ToView())
                return ResultResponse<SoldAbonnementView>.CreateError(_localizer["Discount greater than max discount"]);

            var person = await _personRepository.Get(message.Create.PersonId);
            if (person == null)
                return ResultResponse<SoldAbonnementView>.CreateError(_localizer["Person not found"]);

            var dateSale = DateTime.Now;
            var dateExpiration = ToDateDateExpiration(abonnement.Validity);
            var priceSold = abonnement.BasePrice * (1.0f - GetDiscountValue(create.Discount));

            var result = await _soldAbonnementRepository.Create(new SoldAbonnementModel
            {
                PersonId = create.PersonId,
                Active = true,
                DateSale = dateSale,
                DateExpiration = dateExpiration,
                PriceSold = priceSold,
                VisitCounter = 0,
                Name = abonnement.Name,
                Validity = abonnement.Validity,
                MaxNumberOfVisits = abonnement.MaxNumberOfVisits,
                BasePrice = abonnement.BasePrice,
                Discount = abonnement.MaxDiscount,
                Lessons = abonnement.Lessons
            });

            return new ResultResponse<SoldAbonnementView>(_mapper.Map<SoldAbonnementView>(result));
        }

        private static float GetDiscountValue(DiscountView discount)
        {
            switch (discount)
            {
                case DiscountView.D0:
                    return 0.0f;
                case DiscountView.D5:
                    return 0.05f;
                case DiscountView.D10:
                    return 0.1f;
                case DiscountView.D15:
                    return 0.15f;
                case DiscountView.D20:
                    return 0.2f;
                case DiscountView.D25:
                    return 0.25f;
                case DiscountView.D30:
                    return 0.3f;
                case DiscountView.D40:
                    return 0.4f;
                case DiscountView.D50:
                    return 0.5f;
                case DiscountView.D60:
                    return 0.6f;
                case DiscountView.D70:
                    return 0.7f;
                case DiscountView.D80:
                    return 0.8f;
                case DiscountView.D90:
                    return 0.9f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(discount), discount, null);
            }
        }
    }
}