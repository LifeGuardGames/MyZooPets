using System.Xml; 
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Constants")]
public class CriticalConstants{
    [XmlElement("Constant")]
    public List<CriticalConstant> criticalConstantList = new List<CriticalConstant>();
}

public class CriticalConstant{
    [XmlAttribute("Name")]
    public string Name{get; set;}    

    [XmlAttribute("Type")]
    public string ConstantType{get; set;}

    [XmlAttribute("Value")]
    public string ConstantValue{get; set;}

    //XML format that our parser read is different from what XML Serializer output
    //need this inner text variable to output a closing tag <Constant></Constant> instead of just
    //<constant />
    [XmlText]
    public string Filler{get; set;}
}