namespace NetCoreBE.Application.ExamplesTempleateToCopy;

/// <summary>
/// Only sample Query
/// </summary>
public class SearchQuery : IRequest<ResultCom<object>>
{
    public string QParam { get; set; }
}

public class RQueryHandler : IRequestHandler<SearchQuery, ResultCom<object>>
{
    private readonly ILogger<RQueryHandler> _logger;

    public RQueryHandler(
        ILogger<RQueryHandler> logger
        )
    {
        _logger = logger;
    }
    public async Task<ResultCom<object>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request is null || string.IsNullOrWhiteSpace(request.QParam))
            return ResultCom<object>.Failure($"{nameof(request.QParam)} parameter is missing");
        else
            return ResultCom<object>.Success(null);
    }
}
