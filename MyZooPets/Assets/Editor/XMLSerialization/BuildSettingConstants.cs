using System.Xml; 
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Constants")]
public class BuildSettingConstants{
    [XmlElement("Constant")]
    public List<Constant> BuildSettingConstantList = new List<Constant>();
}
