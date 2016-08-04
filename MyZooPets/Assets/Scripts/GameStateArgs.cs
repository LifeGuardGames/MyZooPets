using System;

public class GameStateArgs : EventArgs {
	private MinigameStates eState;

	public MinigameStates GetGameState() {
		return eState;
	}

	public GameStateArgs(MinigameStates eState) {
		this.eState = eState;
	}
}
