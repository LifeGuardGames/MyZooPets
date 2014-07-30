using System.Xml;
using System.Xml.Serialization;

public class InfoConstant{

	[XmlAttribute("Name")]
	public string Name{get; set;}   

	[XmlAttribute("DisplayName")]
	public string DisplayName{get; set;}
	
	[XmlAttribute("Type")]
	public string ConstantType{get; set;}
	
	[XmlAttribute("Value")]
	public string ConstantValue{get; set;}
}
	