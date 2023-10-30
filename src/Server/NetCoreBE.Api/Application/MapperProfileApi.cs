namespace NetCoreBE.Api.Application;

public class MapperProfileApi : Profile
{
    public MapperProfileApi()
    {
        //Mapper.CreateMap<Item, Product>()
        //     .ConstructUsing(x => new Product(x.Id, x.Price)) //without this line there will be an System.Argumentexception
        //     .ForMember(destination => destination.Number, x => x.MapFrom(source => source.Id))
        //     .ForMember(destination => destination.Price, x => x.MapFrom(source => source.Price))
        //https://stackoverflow.com/questions/37072286/how-to-use-mapper-map-inside-mapperconfiguration-of-automapper

        CreateMap<TicketDto, Ticket>()
            .ReverseMap();

        CreateMap<RequestDto, Request>()
            .ReverseMap();

        CreateMap<RequestHistoryDto, RequestHistory>()
            .ReverseMap();
    }
}
