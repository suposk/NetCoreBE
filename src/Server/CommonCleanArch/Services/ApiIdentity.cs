using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace CommonCleanArch.Services;

public enum RoleEnumType { Unknown = 0, User = 1, Reader = 2, Contributor = 5, Admin = 10 }
public interface IApiIdentity
{
    bool CanModifyFunc(string? createdBy);
    Task<bool> CanModifyFuncAsync(string? createdBy);
    Task<string> GetAccessTokenForUserAsync(string scope);
    Task<string> GetCurrentUserEmail();
    string GetCurrentUserEmailClaim();
    string GetUserName();
    string GetUserNameOrIp();
    bool IsAdmin();
    bool IsAuthenticated();
    bool IsInRole(RoleEnumType roleEnumType);
}

public class ApiIdentity : IApiIdentity
{
    private readonly IHttpContextAccessor _context;
    private readonly ITokenAcquisition _tokenAcquisition;

    public ApiIdentity(IHttpContextAccessor context, ITokenAcquisition tokenAcquisition)
    {
        _context = context;
        _tokenAcquisition = tokenAcquisition;
    }

    public bool IsAuthenticated()
    {
        return _context.HttpContext != null && _context.HttpContext.User.Identity != null && _context.HttpContext.User.Identity.IsAuthenticated ? true : false;
    }

    public bool IsAdmin() => IsInRole(RoleEnumType.Admin);

    public bool IsInRole(RoleEnumType roleEnumType) => IsInRole(roleEnumType.ToString());

    public bool IsInRole(string role) => !string.IsNullOrWhiteSpace(role) && _context.HttpContext != null && _context.HttpContext.User.IsInRole(role) ? true : false;

    public string GetUserName()
    {
        if (_context?.HttpContext == null)
            return null;

        var defname = _context.HttpContext?.User?.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(defname))
            return defname;

        List<string> list = new List<string>();
        var email = _context.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.Email);
        var userData = _context.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.UserData);
        if (list.Count == 0 && email != null && !string.IsNullOrWhiteSpace(email.Value))
            return email.Value;
        if (list.Count > 0)
        {
            var shortestName = list.OrderBy(name => name.Length).FirstOrDefault();
            return shortestName;
        }
        return _context.HttpContext?.User?.Identity?.Name;
    }

    public string GetUserNameOrIp()
    {
        if (_context?.HttpContext is null)
            return null;
        var userName = GetUserName();
        if (!string.IsNullOrWhiteSpace(userName))
            return userName;

        var remoteIpAddress = _context.HttpContext.Connection.RemoteIpAddress.ToString();
#if DEBUG
        return string.Equals("::1", remoteIpAddress) ? null : $"Ip:{remoteIpAddress}";
#else
        return string.Equals("::1", remoteIpAddress) ? "localhost::1" : $"Ip:{remoteIpAddress}";
#endif
    }

    public Task<string> GetCurrentUserEmail()
    {
        return Task.FromResult(GetCurrentUserEmailClaim());
    }

    public string GetCurrentUserEmailClaim()
    {
        if (!IsAuthenticated())
            return null;
        else
        {
            var email = _context.HttpContext?.User.FindFirst(a => a.Type == ClaimTypes.Email)?.Value;
            if (!string.IsNullOrWhiteSpace(email))
                return email;

            return null;
        }
    }


    public async Task<string> GetAccessTokenForUserAsync(string scope)
    {
        if (IsAuthenticated())
        {
            try
            {
                var t = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { scope });
                return t;
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message != null && ex.InnerException.Message.Contains("No account or login hint was passed to the AcquireTokenSilent call"))
                {
                    //return null;
                }
                throw;
            }
        }
        else
        {
            //SignIn();
            return null;
        }
    }

    /// <summary>
    /// Only Admin or user who created can modify record
    /// To add custom execution, validation must override this.    
    /// </summary>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public virtual Task<bool> CanModifyFuncAsync(string? createdBy) => Task.FromResult(CanModifyFunc(createdBy));
    
    /// <summary>
    /// Only Admin or user who created can modify record
    /// To add custom execution, validation must override this.    
    /// </summary>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public virtual bool CanModifyFunc(string? createdBy)
    {
#if DEBUG
        //TODO fix this
        return true;
#endif
        if (IsAdmin())
            return true;
        else
        {
            var res = createdBy == GetUserName();
            return res;
        }
    }

}
