using UnityEngine;
using System.Collections;

public class DoctorMatchZone : MonoBehaviour {

	public enum DoctorMatchButtonTypes{
		None,
		Green,
		Yellow,
		Red
	}

	public DoctorMatchButtonTypes buttonType;

	public ParticleSystem particle;

	public void OnZoneClicked(){

	}
}
