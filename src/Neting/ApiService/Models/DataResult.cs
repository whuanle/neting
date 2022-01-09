using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Neting.ApiService.Models
{
    public class DataResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }


    public class DataResult<T> : DataResult
    {
        public T Data { get; set; }
    }

    public class DataResults<T> : DataResult
    {
        public IEnumerable<T> Data { get; set; }
    }

    public class Pageing<T> : DataResult
    {
        public Page Data { get; set; }

        public class Page
        {
            public int TotalCount { get; set; }
            public IEnumerable<T> Data { get; set; }
        }
    }

    public static class DataResultExtensions
    {
        public static IActionResult ToJsonResult(this DataResult result)
        {
            return new JsonResult(result);
        }

        public static IActionResult ToJsonResult<T>(this DataResult<T> result)
        {
            return new JsonResult(result);
        }
    }
}
