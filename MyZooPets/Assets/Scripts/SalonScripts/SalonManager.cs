using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SalonManager : MonoBehaviour {

	public Button nameButton;
	public Button styleButton;
	public Button buyButton;
	public Text inputFieldabel;
	public InputField input;
	public PositionTweenToggle namePanel;
	public PositionTweenToggle stylePanel;
	public bool HasMadeChange;
	private string tempText = "None";
	private string tempColor = "None";
	public PetSpriteColorLoader loader;
	public PositionTweenToggle purchaseButton;

	// Use this for initialization
	void Start () {
		inputFieldabel.text = DataManager.Instance.GameData.PetInfo.PetName;
    }

	public void ShowNamePanel() {
		nameButton.GetComponent<PositionTweenToggle>().hideDeltaX = -550;
		styleButton.GetComponent<PositionTweenToggle>().hideDeltaX = -550;
		//TODO update hide position
		nameButton.GetComponent<PositionTweenToggle>().UpdateHideDelta();
		styleButton.GetComponent<PositionTweenToggle>().UpdateHideDelta();
		//TODO Hide Background
		nameButton.GetComponent<PositionTweenToggle>().Hide();
		styleButton.GetComponent<PositionTweenToggle>().Hide();
		namePanel.Show();
	}

	public void HideNamePanel() {
		//TODO Show Background
		nameButton.GetComponent<PositionTweenToggle>().Show();
		styleButton.GetComponent<PositionTweenToggle>().Show();
		namePanel.Hide();
	}

	public void ShowStylePanel() {
		nameButton.GetComponent<PositionTweenToggle>().hideDeltaX = 550;
		styleButton.GetComponent<PositionTweenToggle>().hideDeltaX = 550;
		//TODO update hide position
		nameButton.GetComponent<PositionTweenToggle>().UpdateHideDelta();
		styleButton.GetComponent<PositionTweenToggle>().UpdateHideDelta();
		//TODO Hide Background
		nameButton.GetComponent<PositionTweenToggle>().Hide();
		styleButton.GetComponent<PositionTweenToggle>().Hide();
		stylePanel.Show();
	}

	public void HideStylePanel() {
		//TODO Show Background
		nameButton.GetComponent<PositionTweenToggle>().Show();
		styleButton.GetComponent<PositionTweenToggle>().Show();
		stylePanel.Hide();
	}

	public void ChangeName() {
		purchaseButton.Show();
		tempText = input.text;
		inputFieldabel.text = tempText;
		HasMadeChange = true;
	}

	public void ChangeColor(string color) {
		purchaseButton.Show();
		HasMadeChange = true;
		tempColor = color;
		loader.ChangeStyle(tempColor);
	}

	public void Quit() {
		LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
	}

	public void ShowPurchaseButton() {
		purchaseButton.Show();
	}

	public void PurchasingChange() {
		if(tempText != "None") { 
			DataManager.Instance.GameData.PetInfo.ChangeName(tempText);
		}
		if(tempColor != "None") {
			switch(tempColor) {
				case "BlueYellow":
					DataManager.Instance.GameData.PetInfo.ChangeColor(PetColor.BlueYellow);
					break;
				case "OrangeYellow":
					DataManager.Instance.GameData.PetInfo.ChangeColor(PetColor.OrangeYellow);
					break;
				case "PinkBlue":
					DataManager.Instance.GameData.PetInfo.ChangeColor(PetColor.PinkBlue);
					break;
				case "PurpleLime":
					DataManager.Instance.GameData.PetInfo.ChangeColor(PetColor.PurpleLime);
					break;
				case "YellowPink":
					DataManager.Instance.GameData.PetInfo.ChangeColor(PetColor.YellowPink);
					break;
			}
		}
		DataManager.Instance.GameData.Inventory.InventoryItems.Remove("Usable1");
	}
}
