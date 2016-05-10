using UnityEngine;
using System.Collections;

public class DoctorMatchZone : MonoBehaviour {
	public DoctorMatchManager.DoctorMatchButtonTypes buttonType;
	public ParticleSystem particle;

	public void OnZoneClicked(){
		DoctorMatchManager.Instance.OnZoneClicked(buttonType);
	}
}
