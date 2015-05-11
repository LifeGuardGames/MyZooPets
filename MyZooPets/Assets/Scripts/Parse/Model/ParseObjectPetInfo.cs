using UnityEngine;
using System.Collections;
using Parse;

[ParseClassName("PetInfo")]
public class ParseObjectPetInfo : ParseObject {
	
	public ParseObjectPetInfo(){}

	[ParseFieldName("name")]
	public string Name{
		get{ return GetProperty<string>("Name");}
		set{ SetProperty<string>(value, "Name");}
	}

	[ParseFieldName("color")]
	public string Color{
		get{ return GetProperty<string>("Color");}
		set{ SetProperty<string>(value, "Color");}
	}

	[ParseFieldName("species")]
	public string Species{
		get{ return GetProperty<string>("Species");}
		set{ SetProperty<string>(value, "Species");}
	}
}
