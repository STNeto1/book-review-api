using HotChocolate.AspNetCore;
using HotChocolate.Execution;

namespace book_review_api.Patch;

public class IntrospectionInspector : DefaultHttpRequestInterceptor
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public IntrospectionInspector(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken)
    {
        if (_webHostEnvironment.IsDevelopment())
        {
            requestBuilder.AllowIntrospection();
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}