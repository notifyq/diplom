
using Microsoft.AspNetCore.Mvc;

namespace api_CodeFlow.Extentions
{
/// <summary>
/// Потоковая передача файла с поддержкой дозагрузки.
/// 
/// К примеру пользователь скачал 50% контента и остановил загрузку, а позже возобновил.
/// </summary>
    public class RangeFileResult : FileResult
    {
        private readonly string _fileName;

        public RangeFileResult(string fileName, string contentType)
            : base(contentType)
        {
            _fileName = fileName;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var rangeHeader = context.HttpContext.Request.Headers["Range"].ToString();
            var fileInfo = new FileInfo(_fileName);
            long start = 0, end = fileInfo.Length - 1;

            if (!string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes="))
            {
                var range = rangeHeader.Substring(6).Split('-');
                if (range.Length == 2)
                {
                    start = long.Parse(range[0]);
                    end = string.IsNullOrEmpty(range[1]) ? end : long.Parse(range[1]);
                }
            }

            context.HttpContext.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileInfo.Length}");
            context.HttpContext.Response.Headers.Add("Content-Length", (end - start + 1).ToString());

            using (var stream = new FileStream(_fileName, FileMode.Open))
            {
                stream.Seek(start, SeekOrigin.Begin);
                var buffer = new byte[end - start + 1];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                await context.HttpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
