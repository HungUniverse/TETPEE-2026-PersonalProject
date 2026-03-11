namespace TetPee.API.Middlewares;

public class GlobalExceptionHandleMiddleware: IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e); 
        }
    }
}