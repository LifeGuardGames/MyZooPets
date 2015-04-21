using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Game tutorial decorations new
/// This is a redo for the decorations, basically the old one is too complicated
/// just trimming this down to encourage the user to click on the deco mode.
/// Hopefully the rest of the steps are self-guided enough to carry the user through.
/// </summary>
public class GameTutorialDecorationsNew : GameTutorial{
	public GameTutorialDecorationsNew() : base(){
	}

	protected override void SetMaxSteps(){
		maxSteps = 1;
	}

	protected override void SetKey(){
		tutorialKey = TutorialManagerBedroom.TUT_DECOS;
	}

	protected override void _End(bool isFinished){
	}

	protected override void ProcessStep(int step){
		switch(step){
		case 0:
			ShineOnDecoButton();
			break;
		}
	}

	private void ShineOnDecoButton(){
		DecoInventoryUIManager.OnDecoOpened += RemoveShineOnDecoButton;
		NavigationUIManager.Instance.ToggleShineDecoButton(true);

		// add the button to the process list so the user can click it
		AddToProcessList(NavigationUIManager.Instance.DecoButton);
	}

	public void RemoveShineOnDecoButton(object go, EventArgs e){
		DecoInventoryUIManager.OnDecoOpened -= RemoveShineOnDecoButton;
		NavigationUIManager.Instance.ToggleShineDecoButton(false);
		Advance();
	}
}
