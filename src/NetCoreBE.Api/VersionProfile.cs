using NetCoreBE.Api.Application.Features.Ticket;

namespace NetCoreBE.Api;

public class VersionProfile : Profile
{
    public VersionProfile()
    {
        CreateMap<TicketDto, Ticket>()
            .ReverseMap();
    }
}
