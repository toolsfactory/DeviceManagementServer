using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.IoT;
using Amazon.IoT.Model;

namespace VTV.OpsConsole.RemoteManagement.APIServer
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class AWSExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public AWSExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ResourceNotFoundException ex)        { await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.NotFound); }
            catch (ResourceAlreadyExistsException ex)   { await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.Conflict); }
            catch (LimitExceededException ex)           { await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.Gone); }
            catch (ThrottlingException ex)              { await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.ServiceUnavailable); }
            catch (AmazonIoTException ex)               { await HandleExceptionAsync(httpContext, ex.Message, HttpStatusCode.FailedDependency); }
        }

        private static Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
        }
    }
}
