using UnityEngine;
using System.Collections;

public class CampMicro : Micro{
	public CampfireItem campfire;
	public CampSmokeItem[] smokeItems;
	private int currentItem;
	private float smokeTime = 2f;
	private float currentTime;

	public override string Title{
		get{
			return "Avoid Smoke";
		}
	}

	public override int Background{
		get{
			return 4;
		}
	}

	void Update(){
		if(MicroMixManager.Instance.IsPaused || MicroMixManager.Instance.IsTutorial){
			return;
		}
		currentTime += Time.deltaTime;
		if(currentTime > smokeTime && currentItem < smokeItems.Length){
			smokeItems[currentItem].Setup(Random.value * 2 * Mathf.PI);
			currentItem++;
			currentTime = 0;
		}
		foreach(CampSmokeItem item in smokeItems){
			if(!item.Active){
				return;
			}
			if(Mathf.DeltaAngle(item.GetCurrentRadians() * Mathf.Rad2Deg, item.GetCurrentRadians() * Mathf.Rad2Deg) < Mathf.PI / 4){
				//item.Cancel();
			}
		}
	}

	protected override void _StartMicro(int difficulty, bool randomize){
		currentTime = 0;
	}

	protected override void _EndMicro(){
		
	}

	protected override void _Pause(){
		
	}

	protected override void _Resume(){
		
	}

	protected override IEnumerator _Tutorial(){
		yield return 0;
	}
}
