using UnityEngine;
using System.Collections;

public class MicroMixFireworks : MonoBehaviour{
	public ParticleSystem[] pSystems;
	private bool[] particlesPaused;
	private IEnumerator fireIEnum;
	void Start(){
		particlesPaused = new bool[pSystems.Length];
	}

	public void StartFireworks(){
		fireIEnum = FireworksHelper();
		StartCoroutine(fireIEnum);

	}

	public void StopFireworks(){
		StopCoroutine(fireIEnum);
		for(int i = 0; i < pSystems.Length; i++){
			pSystems[i].Stop();
		}
	}

	public void Pause(){
		for(int i = 0; i < pSystems.Length; i++){
			particlesPaused[i] = pSystems[i].isPlaying;
			pSystems[i].Pause();
		}
	}

	public void Resume(){
		for(int i = 0; i < pSystems.Length; i++){
			if(particlesPaused[i]){
				pSystems[i].Play();
			}
		}
	}
	private IEnumerator FireworksHelper(){
		while (true){
			int index;
			do {
				index = Random.Range(0,pSystems.Length);
			} while (pSystems[index].isPlaying);
			pSystems[index].transform.position = CameraUtils.RandomWorldPointOnScreen(Camera.main,.2f,.2f,0);
			pSystems[index].Play();
			yield return MicroMixManager.Instance.WaitSecondsPause(.25f);
		}
	}
}
