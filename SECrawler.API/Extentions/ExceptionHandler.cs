using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace SECrawler.API.Extensions;

public class ExceptionHandler
{
}

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;
    public static readonly List<string> RequestHeaders = new List<string>();
    public static readonly List<string> ResponseHeaders = new List<string>();

    public CustomExceptionHandlerMiddleware(RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        string message = null;
        string customMessage = null;
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        ResultStatusCode apiStatusCode = ResultStatusCode.ServerError;
        bool isSuccess = false;

        try
        {
            await _next(context);
            if (context.Response != null && !context.Response.HasStarted && context.Response.StatusCode == 401)
                throw new UnauthorizedAccessException("Unauthorized");
        }
        catch (AppException exception)
        {
            httpStatusCode = exception.HttpStatusCode;
            apiStatusCode = exception.ApiStatusCode;
            isSuccess = exception.IsSuccess;
            customMessage = exception.CustomMessage;
            Log(exception);
            await WriteToResponseAsync();
        }
        catch (SecurityTokenExpiredException exception)
        {
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (UnauthorizedAccessException exception)
        {
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (System.Exception exception)
        {
            Log(exception);
            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException(
                    "The response has already started, the http status code middleware will not be executed.");

            var result = new ApiResult(isSuccess, apiStatusCode, (int)httpStatusCode, customMessage);

            var json = JsonConvert.SerializeObject(result);

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

        void SetUnAuthorizeResponse(System.Exception exception)
        {
            httpStatusCode = HttpStatusCode.Unauthorized;
            apiStatusCode = ResultStatusCode.UnAuthorized;
            Log(exception);
        }
    }

    private void Log(System.Exception exception)
    {
        var dic = new Dictionary<string, string>
        {
            ["Exception"] = exception.Message,
            ["StackTrace"] = exception.StackTrace,
        };
        if (exception.InnerException != null)
        {
            dic.Add("InnerException.Exception", exception.InnerException.Message);
            dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
        }

        if (exception is AppException appException && appException.AdditionalData != null)
            dic.Add("AdditionalData", JsonConvert.SerializeObject(appException.AdditionalData));

        if (exception is SecurityTokenExpiredException tokenException)
            dic.Add("Expires", tokenException.Expires.ToString());

        var message = JsonConvert.SerializeObject(dic);

        _logger.LogError(exception, message);
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        var body = request.Body;

        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        await request.Body.ReadAsync(buffer, 0, buffer.Length);

        var bodyAsText = Encoding.UTF8.GetString(buffer);

        request.Body = body;

        return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
               response.Body.Seek(0, SeekOrigin.Begin);

               string text = await new StreamReader(response.Body).ReadToEndAsync();

               response.Body.Seek(0, SeekOrigin.Begin);

               return $"{response.StatusCode}: {text}";
    }
}

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public ResultStatusCode StatusCode { get; set; }
    public int? HttpStatusCode { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Message { get; set; }

    public ApiResult(bool isSuccess, ResultStatusCode statusCode, int? httpStatusCode, string message = null)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        HttpStatusCode = httpStatusCode;
        Message = message ?? statusCode.ToDisplay();
    }

    #region Implicit Operators

    public static implicit operator ApiResult(OkResult result)
    {
        return new ApiResult(true, ResultStatusCode.Success, result.StatusCode);
    }

    public static implicit operator ApiResult(BadRequestResult result)
    {
        return new ApiResult(false, ResultStatusCode.BadRequest, result.StatusCode);
    }

    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }

        return new ApiResult(false, ResultStatusCode.BadRequest, result.StatusCode, message);
    }

    public static implicit operator ApiResult(ContentResult result)
    {
        return new ApiResult(true, ResultStatusCode.Success, result.StatusCode, result.Content);
    }

    public static implicit operator ApiResult(OkObjectResult result)
    {
        return new ApiResult(true, ResultStatusCode.Success, result.StatusCode);
    }

    public static implicit operator ApiResult(NoContentResult result)
    {
        return new ApiResult(true, ResultStatusCode.NotFound, result.StatusCode);
    }

    public static implicit operator ApiResult(NotFoundResult result)
    {
        return new ApiResult(false, ResultStatusCode.NotFound, result.StatusCode);
    }



    #endregion
}

public class AppException : System.Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public ResultStatusCode ApiStatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string CustomMessage { get; set; }

    public object AdditionalData { get; set; }

    public AppException()
        : this(ResultStatusCode.ServerError)
    {
    }

    public AppException(ResultStatusCode statusCode)
        : this(statusCode, HttpStatusCode.InternalServerError)
    {
    }

    public AppException(string customMessage)
        : this(customMessage, HttpStatusCode.InternalServerError)
    {
    }
    public AppException(ResultStatusCode statusCode, string message, bool isSuccess = false)
        : this(statusCode, message, HttpStatusCode.InternalServerError, isSuccess)
    {
    }

    public AppException(string message, object additionalData, bool isSuccess = false)
        : this(ResultStatusCode.ServerError, message, additionalData, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, object additionalData, bool isSuccess = false)
        : this(statusCode, null, additionalData, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, HttpStatusCode httpStatusCode, bool isSuccess = false)
        : this(statusCode, null, httpStatusCode, null, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string message, object additionalData, bool isSuccess = false)
        : this(statusCode, message, HttpStatusCode.InternalServerError, additionalData, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string customMessage, HttpStatusCode httpStatusCode,
        bool isSuccess = false)
        : this(statusCode, customMessage, httpStatusCode, null, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string message, HttpStatusCode httpStatusCode,
        object additionalData, bool isSuccess = false)
        : this(statusCode, message, httpStatusCode, null, additionalData, isSuccess)
    {
    }

    public AppException(string message, System.Exception exception, bool isSuccess = false)
        : this(ResultStatusCode.ServerError, message, exception, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, System.Exception exception, bool isSuccess = false)
        : this(statusCode, "", exception, null, isSuccess)
    {
    }

    public AppException(System.Exception exception)
        : this(ResultStatusCode.ServerError, exception, HttpStatusCode.InternalServerError)
    {
    }

    public AppException(string logMessage, ResultStatusCode ResultStatusCode)
        : this(ResultStatusCode, new System.Exception(logMessage), HttpStatusCode.InternalServerError)
    {
    }
    
    public AppException(ResultStatusCode statusCode, System.Exception exception, HttpStatusCode httpStatusCode,
        bool isSuccess = false)
        : this(statusCode, null, httpStatusCode, exception, isSuccess)
    {
    }

    public AppException(string message, System.Exception exception, object additionalData, bool isSuccess = false)
        : this(ResultStatusCode.ServerError, message, exception, additionalData, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string message, System.Exception exception, bool isSuccess = false)
        : this(statusCode, message, HttpStatusCode.InternalServerError, exception, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string message, System.Exception exception, object additionalData,
        bool isSuccess = false)
        : this(statusCode, message, HttpStatusCode.InternalServerError, exception, additionalData, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string customMessage, HttpStatusCode httpStatusCode,
        System.Exception exception, bool isSuccess = false)
        : this(statusCode, customMessage, httpStatusCode, exception, null, isSuccess)
    {
    }

    public AppException(ResultStatusCode statusCode, string customMessage, HttpStatusCode httpStatusCode,
        System.Exception exception, object additionalData, bool isSuccess = false)
        : base(customMessage, exception)
    {
        ApiStatusCode = statusCode;
        HttpStatusCode = httpStatusCode;
        AdditionalData = additionalData;
        IsSuccess = isSuccess;
        CustomMessage = customMessage;
    }
}

public static class EnumExtensions
{
    public static IEnumerable<T> GetEnumValues<T>(this T input) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new NotSupportedException();

        return System.Enum.GetValues(input.GetType()).Cast<T>();
    }

    public static IEnumerable<T> GetEnumFlags<T>(this T input) where T : struct
    {
        if (!typeof(T).IsEnum)
            throw new NotSupportedException();

        foreach (var value in System.Enum.GetValues(input.GetType()))
            if ((input as System.Enum).HasFlag(value as System.Enum))
                yield return (T)value;
    }

    public static string ToDisplay(this System.Enum value, DisplayProperty property = DisplayProperty.Name)
    {

        var attribute = value.GetType().GetField(value.ToString())
            .GetCustomAttributes(typeof(DisplayAttribute),false).FirstOrDefault();

        if (attribute == null)
            return value.ToString();

        var propValue = attribute.GetType().GetProperty(property.ToString()).GetValue(attribute, null);
        return propValue.ToString();
    }

    public static Dictionary<int, string> ToDictionary(this System.Enum value)
    {
        return System.Enum.GetValues(value.GetType()).Cast<System.Enum>().ToDictionary(p => Convert.ToInt32(p), q => ToDisplay(q));
    }
}
public enum DisplayProperty
{
    // Description,
    // GroupName,
    Name,
    // Prompt,
    // ShortName,
    // Order
}

public enum ResultStatusCode : long
{
    [Display(Name = "Success")]
    Success = 0,

    [Display(Name = "Server Error")]
    ServerError = 1,

    [Display(Name = "Bad Request")]
    BadRequest = 2,

    [Display(Name = "NotFound")]
    NotFound = 3,

    [Display(Name = "ListEmpty")] 
    ListEmpty = 4,

    [Display(Name = "LogicError")]
    LogicError = 5,
    [Display(Name = "UnAuthorized")]
    UnAuthorized = 6,
}