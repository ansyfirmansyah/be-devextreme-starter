namespace be_devextreme_starter.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message, StatusCode = 200 };
        }

        public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
        {
            return new ApiResponse<T> { Success = true, Data = data, Message = message, StatusCode = 201 };
        }

        public static ApiResponse<object> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<object> { Success = false, Data = null, Message = message, StatusCode = 404 };
        }

        public static ApiResponse<object> BadRequest(string message = "Bad request")
        {
            return new ApiResponse<object> { Success = false, Data = null, Message = message, StatusCode = 400 };
        }

        public static ApiResponse<object> Error(string message = "An unexpected error occurred", int statusCode = 500)
        {
            return new ApiResponse<object> { Success = false, Data = null, Message = message, StatusCode = statusCode };
        }
    }
}