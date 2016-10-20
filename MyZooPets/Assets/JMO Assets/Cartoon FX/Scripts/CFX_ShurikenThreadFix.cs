using UnityEngine;
using System.Collections;

// Cartoon FX  - (c) 2015, Jean Moreno

// Drag/Drop this script on a Particle System (or an object having Particle System objects as children) to prevent a Shuriken bug
// where a system would emit at its original instantiated position before being translated, resulting in particles in-between
// the two positions.
// Possibly a threading bug from Unity (as of 3.5.4)

public class CFX_ShurikenThreadFix : MonoBehaviour
{
	private ParticleSystem[] systems;
	
	void OnEnable()
	{
		systems = GetComponentsInChildren<ParticleSystem>();
		
		foreach(ParticleSystem ps in systems)
#pragma warning disable 0618 // Type or member is obsolete
			ps.enableEmission = false;
#pragma warning restore 0618 // Type or member is obsolete

		StartCoroutine("WaitFrame");
	}
	
	IEnumerator WaitFrame()
	{
		yield return null;
		
		foreach(ParticleSystem ps in systems)
		{
#pragma warning disable 0618 // Type or member is obsolete
			ps.enableEmission = true;
#pragma warning restore 0618 // Type or member is obsolete

			ps.Play(true);
		}
	}
}