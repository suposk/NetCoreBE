using Microsoft.AspNetCore.Http;

namespace CommonCleanArch.Application.Helpers;

public static class OtherHelper
{
    public const string CACHECONST = "@Cache";

    public static bool IsNotNullValidIdExt(this EntityBase entity) => entity == null || string.IsNullOrWhiteSpace(entity.Id);

    public static string GetPrimaryCacheKeyExt<TEntity>(this TEntity entity) => $"{typeof(TEntity).Name}{CACHECONST}";

    public static IResult GetIResultExt<T>(this ResultCom<T> res) => res.IsSuccess ? Results.Ok(res.Value) :
        Results.Problem(res?.ErrorMessage, null, res?.Error?.StatusCode);

    public static IResult GetIResultNoContentExt(this ResultCom res) => res.IsSuccess ? Results.NoContent() :
        Results.Problem(res?.ErrorMessage, null, res?.Error?.StatusCode);
}
