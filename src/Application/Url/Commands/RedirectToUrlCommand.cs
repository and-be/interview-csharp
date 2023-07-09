using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerService.Application.Common.Exceptions;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record RedirectToUrlCommand : IRequest<string>
{
    public string Id { get; init; } = default!;
}

public class RedirectToUrlCommandValidator : AbstractValidator<RedirectToUrlCommand>
{
    public RedirectToUrlCommandValidator()
    {
        _ = RuleFor(v => v.Id)
          .NotEmpty()
          .WithMessage("Id is required.");
    }
}

public class RedirectToUrlCommandHandler : IRequestHandler<RedirectToUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public RedirectToUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<string> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var urlLocationInDatabase = _hashids.DecodeLong(request.Id).FirstOrDefault();

        var url = await _context.Urls.FirstOrDefaultAsync(x => x.Id == urlLocationInDatabase, cancellationToken);
        if (url == null)
        {
            throw new NotFoundException($"Requested URL '{KnownPaths.ApplicationUrlHttps}/u/{request.Id}' could not be redirected because ID '{request.Id}' is unknown.");
        }

        return url.OriginalUrl;
    }
}
