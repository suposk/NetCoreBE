namespace CommonCleanArch.Helpers;
public static class OtherHelper
{
	public static double ConvertBytesToMegabytesExt(this long bytes) => (bytes / 1024f) / 1024f;	

	public static bool IsNotNullValidIdExt(this IDtoBase dto)
		=> dto == null || string.IsNullOrWhiteSpace(dto.Id.ToString()) || dto.Id == Guid.Empty;

    public static bool IsNotNullValidIdExt(this EntityBase dto)
        => dto == null || string.IsNullOrWhiteSpace(dto.Id);

    public static bool IsNotNullValidIdExt(this string Id) => Id == null || string.IsNullOrWhiteSpace(Id);
	//|| Guid.Parse(Id) == Guid.Empty;

	public static string? GetTypeNameExt(this Type type) 
	{
		if (type == null)
            return null;
        var typeName = type.FullName;
        if (type.IsGenericType)
		{
            var genericType = type.GetGenericTypeDefinition();
            var genericTypeName = genericType.Name;
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            var genericArgs = string.Join(",", type.GetGenericArguments().Select(t => t.GetTypeNameExt()).ToArray());
            //typeName = $"{genericTypeName}<{genericArgs}>";
            typeName = $"{type.Namespace}.{genericTypeName}<{genericArgs}>";
        }
        return typeName;
	}
}
