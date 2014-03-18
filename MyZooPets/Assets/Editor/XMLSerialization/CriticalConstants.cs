using System.Xml; 
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Constants")]
public class CriticalConstants{
    [XmlElement("Constant")]
    public List<Constant> CriticalConstantList = new List<Constant>();
}
