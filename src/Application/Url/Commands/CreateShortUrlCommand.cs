using FluentValidation;
using HashidsNet;
using MediatR;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record CreateShortUrlCommand : IRequest<string>
{
    public string Url { get; init; } = default!;
}

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        _ = RuleFor(v => v.Url)
          .NotEmpty()
          .WithMessage("Url is required.");
    }
}

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public CreateShortUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var originalUrl = request.Url;

        if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
        {
            throw new ArgumentException($"Requested URL '{originalUrl}' is not valid.");
        }

        var urlInDatabase = _context.Urls.FirstOrDefault(x => x.OriginalUrl == originalUrl);
        if (urlInDatabase == null)
        {
            urlInDatabase = new Domain.Entities.Url { OriginalUrl = originalUrl };
            _ = _context.Urls.Add(urlInDatabase);
            _ = await _context.SaveChangesAsync(cancellationToken);
        }

        var urlRedirectionId = _hashids.EncodeLong(urlInDatabase.Id);

        var shortUrl = $"{KnownPaths.ApplicationUrlHttps}/u/{urlRedirectionId}";

        return shortUrl;
    }
}
