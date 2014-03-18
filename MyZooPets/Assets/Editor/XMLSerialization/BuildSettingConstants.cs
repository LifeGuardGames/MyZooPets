using System.Xml; 
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("Constants")]
public class BuildSettingConstants{
    [XmlElement("Constant")]
    public List<Constant> BuildSettingConstantList = new List<Constant>();
    // [XmlElement("Constant")]
    // public Constant LiteBundleID{get; set;}

    // [XmlElement("Constant")]
    // public Constant ProBundleID{get; set;}
    // // [XmlElement("Constant")]
    // public Constant IsLiteVersion{get; set;}
    // // [XmlElement("Constant")]
    // public Constant AnalyticsEnabled{get; set;}
}
