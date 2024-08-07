namespace CommonCleanArch.Application.Search;

public interface IPropertyMappingService
{
    Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    bool ValidMappingExistsFor<TSource, TDestination>(string fields);
}
