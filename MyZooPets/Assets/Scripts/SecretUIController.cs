using UnityEngine;
using System.Collections;

public class SecretUIController : Singleton<MonoBehaviour> {

	public TweenToggle tween;

	public void ShowPanel(){
		tween.Show();
	}

	public void HidePanel(){
		tween.Hide();
	}
}
