using HashidsNet;
using Machine.Specifications;
using UrlShortenerService.Application.Common.Interfaces;
using UrlShortenerService.Application.Url.Commands;

namespace Application.Test;
public class UrlCommandTest
{
    [Subject(typeof(CreateShortUrlCommandHandler))]
    public class CreateShortUrlCommandTest
    {
        static CreateShortUrlCommandHandler CreateShortUrlCommandHandler;
        static CreateShortUrlCommand Request;
        static CancellationToken CancellationToken;
        static IApplicationDbContext Context;
        static IHashids Hashids;
        static string ShortUrl;
        static Exception Ex;

        public class WhenEmptyUrlIsRequestedToBeShortened : CreateShortUrlCommandTest
        {
            Establish the = () =>
            {
                CreateShortUrlCommandHandler = new CreateShortUrlCommandHandler(Context, Hashids);
            };

            Because of = async () =>
            {
                try
                {
                    ShortUrl = await CreateShortUrlCommandHandler.Handle(Request, CancellationToken);
                }
                catch (NullReferenceException ex)
                {
                    Ex = ex;
                }
            };

            It _shouldThrowException = () => Ex.ShouldNotBeNull();
        }
    }
}
