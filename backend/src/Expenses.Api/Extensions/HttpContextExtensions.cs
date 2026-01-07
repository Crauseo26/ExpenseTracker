namespace Expenses.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is Guid userId)
        {
            return userId;
        }

        throw new InvalidOperationException("UserId not found in request context");
    }

    public static Guid? TryGetUserId(this HttpContext context)
    {
        if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is Guid userId)
        {
            return userId;
        }

        return null;
    }
}
