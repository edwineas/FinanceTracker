namespace FinanceTracker.API.Responses;

public static class ApiResults
{
  public static IResult Ok<T>(T data, string title = "OK") =>
   Results.Json(new ApiResponse<T>(title, data), statusCode: 200);

  public static IResult Created() =>
    Results.StatusCode(201);

  public static IResult Created<T>(T data, string title = "Created") =>
    Results.Json(new ApiResponse<T>(title, data), statusCode: 201);

  public static IResult Deleted() =>
  Results.StatusCode(204);

  public static IResult Conflict(string title = "Conflict", object? errors = null) =>
    Results.Json(new ApiErrorResponse(title, errors), statusCode: 409);

  public static IResult Unauthorized(string title = "Unauthorized", object? errors = null) =>
    Results.Json(new ApiErrorResponse(title, errors), statusCode: 401);

  public static IResult BadRequest(string title, object? errors = null) =>
    Results.Json(new ApiErrorResponse(title, errors), statusCode: 400);
}
