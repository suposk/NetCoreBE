namespace NetCoreBE.Api.Application.Features.Tickets;

public class SearchQuery : IRequest<PagedListDto<TicketDto>?>
{
    public TicketSearchParameters? SearchParameters { get; set; }
}

public class SearchQueryHandler : IRequestHandler<SearchQuery, PagedListDto<TicketDto>?>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchQueryHandler> _logger;

    public SearchQueryHandler(
        ITicketRepository ticketRepository,
        IPropertyMappingService propertyMappingService,
        IMapper mapper,
        ILogger<SearchQueryHandler> logger
        )
    {
        _ticketRepository = ticketRepository;
        _propertyMappingService = propertyMappingService;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<PagedListDto<TicketDto>?> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var searchParameters = request.SearchParameters;
        if (searchParameters is null)
            throw new ArgumentNullException(nameof(searchParameters));

        if (!_propertyMappingService.ValidMappingExistsFor<TicketDto, Ticket>(searchParameters.OrderBy))
            throw new BadRequestException($"Invalid OrderBy field: {searchParameters.OrderBy}");

        var repo = await _ticketRepository.Search(searchParameters);        
        var dtos = _mapper.Map<List<TicketDto>>(repo.Results);
        var res = new PagedListDto<TicketDto>(dtos, repo.TotalCount, repo.CurrentPage, repo.PageSize);
        return res;
    }
}
