using CommonCleanArch.Application.Search;

namespace NetCoreBE.Infrastructure.Search;

public class PropertyMappingService : IPropertyMappingService
{
    private Dictionary<string, PropertyMappingValue> _TicketPropertyMapping =
      new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
      {
           { nameof(DtoBase.Id), new PropertyMappingValue(new List<string>() { nameof(EntityBase.Id) } ) },
           { nameof(DtoBase.CreatedAt), new PropertyMappingValue(new List<string>() { nameof(EntityBase.CreatedAt) } , true) },
           { nameof(DtoBase.CreatedBy), new PropertyMappingValue(new List<string>() { nameof(EntityBase.CreatedBy) } , true) },
           { nameof(TicketDto.TicketType), new PropertyMappingValue(new List<string>() { nameof(Ticket.TicketType) } )},
           { nameof(TicketDto.Status), new PropertyMappingValue(new List<string>() { nameof(Ticket.Status) } )},
           //{ nameof(Ticket.Note), new PropertyMappingValue(new List<string>() { nameof(Ticket.Note) } )},           
           //{ "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) }
      };

    private Dictionary<string, PropertyMappingValue> _CrudExamplePropertyMapping =
      new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
      {
           { nameof(DtoBase.Id), new PropertyMappingValue(new List<string>() { nameof(EntityBase.Id) } ) },
           { nameof(DtoBase.CreatedAt), new PropertyMappingValue(new List<string>() { nameof(EntityBase.CreatedAt) } , true) },
           { nameof(DtoBase.CreatedBy), new PropertyMappingValue(new List<string>() { nameof(EntityBase.CreatedBy) } , true) },
           { nameof(CrudExampleDto.Name), new PropertyMappingValue(new List<string>() { nameof(CrudExample.Name) } )},
           { nameof(CrudExampleDto.Description), new PropertyMappingValue(new List<string>() { nameof(CrudExample.Description) } )},
           { nameof(CrudExampleDto.IsActive), new PropertyMappingValue(new List<string>() { nameof(CrudExample.IsActive) } )},
           //{ nameof(Ticket.Note), new PropertyMappingValue(new List<string>() { nameof(Ticket.Note) } )},           
           //{ "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) }
      };


    private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

    public PropertyMappingService()
    {        
        _propertyMappings.Add(new PropertyMapping<TicketDto, Ticket>(_TicketPropertyMapping));
        _propertyMappings.Add(new PropertyMapping<CrudExampleDto, CrudExample>(_CrudExamplePropertyMapping));
    }

    public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
    {
        var propertyMapping = GetPropertyMapping<TSource, TDestination>();
        if (string.IsNullOrWhiteSpace(fields))        
            return true;        

        // the string is separated by ",", so we split it.
        var fieldsAfterSplit = fields.Split(',');

        // run through the fields clauses
        foreach (var field in fieldsAfterSplit)
        {
            // trim
            var trimmedField = field.Trim();

            // remove everything after the first " " - if the fields 
            // are coming from an orderBy string, this part must be 
            // ignored
            var indexOfFirstSpace = trimmedField.IndexOf(" ");
            var propertyName = indexOfFirstSpace == -1 ?
                trimmedField : trimmedField.Remove(indexOfFirstSpace);

            // find the matching property
            if (!propertyMapping.ContainsKey(propertyName))            
                return false;            
        }
        return true;
    }


    public Dictionary<string, PropertyMappingValue> GetPropertyMapping
       <TSource, TDestination>()
    {
        // get matching mapping
        var matchingMapping = _propertyMappings
            .OfType<PropertyMapping<TSource, TDestination>>();

        if (matchingMapping.Count() == 1)        
            return matchingMapping.First()._mappingDictionary;        

        throw new Exception($"Cannot find exact property mapping instance " +
            $"for <{typeof(TSource)},{typeof(TDestination)}");
    }
}
