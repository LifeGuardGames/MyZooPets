using UnityEngine.SceneManagement;

/// <summary>
/// Questionaire manager. Check when it's appropriate to spawn the questionaire
/// </summary>
public class QuestionaireManager : Singleton<QuestionaireManager> {
	public QuestionaireAgeController questionaireAgeController;
	public QuestionaireAsthmaController questionaireAsthmaController;

	void Start(){
		#if DEVELOPMENT_BUILD
		//PlayerPrefs.DeleteAll();
		#endif

		CheckToOpenQuestionaire();
	}

	/// <summary>
	/// Checks to open questionaire in loading scene. Collects user age and asthma info
	/// </summary>
	private void CheckToOpenQuestionaire(){
		if(!DataManager.Instance.IsQuestionaireCollected){
			Invoke("ShowQuestionaire", 0.5f);
		}
		else{
			ContinueLoading(false);
		}
	}

	private void ShowQuestionaire(){
		questionaireAgeController.ShowPanel();
	}

	/// <summary>
	/// Collected information about player
	/// </summary>
	public void QuestionaireCollected(){
		DataManager.Instance.IsQuestionaireCollected = true;
	}

	public void ContinueLoading(bool doTransition){
		if(DataManager.Instance.GameData.PetInfo.IsHatched){
			if(doTransition) {
				LoadLevelManager.Instance.StartLoadTransition(SceneUtils.BEDROOM);
			}
			else {
				PromotionUiManager.Instance.Show();
			}
		}
		else{
			if(doTransition) {
				LoadLevelManager.Instance.StartLoadTransition(SceneUtils.MENU);
			}
			else {
				SceneManager.LoadScene(SceneUtils.MENU);
			}
		}
	}
}
