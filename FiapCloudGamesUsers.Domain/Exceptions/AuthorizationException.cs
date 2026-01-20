using Microsoft.Extensions.Logging;
using System.Net;

namespace FiapCloudGamesUsers.Domain.Exceptions;

public class AuthorizationException : BaseException
{
    public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Forbidden;
    public override LogLevel LogLevel { get; set; } = LogLevel.Warning;

    public AuthorizationException() : base("Oops! It looks like you don’t have permission to do that.")
    {
    }
}
