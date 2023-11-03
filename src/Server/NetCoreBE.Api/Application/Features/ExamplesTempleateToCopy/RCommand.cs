namespace NetCoreBE.Api.Application.Features.ExamplesTempleateToCopy;

/// <summary>
/// Only sample Command
/// </summary>
public class RCommand : IRequest<bool>
{
    public string SomePam { get; set; }
}

public class RCommandHandler : IRequestHandler<RCommand, bool>
{
    private readonly ILogger<RCommandHandler> _logger;

    public RCommandHandler(
        ILogger<RCommandHandler> logger
        )
    {
        _logger = logger;
    }
    public async Task<bool> Handle(RCommand request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request is null || string.IsNullOrWhiteSpace(request.SomePam))
            return false;
        else
            return true;
    }
}