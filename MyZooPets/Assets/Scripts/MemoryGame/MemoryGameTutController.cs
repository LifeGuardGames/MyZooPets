using UnityEngine;

public class MemoryGameTutController : MonoBehaviour {
    public void OnTutorialDoneButton() {
		MemoryGameManager.Instance.OnTutorialComplete();
	}
}
