namespace NetCoreBE.Api;

public class MapperProfileApi : Profile
{
    public MapperProfileApi()
    {
        CreateMap<TicketDto, Ticket>()
            .ReverseMap();
    }
}
