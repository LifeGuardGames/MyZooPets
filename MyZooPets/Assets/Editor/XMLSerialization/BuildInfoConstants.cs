using System.Xml; 
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Constants")]
public class BuildInfoConstants{
	[XmlElement("Constant")]
	public List<Constant> BuildInfoConstantList = new List<Constant>();
}