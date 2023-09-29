namespace Gleeman.JwtGenerator.Helpers;

internal class TokenExpireHelper
{
    public static DateTime SetExpireDate(ExpireType expireType, int ExpireTime)
    {
        DateTime expires = expireType switch
        {
            ExpireType.Minute => DateTime.Now.AddMinutes(ExpireTime),
            ExpireType.Hour => DateTime.Now.AddHours(ExpireTime),
            ExpireType.Day => DateTime.Now.AddDays(ExpireTime),
            ExpireType.Month => DateTime.Now.AddMonths(ExpireTime),
            _ => throw new Exception("Expire type not found!")
        };

        return expires;
    }
}
