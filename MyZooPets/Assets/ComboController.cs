using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComboController : MonoBehaviour {
	public Text scoreText;
	public Text comboText;
	public Text comboTable;
	public GameObject UISlotPrefab;
	//Scrolling table
	private int comboTableLength = 10;
	private Image[] slotImages;

	public void Setup() {
		Debug.Log("Called");
		slotImages = new Image[comboTableLength];
		GameObject slotObject;
		for (int i = 0; i < slotImages.Length; i++) {
			slotObject = GameObjectUtils.AddChild(gameObject, UISlotPrefab);//Instantiate(UISlotPrefab);
			Debug.Log("GOTEM");
			/*slotObject.transform.position=transform.position+new Vector3(0,i*10);
			slotObject.transform.rotation=Quaternion.Euler(new Vector3(0,0,90));
			slotImages[i]=slotObject.GetComponent<Image>();*/
		}
	}

	public void UpdateScore(int newScore) {
		scoreText.text = "Score: \n" + newScore.ToString();
	}

	public void UpdateCombo(int newCombo) {
		comboText.text = "Combo: \n" + newCombo.ToString();
		//comboTable.text="";
		string tableString = "";
		for (int i = comboTableLength; i >= 0; i--) {
			if (newCombo < comboTableLength / 2) { //0->10 as long as combo is less than 5
				if (i == newCombo)
					tableString += "=";
				tableString += "x" + i + "\n";
			} else { //At this point the table should scroll
				int tableValue = (newCombo + (i - comboTableLength / 2)); //Goes from (-5 to +5) + newCombo
				if (tableValue == newCombo)
					tableString += "=";
				tableString += "x" + tableValue + "\n";
			}
		}
//		slotImages [newCombo % comboTableLength].color = GetComboColor(newCombo);
//		comboTable.text = tableString;
	}

	private Color GetComboColor(int newCombo) {
		switch (newCombo / 10) {
			case 0:
				return Color.red;
			case 1:
				return Color.blue;
			case 2:
				return Color.green;
			case 3:
				return Color.yellow;
			default:
				return Color.magenta;	
		}
	}
}
