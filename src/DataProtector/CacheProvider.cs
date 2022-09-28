using DataProtector.Protector;
using System.Xml.Serialization;
using System.Xml;
using System.Text;

namespace DataProtector;

public class CacheProvider
{
    public void CacheConnections(List<ConnectionString> connections)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ConnectionString>));
        using MemoryStream memoryStream = new MemoryStream();

        using XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xmlSerializer.Serialize(xmlTextWriter, connections);
        //xmlSerializer.Serialize(memoryStream, connections);

        byte[] buffer = memoryStream.ToArray().Protect();
        File.WriteAllBytes("data.protected", buffer);
    }

    public List<ConnectionString> GetConnectionsFromCache()
    {
        var data = File.ReadAllBytes("data.protected").Unprotect();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ConnectionString>));
        var retValue = (List<ConnectionString>)xmlSerializer.Deserialize(new MemoryStream(data));
        return retValue;
    }
}
