using Ezenity.Application.Abstractions.Security;
using Ezenity.Core.Entities.Accounts;
using Microsoft.AspNetCore.Http;

namespace Ezenity.API.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public int UserId =>
        _http.HttpContext?.Items["Account"] is Account a ? a.Id : 0;

    public string? Email =>
        _http.HttpContext?.Items["Account"] is Account a ? a.Email : null;

    public bool IsAdmin =>
        _http.HttpContext?.Items["Account"] is Account a && a.Role?.Name == "Admin";
}
