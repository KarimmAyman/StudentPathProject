using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        // Success Response
        public static ApiResponse SuccessResponse(string message, int statusCode = 200, DriverReadDTO createdDriver = null)
        {
            return new ApiResponse
            {
                Success = true,
                StatusCode = statusCode,
                Message = message
            };
        }

        // Error Response
        public static ApiResponse ErrorResponse(string message, int statusCode = 400)
        {
            return new ApiResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }

    // Generic ApiResponse for specific types
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; } = default!;

        // Success Response
        public static new ApiResponse<T> SuccessResponse(string message, int statusCode = 200, T data = default!)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        // Error Response
        public static new ApiResponse<T> ErrorResponse(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
