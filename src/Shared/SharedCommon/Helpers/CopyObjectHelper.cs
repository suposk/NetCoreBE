using System.Xml.Serialization;

namespace SharedCommon.Helpers;

public static class CopyObjectHelper
{
    public static T CreateDeepCopyXml<T>(T obj)
    {
        using (var ms = new MemoryStream())
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return (T)serializer.Deserialize(ms);
        }
    }
}
