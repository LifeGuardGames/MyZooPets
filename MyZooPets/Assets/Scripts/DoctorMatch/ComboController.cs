using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboController : MonoBehaviour{
	public Text scoreText;
	public Text comboText;
	public Transform comboTable;
	public GameObject UISlotPrefab;
	//Scrolling table
	private int colorCount = 6;
	private RawImage[] slotImages;
	private Color[] colorCache;
	private IEnumerator flashLine;
	private IEnumerator clearLine;
	private IEnumerator timeLowLine;
	private int lastCombo = 0;
	private int combo = 0;
	//Maximum amount of time for a correct diagnosis
	private float timeToCombo = 3f;
	private float currentComboTime = 0;
	private int comboBonus = 5;
	private bool countingDown = true;

	void Update(){
		if(DoctorMatchManager.Instance.Paused || !countingDown)
			return;
		if(currentComboTime > 0){
			currentComboTime -= Time.deltaTime;
			if(currentComboTime <= timeToCombo / 2){
				TimeLowColor(currentComboTime);
			}
		}
		else if(combo != 0){
			ResetCombo();
		}
	}

	public int Combo{
		get{ return combo; }
	}

	public int ComboMod{
		get { return Mathf.Clamp(combo, 0, (comboBonus * 2) - 1); }
	}

	public int ComboLevel{
		get{
			if((combo + 1) % (comboBonus * 2) == 0 && combo != 0){ //Big combo bonus
				return 2;
			}
			else if((combo + 1) % comboBonus == 0 && combo != 0){ //Small combo bonus
				return 1;
			}
			else{
				return 0;
			}
		}
	}

	public void StartCounting(){
		countingDown = true;
	}

	public void StopCounting(){
		countingDown = false;
		if(timeLowLine != null){
			StopCoroutine(timeLowLine);
			timeLowLine = null;
			RestoreColors();
		}
	}

	public void ResetCombo(){
		combo = 0;
		currentComboTime = 0;
		UpdateCombo();
	}

	public void Setup(){
		slotImages = new RawImage[(comboBonus * 2)];
		GameObject slotObject;
		for(int i = 0; i < slotImages.Length; i++){
			slotObject = Instantiate(UISlotPrefab);
			slotObject.transform.SetParent(comboTable);
			slotObject.transform.localScale = new Vector3(1, 1, 1);
			slotObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
			slotImages[i] = slotObject.GetComponent<RawImage>();
			slotImages[i].rectTransform.localPosition = new Vector3(0, slotImages[i].rectTransform.sizeDelta.x * i);
		}
	}

	public void UpdateScore(int newScore){
		scoreText.text = "Score: \n" + newScore.ToString();
	}

	public void TimeLowColor(float timeLeft){
		if(flashLine != null){
			StopCoroutine(flashLine);
			flashLine = null;
		}
		if(timeLowLine == null){
			timeLowLine = TimeLow(timeLeft);
			StartCoroutine(timeLowLine);
		}
	}

	public Vector3 GetComboPosition(int combo){
		return slotImages[combo % (comboBonus * 2)].transform.position;
	}

	public void IncrementCombo(){
		combo++;
		UpdateCombo();
		currentComboTime = timeToCombo;
	}

	private void UpdateCombo(){
		comboText.text = "x" + combo.ToString();
		if(combo == 0){
			StopColor();
		}
		else if(combo / comboBonus < colorCount){
			SetColor(combo - 1);
			lastCombo = combo;
		}
		else{
			FlashColor(combo);
		}
		lastCombo = ComboMod;
	}

	private void StopColor(){ //newCombo == 0
		if(flashLine != null){
			StopCoroutine(flashLine);
			flashLine = null;
		}
		if(timeLowLine != null){
			StopCoroutine(timeLowLine);
			timeLowLine = null;
			RestoreColors();
		}
		if(clearLine == null){
			clearLine = ClearLine();
			StartCoroutine(clearLine);
		}
	}

	private void SetColor(int newCombo){ //0 <= newCombo/littleCombo < colorCount
		if(timeLowLine != null){
			StopCoroutine(timeLowLine);
			timeLowLine = null;
			RestoreColors();
		}
		if(clearLine != null){
			StopCoroutine(clearLine);
			clearLine = null;
			ImmediateClear();
		}
		slotImages[newCombo % (comboBonus * 2)].color = GetComboColor(newCombo / comboBonus);
	}

	private void FlashColor(int newCombo){ //colorCount <= newCombo/littleCombo
		if(timeLowLine != null){
			StopCoroutine(timeLowLine);
			timeLowLine = null;
			RestoreColors();
		}
		if(flashLine == null){
			flashLine = FlashBar();
			StartCoroutine(flashLine);
		}
	}

	private IEnumerator ClearLine(){
		for(int i = lastCombo; i >= 0; i--){
			slotImages[i].color = Color.white;
			yield return new WaitForEndOfFrame();
		}
		clearLine = null; //Mark us as done
	}

	private IEnumerator TimeLow(float timeLeft){
		colorCache = new Color[(comboBonus * 2)];
		for(int i = 0; i < (comboBonus * 2); i++){ //Cache the colors
			colorCache[i] = slotImages[i].color;
		}
		ImmediateClear();
		yield return new WaitForSeconds(timeLeft / 4);
		RestoreColors();
		yield return new WaitForSeconds(timeLeft / 4);
		ImmediateClear();
		yield return new WaitForSeconds(timeLeft / 4);
		RestoreColors();
		yield return new WaitForSeconds(timeLeft / 4);
		timeLowLine = null; //Mark us as done
	}

	private IEnumerator FlashBar(){ //Does not need to be marked as done, will be marked null when completed
		int colorStart = 0;
		while(true){
			for(int i = 0; i < (comboBonus * 2); i++){
				slotImages[i].color = GetComboColor((int)Mathf.PingPong(i + colorStart, colorCount - 1));
			}
			yield return new WaitForSeconds(.3f);
			colorStart++;
		}
	}

	private void ImmediateClear(){
		for(int i = 0; i < (comboBonus * 2); i++){
			slotImages[i].color = Color.white;
		}
	}

	private void RestoreColors(){
		for(int i = 0; i < (comboBonus * 2); i++){
			slotImages[i].color = colorCache[i];
		}
	}

	private Color GetComboColor(int colorNumber){ //ROYGBIV, once you reach a certain level the bar starts flashing.
		Color color = Color.black;
		switch(colorNumber){ //Red
		case 0:
			ColorUtility.TryParseHtmlString("#FF0000", out color);
			break;	
		case 1:
			ColorUtility.TryParseHtmlString("#FFA500", out color);
			break;
		case 2:
			ColorUtility.TryParseHtmlString("#FFFF00", out color);
			break;
		case 3:
			ColorUtility.TryParseHtmlString("#00FF00", out color);
			break;
		case 4:
			ColorUtility.TryParseHtmlString("#0000FF", out color);
			break;
		case 5:
			ColorUtility.TryParseHtmlString("#4B0082", out color);
			break;
		default:
			Debug.LogWarning("INVALID COLOR");
			break;
		}
		return color;
	}
}
