using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankingService.Functional
{
    public class Result
    {
        public bool Success { get; set; }
        public string Error { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public Result(HttpStatusCode statusCode = HttpStatusCode.OK, string error = "", bool success = true)
        {
            Error = error;
            StatusCode = statusCode;
            Success = success;
        }

        public static Result Fail(HttpStatusCode statusCode = HttpStatusCode.OK, string error = "")
        {
            return new Result(statusCode, error, false);
        }

        public static Result<T> Fail<T>(HttpStatusCode statusCode = HttpStatusCode.OK, string error = "")
        {
            return new Result<T>(default, statusCode, error, false);
        }

        public static Result<T> Ok<T>(T value, HttpStatusCode statuscode = HttpStatusCode.OK, string error = "")
        {
            return new Result<T>(value, statuscode, error, true);
        }

        public static Result Ok()
        {
            return new Result();
        }
    }

    public class Result<T> : Result
    {
        protected internal Result(T value, HttpStatusCode statuscode = HttpStatusCode.OK, string error = "", bool success = true)
            : base(statuscode, error, success)
        {
            Value = value;
        }

        [IgnoreDataMember]
        public T Value { get; private set; }
    }


}
