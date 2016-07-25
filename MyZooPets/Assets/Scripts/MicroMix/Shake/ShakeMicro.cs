using UnityEngine;
using System.Collections;

public class ShakeMicro : Micro{
	public GameObject inhaler;

	public override string Title{
		get{
			return "Shake";
		}
	}

	public override int Background{
		get{
			return 0;
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		
	}

	protected override void _EndMicro(){
		
	}

	protected override IEnumerator _Tutorial(){
		MicroMixFinger finger = MicroMixManager.Instance.finger;
		finger.gameObject.SetActive(true);
		yield return finger.MoveTo(inhaler.transform.position,inhaler.transform.position+Vector3.down*2,.3f,.15f);
		for(int i = 0; i < 2; i++){
			yield return finger.ShakeToBack(inhaler.transform.position+Vector3.down*2,inhaler.transform.position+Vector3.up*1,0,.3f);
		}

		finger.gameObject.SetActive(false);
	}
}
