namespace NetCoreBE.Application.ExamplesTempleateToCopy;

/// <summary>
/// Only sample Command
/// </summary>
public class RCommand : IRequest<ResultCom<object>>
{
    public string ComParam { get; set; }
}

public class RCommandHandler : IRequestHandler<RCommand, ResultCom<object>>
{
    private readonly ILogger<RCommandHandler> _logger;

    public RCommandHandler(
        ILogger<RCommandHandler> logger
        )
    {
        _logger = logger;
    }
    public async Task<ResultCom<object>> Handle(RCommand request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request is null || string.IsNullOrWhiteSpace(request.ComParam))
            return ResultCom<object>.Failure($"{nameof(request.ComParam)} parameter is missing");
        else
            return ResultCom<object>.Success(null);
    }
}