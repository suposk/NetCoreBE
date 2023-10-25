namespace NetCoreBE.Api.Application;

public class MapperProfileApi : Profile
{
    public MapperProfileApi()
    {
        CreateMap<TicketDto, Ticket>()
            .ReverseMap();

        CreateMap<RequestDto, Request>()
            .ReverseMap();

        CreateMap<RequestHistoryDto, RequestHistory>()
            .ReverseMap();
    }
}
