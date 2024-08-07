namespace NetCoreBE.Application.Tickets;

public class SearchTicketQuery : IRequest<ResultCom<PagedListDto<TicketDto>>>
{
    public TicketSearchParameters? SearchParameters { get; set; }
}

public class SearchQueryHandler : IRequestHandler<SearchTicketQuery, ResultCom<PagedListDto<TicketDto>>>
{    
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IMapper _mapper;
    private readonly IApiDbContext _apiDbContext;
    private readonly ILogger<SearchQueryHandler> _logger;    

    public SearchQueryHandler(
        ITicketRepository ticketRepository,
        IPropertyMappingService propertyMappingService,
        IMapper mapper,
        IApiDbContext apiDbContext,
        ILogger<SearchQueryHandler> logger
        )
    {        
        _propertyMappingService = propertyMappingService;
        _mapper = mapper;
        _apiDbContext = apiDbContext;
        _logger = logger;
    }

    public async Task<ResultCom<PagedListDto<TicketDto>>> Handle(SearchTicketQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var searchParameters = request.SearchParameters;            
            _logger.LogInformation("param {@searchParameters}", searchParameters);
            if (searchParameters is null)                
                return ResultCom<PagedListDto<TicketDto>>.Failure($"searchParameters is null", HttpStatusCode.BadRequest);
            if (!_propertyMappingService.ValidMappingExistsFor<TicketDto, Ticket>(searchParameters.OrderBy))                
                return ResultCom<PagedListDto<TicketDto>>.Failure($"Invalid OrderBy field: {searchParameters.OrderBy}", HttpStatusCode.BadRequest);
                        
            var collection = _apiDbContext.Tickets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchParameters.TicketType))                            
                collection = collection.Where(a => a.TicketType.Contains(searchParameters.TicketType.Trim()));
            
            if (!string.IsNullOrWhiteSpace(searchParameters.Status))                           
                collection = collection.Where(a => a.Status.Contains(searchParameters.Status.Trim()));
            
            if (!string.IsNullOrWhiteSpace(searchParameters.Note))                            
                collection = collection.Where(a => a.Note.Contains(searchParameters.Note.Trim()));
            
            if (!string.IsNullOrWhiteSpace(searchParameters.SearchTerm))
            {
                var searchQuery = searchParameters.SearchTerm.Trim();
                collection = collection.Where(a => (a.Status != null && a.Status.Contains(searchQuery))
                    || (a.Note != null && a.Note.Contains(searchQuery))
                    //|| (a.TicketType != null && a.TicketType.Contains(searchQuery))
                    );
            }

            if (!string.IsNullOrWhiteSpace(searchParameters.OrderBy))
            {
                // get property mapping dictionary
                var propertyMappingDictionary = _propertyMappingService.GetPropertyMapping<TicketDto, Ticket>();
                collection = collection.ApplySort(searchParameters.OrderBy, propertyMappingDictionary);
            }
            var repo = await PagedList<Ticket>.CreateAsync(collection,
                searchParameters.CurrentPage,
                searchParameters.PageSize);

            var dtos = _mapper.Map<List<TicketDto>>(repo.Results);
            var res = new PagedListDto<TicketDto>(dtos, repo.TotalCount, repo.CurrentPage, repo.PageSize);
            return ResultCom<PagedListDto<TicketDto>>.Success(res);            
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error");
            return ResultCom<PagedListDto<TicketDto>>.Failure($"{ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
}
