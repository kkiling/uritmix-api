using System.ComponentModel;
using DataAccess.Auth;
using View.Auth;
using FluentValidation;
using Helpers.Core;
using Mapping.Enum.Person;
using MediatR;
using Microsoft.Extensions.Localization;
using Model.Auth;
using Service.Security.UserJwt;

namespace Command.Auth;

public class Refresh
{
    [DisplayName("Refresh")]
    public record RefreshForm
    {
        public string Token { get; init; } = null!;
    }

    [DisplayName("RefreshCommand")]
    public record Command(RefreshForm Refresh) : IRequest<ResultResponse<LoggedPersonView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Refresh).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Refresh.Token).NotNull().NotEmpty();
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<LoggedPersonView>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Handler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator,
            IStringLocalizer<Handler> localizer)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _localizer = localizer;
        }

        public async Task<ResultResponse<LoggedPersonView>> Handle(Command message, CancellationToken ct)
        {
            var res = _jwtTokenGenerator.ResolveRefreshToken(message.Refresh.Token);
            if (res.Type != RefreshTokenValidateType.Valid)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Refresh token not valid"]);

            var token = await _refreshTokenRepository.Get(res.TokenId);
            if (token == null || token.Person.Auth == null)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Refresh token not valid"]);
            if (token.IsRevoked)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Refresh token has been revoked"]);

            var auth = token.Person.Auth;
            if (auth.Status == AuthStatus.Blocked)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["User is blocked"]);

            await _refreshTokenRepository.CreateOrUpdate(new RefreshTokenModel
            {
                PersonId = token.PersonId,
                IsRevoked = false
            });

            var result = new LoggedPersonView
            {
                FirstName = token.Person.FirstName,
                LastName = token.Person.LastName,
                Role = token.Person.Auth.Role.ToView(),
                Email = token.Person.Auth.Email,
                AccessToken = _jwtTokenGenerator.CreateAccessToken(token.Person.Id, token.Person.Auth.Email,
                    token.Person.Auth.Role),
                RefreshToken = _jwtTokenGenerator.CreateRefreshToken(token.Person.Auth.Email, token.Id)
            };

            return new ResultResponse<LoggedPersonView>(result);
        }
    }
}