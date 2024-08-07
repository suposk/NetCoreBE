namespace NetCoreBE.Application.ExamplesTempleateToCopy;

/// <summary>
/// Only sample Query
/// </summary>
public class SearchQuery : IRequest<bool>
{
    public string SomePam { get; set; }
}

public class RQueryHandler : IRequestHandler<SearchQuery, bool>
{
    private readonly ILogger<RQueryHandler> _logger;

    public RQueryHandler(
        ILogger<RQueryHandler> logger
        )
    {
        _logger = logger;
    }
    public async Task<bool> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request is null || string.IsNullOrWhiteSpace(request.SomePam))
            return false;
        else
            return true;
    }
}
