using UnityEngine;
using System;
using System.Collections;

public enum PartitionLocationTypes{
	None,	// Default value
	Base,	// This is the starting 0 partition
	TriggerNinja,
	Shooter,
	Memory,
	Clinic,
	Runner
}

// A partition location in the specific scene
public class ImmutableDataPartitionLocation{

	private string id;
	public string Id{
		get{ return id; }
	}

	private Vector3 offset;
	public Vector3 Offset{
		get{ return offset; }
	}

	private ZoneTypes zone;
	public ZoneTypes Zone{
		get{ return zone; }
	}

	private int partition;
	public int Partition{
		get{ return partition; }
	}

	private PartitionLocationTypes attribute;
	public PartitionLocationTypes Attribute{
		get{ return attribute; }
	}

	public ImmutableDataPartitionLocation(string id, IXMLNode xmlNode, string error){
		Hashtable hashElements = XMLUtils.GetChildren(xmlNode);
		this.id = id;
		offset = StringUtils.ParseVector3(XMLUtils.GetString(hashElements["Offset"] as IXMLNode, null, error));
		zone = (ZoneTypes)Enum.Parse(typeof(ZoneTypes), XMLUtils.GetString(hashElements["Zone"] as IXMLNode, null, error));
		partition = XMLUtils.GetInt(hashElements["Partition"] as IXMLNode, 0, error);
		attribute = (PartitionLocationTypes)Enum.Parse(typeof(PartitionLocationTypes),
		                                               XMLUtils.GetString(hashElements["Attribute"] as IXMLNode, null, error));
	}
}
