namespace NetCoreBE.Application.OldTickets;

public class SearchOldTicketQuery : IRequest<PagedListDto<OldTicketDto>?>
{
    public TicketSearchParameters? SearchParameters { get; set; }
}

public class SearchQueryHandler : IRequestHandler<SearchOldTicketQuery, PagedListDto<OldTicketDto>?>
{
    private readonly IOldTicketRepository _ticketRepository;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IMapper _mapper;
    private readonly ILogger<SearchQueryHandler> _logger;

    public SearchQueryHandler(
        IOldTicketRepository ticketRepository,
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
    public async Task<PagedListDto<OldTicketDto>?> Handle(SearchOldTicketQuery request, CancellationToken cancellationToken)
    {
        var searchParameters = request.SearchParameters;
        if (searchParameters is null)
            throw new ArgumentNullException(nameof(searchParameters));

        if (!_propertyMappingService.ValidMappingExistsFor<OldTicketDto, OldTicket>(searchParameters.OrderBy))
            throw new BadRequestException($"Invalid OrderBy field: {searchParameters.OrderBy}");

        var repo = await _ticketRepository.Search(searchParameters);
        var dtos = _mapper.Map<List<OldTicketDto>>(repo.Results);
        var res = new PagedListDto<OldTicketDto>(dtos, repo.TotalCount, repo.CurrentPage, repo.PageSize);
        return res;
    }
}
