using Microsoft.AspNetCore.Mvc;

namespace api_CodeFlow.Extentions
{
    public class PartialFileResult : FileResult
    {
        private readonly Stream _stream;
        private readonly long _start;
        private readonly long _end;

        public PartialFileResult(Stream stream, string contentType, long start, long end)
            : base(contentType)
        {
            _stream = stream;
            _start = start;
            _end = end;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("Content-Range", $"bytes {_start}-{_end}/{_stream.Length}");
            context.HttpContext.Response.Headers.Add("Content-Length", (_end - _start + 1).ToString());

            _stream.Seek(_start, SeekOrigin.Begin);
            var buffer = new byte[_end - _start + 1];
            await _stream.ReadAsync(buffer, 0, buffer.Length);
            await context.HttpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
