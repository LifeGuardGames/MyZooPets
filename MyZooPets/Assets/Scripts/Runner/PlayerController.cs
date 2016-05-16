using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : Singleton<PlayerController>{
	public static EventHandler<EventArgs> OnJump;
	public static EventHandler<EventArgs> OnDrop;

	[System.Serializable]
	public class PlatformerControllerMovement{

		[System.NonSerialized]
		public float maxHeight = 18;
		public float minHeight = 9;
		public float maxSpeed = 45;
		public float minSpeed = 15;
		public float starSpeed = 50; //Speed when we go invicible
		public float slowdownRate = 10;
		public float currentSpeed = 0f; //current movement speed after it gets smoothed by acceleration
		public float targetSpeed; //The speed you want the character to reach to
		public float acceleration = 5f; //How fast does the character change speed? higher is faster
		public float jumpHeight = 9;
		[System.NonSerialized]
		public float verticalSpeed = 0f;
		public float maxFallSpeed = 100f; //maximum speed the player is allowed to fall
		public bool starMode = false; //Although both starMode and publicStarMode are public, this one is used internally
		public bool publicStarMode=false; //And this one is set after a delay and accessed externally through the property StarMode
		public bool invincible = false;
		private float gravity;
//		private float gravity = 130f; //gravity is calculated based on the target speed
		/// <summary>
		/// Gets the gravity. Dependent on the target speed
		/// </summary>
		/// <value>The gravity.</value>
		public float Gravity{ //Set when you jump
			get{
				return gravity;
			}
			set {
				gravity = 10 * value;
			}
		}

		/// <summary>
		/// Increases the target speed.
		/// </summary>
		/// <param name="isNormal">If set to <c>true</c> is normal.</param>
		public void IncreaseTargetSpeed(float increment){
			this.targetSpeed += increment;
			//cap the speed at 45
			if(this.targetSpeed >= maxSpeed)
				this.targetSpeed = maxSpeed;
		}

		/// <summary>
		/// Resets the target speed.
		/// </summary>
		public void ResetTargetSpeed(){
			targetSpeed = minSpeed; 
		}
	}
	public PlatformerControllerMovement movement = new PlatformerControllerMovement();
	public float timeUntilTargetSpeedIncrease = 30f;
	public Animator anim;
	public float minAnimSpeed = .6f;
	public float maxAnimSpeed = 1.5f;
	public Camera nguiCamera;
	private Vector2 amountToMove; //How much you want the player to move
	private PlayerPhysics playerPhysics; //Reference to physics
	private float speedIncreaseCounter = 0f; //Time till we speed up the game
	private Vector2 initialPosition; //Where the player start
	private GameObject floatyLocation;
	private SpriteRenderer[] spriteRendererList; //List of SpriteRenderers loaded by PetSpriteColorLoader
	private Color[] colorList; //Used by spriteRendererList to revert colors back to original
	private bool flipColor = false;
	private float magnetTime=0; //How long until magnet is disabled
	private float magnetTimeIncrease=5f; //How much extra time a magnet gives
	private IEnumerator currentColor;
	#if UNITY_EDITOR	
	// used just for testing keyboard input in unity editor
	private bool bDelay = false;
#endif
	public float Speed {
		get{
			return movement.currentSpeed;
		}
	}
	public float MinSpeed {
		get{
			return movement.minSpeed;
		}
	}
	public float MaxSpeed {
		get{
			return movement.maxSpeed;
		}
	}
	public bool StarMode {
		get {
			return movement.publicStarMode;
		}
	}
	public bool Invincible {
		get {
			return movement.invincible||movement.publicStarMode;
		}
	}
	public GameObject FloatyLocation{
		get{
			return floatyLocation;
		}
	}

	void Start(){
		playerPhysics = GetComponent<PlayerPhysics>();
		initialPosition = this.transform.position;
		floatyLocation = this.transform.Find("FloatyLocation").gameObject;
		RunnerGameManager.OnStateChanged += GameStateChanged;
		Reset();
	}
    
	void Update(){
		if(!RunnerGameManager.Instance.GameRunning)
			return;
#if UNITY_EDITOR    
        CheckKeyMovement();
#endif
		UpdateSpeed();
		CheckAndActOnDeath();
		UpdateMagnet();
	}

	void OnDestroy(){
		RunnerGameManager.OnStateChanged -= GameStateChanged;
	}

#if UNITY_EDITOR    
//	void OnGUI(){
//		GUI.contentColor = Color.black;
//		 if(GUI.Button(new Rect(0, 0, 100, 100), "+speed")){
//			movement.targetSpeed += 5;
//		 }
//
//		if(GUI.Button(new Rect(100, 0, 100, 100), "-speed")){
//			movement.targetSpeed -= 5;
//		}

//		GUI.Label(new Rect(200, 0, 100, 100), movement.Gravity.ToString());
//	}
#endif
	void OnGUI(){
		if (magnetTime>0){
			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
			GUI.Box(new Rect(screenPos.x-30,Screen.height-screenPos.y-20,60,40), "MAGNETIC: \n" + magnetTime.ToString());
		}
	}
	void FixedUpdate(){
		if(!RunnerGameManager.Instance.GameRunning)
			return;

		//update runner horizontal movement
		UpdateHorizontalMovement();

		//apply game gravity
		ApplyGravity(); 

		amountToMove = new Vector2(movement.currentSpeed, movement.verticalSpeed);

		//always want movement to be framerate independent
		amountToMove *= Time.deltaTime;

		playerPhysics.Move(amountToMove);
	}

	// Listen to finger down gesture
	void OnTap(TapGesture e){ 
		if(RunnerGameManager.Instance.GameRunning ){
			float touchLocationDifference = Camera.main.ScreenToWorldPoint(e.Position).y - floatyLocation.transform.position.y; //Height of touch - height of player
			if(e.Position.y>Screen.height/2){
				Jump(movement.jumpHeight);//Jump(Mathf.Clamp(touchLocationDifference,movement.minHeight,movement.maxHeight)); //In world coordinates
				if(OnJump != null)
					OnJump(this, EventArgs.Empty);
			}
			else{
				Drop();
				
				if(OnDrop != null)
					OnDrop(this, EventArgs.Empty);
			}


		}
	}
	
	/* Listen to swipe down gesture
	void OnSwipe(SwipeGesture gesture){
		if(RunnerGameManager.Instance.GetGameState() == MinigameStates.Playing){
			FingerGestures.SwipeDirection direction = gesture.Direction;
			if(direction == FingerGestures.SwipeDirection.Down){
				Drop();
				
				if(OnDrop != null)
					OnDrop(this, EventArgs.Empty);
			}
			else if (direction == FingerGestures.SwipeDirection.Up){
				Jump();
			
			if(OnJump != null)
				OnJump(this, EventArgs.Empty);
			
		}
		}
	}*/
	
	/// <summary>
	/// Reset player position and physics
	/// </summary>
	public void Reset(){
		speedIncreaseCounter = 0f;
		transform.position = initialPosition;
		magnetTime=0;
		MagneticField.Instance.enabled=false;
		movement.verticalSpeed = 0f;
		movement.currentSpeed = 0f;
		anim.speed=minAnimSpeed;
		movement.ResetTargetSpeed();
		movement.Gravity=movement.targetSpeed;
	}
	/// <summary>
	/// Passes the body's renderer for coloring effects
	/// </summary>
	public void SetRenderers(SpriteRenderer[] spriteRendererList){
		this.spriteRendererList=spriteRendererList;
		colorList = new	Color[spriteRendererList.Length];
		for (int i=0; i< colorList.Length; i++){
			colorList[i] = spriteRendererList[i].color;
		}
	}

	/// <summary>
	/// Increases the player's target speed which causes the player to speed up over time
	/// </summary>
	public void IncreaseSpeed(float increment){
		movement.IncreaseTargetSpeed(increment);
	}
	public void StartStarMode(){
		SolidColor(c: new Color(.8f,.8f,0,.8f));
	}
	public void StartMagnet(){ //TODO: Add some visual effect when magnet is enabled
		if (magnetTime<=0)
			magnetTime=magnetTimeIncrease;
		else
			magnetTime+=magnetTimeIncrease;
		MagneticField.Instance.enabled=true;
	}
	/// <summary>
	/// Makes the player visible.
	/// </summary>
	/// <param name="isVisible">If set to <c>true</c> is visible.</param>
	public void MakePlayerVisible(bool isVisible){
		this.transform.Find("Body").gameObject.SetActive(isVisible);
	}
	//---------------------------------------------------
	// TriggerSlowdown()
	// Slow down the game and decrease the distance between 
	// player and megahazard
	//---------------------------------------------------
	public void TriggerSlowdown(float inDivisor, string itemID){
		movement.ResetTargetSpeed();
		MegaHazard.Instance.TriggerSlowdown();
		if (DataManager.Instance.GameData.RunnerGame.RunnerItemCollided.Contains(itemID)){ //The tutorial pop up handles the flash call itself, so only activate on subsequent calls
			//StartCoroutine(FlashColor());
			FlashColor(Color.red);
		}
	}
	public void SolidColor(Color c, float time=3f){
		if (currentColor!=null) //If there is a coroutine running right now
			StopCoroutine(currentColor);
		RevertColor();
		currentColor = SolidIEnumerator(c, time);
		StartCoroutine(currentColor);
		
	}
	public void FlashColor(Color c){
		if (currentColor!=null) //If there is a coroutine running right now
			StopCoroutine(currentColor);
		RevertColor();
		currentColor = FlashIEnumerator(c);
		StartCoroutine(currentColor);
	}
	private IEnumerator FlashIEnumerator(Color c) { //HACK: Both flash and solid have code extra to simply changing the color. Still, this seems better than having to pass a function to this coroutine
		movement.invincible=true;

		for (int i=0; i<3; i++){
			TurnColor(c);
			yield return new WaitForSeconds(.2f);
			RevertColor();
			yield return new WaitForSeconds(.3f);
		}
		movement.invincible=false;
	}
	private IEnumerator SolidIEnumerator(Color c, float time) {
		movement.invincible=true;
		movement.starMode=true;
		movement.publicStarMode=true;
		TurnColor(c);
		yield return new WaitForSeconds(time);
		movement.starMode=false;
		yield return new WaitForSeconds(1f); //Give them a second (figurative) to get oriented and avoid smoke clouds
		RevertColor();
		movement.invincible=false;
		//movement.publicStarMode=false;
		
	}
	private void TurnColor(Color c){
		for (int i=0; i< colorList.Length; i++){
			spriteRendererList[i].color = c;
		}
	}
	private void RevertColor(){
		for (int i=0; i< colorList.Length; i++){
			spriteRendererList[i].color = colorList[i];
		}

	}

	private void UpdateHorizontalMovement(){
		if (movement.starMode) {
			movement.currentSpeed = Mathf.Lerp(movement.currentSpeed, movement.starSpeed, movement.acceleration * Time.deltaTime); 
		} else {
			movement.currentSpeed = Mathf.Lerp(movement.currentSpeed, movement.targetSpeed, movement.acceleration * Time.deltaTime); 
			if (!movement.starMode&&movement.publicStarMode&&Mathf.Abs(movement.currentSpeed/movement.targetSpeed-1)<.1) //If we are waiting to turn of publicStarMode
				movement.publicStarMode=false;
		}
		float speedPercentage = (movement.currentSpeed-MinSpeed)/(MaxSpeed-MinSpeed);
		speedPercentage = (speedPercentage<0) ? 0 : speedPercentage; //Make sure we are greater than 0.
		float targetAnimSpeed = minAnimSpeed + (maxAnimSpeed-minAnimSpeed) * speedPercentage; 
		anim.speed = Mathf.Lerp(anim.speed, targetAnimSpeed, movement.acceleration * Time.deltaTime); 
	}

	/// <summary>
	/// Applies the gravity.
	/// gravity will be changed manually depending on the speed of the horizontal
	/// movement
	/// </summary>
	private void ApplyGravity(){

		//if grounded just set speed to gravity speed
		if(playerPhysics.Grounded && !playerPhysics.Jumping){
			movement.verticalSpeed = -movement.Gravity * Time.deltaTime;
		}
        //if jumping keep decreasing the vertical speed by gravity
        else{
			movement.verticalSpeed -= movement.Gravity * Time.deltaTime;
		}

		//make sure we don't fall nay faster than maxFallSpeed
		movement.verticalSpeed = Mathf.Max(movement.verticalSpeed, -movement.maxFallSpeed);
	}

	/// <summary>
	/// Calculates the jump vertical speed.
	/// </summary>
	/// <returns>The vertical speed.</returns>
	/// <param name="targetJumpHeight">Target jump height.</param>
	private float CalculateJumpVerticalSpeed(float targetJumpHeight){
		// from jump height and gravity we deduce the upwards speed for character
		// at apex
		return Mathf.Sqrt(2 * targetJumpHeight * movement.Gravity);
	}
	//heightMultiplier multiplies movement.jumpHeight to increase speed and max heiht achieved.
	private void Jump(float height){
		if(playerPhysics.Grounded){
         //   AudioManager.Instance.PlayClip("runnerJumpUp", variations: 3);
			AudioManager.Instance.PlayClip("runnerJumpUp");
			//movement.verticalSpeed = CalculateJumpVerticalSpeed(movement.jumpHeight);
			movement.Gravity=movement.targetSpeed;
			movement.verticalSpeed = CalculateJumpVerticalSpeed(height);
			playerPhysics.Jumping = true;
		}
	}

	private void Drop(){
		AudioManager.Instance.PlayClip("runnerJumpDown");

		playerPhysics.AllowPassThroughLayer = true;

		// Immediately tween down
		if(movement.verticalSpeed >= 0){
			movement.verticalSpeed = -10f;
		}
		else{
			movement.verticalSpeed += -10f;
		}
	}
	
	//---------------------------------------------------
	// UpdateSpeed()
	// Increase the pace of the game
	//---------------------------------------------------
	private void UpdateSpeed(){ //Replaced by coin increase speed
		/*if(!RunnerGameManager.Instance.IsTutorialRunning()){
			speedIncreaseCounter += Time.deltaTime;

			if(speedIncreaseCounter >= timeUntilTargetSpeedIncrease){
				//increase time
				movement.IncreaseTargetSpeed(true);
				anim.speed += 0.2f;
				speedIncreaseCounter = 0; 
			}
		}*/
	}

	/// <summary>
	/// Listen to when game state change. Disable runner animation when game is paused.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	private void GameStateChanged(object sender, GameStateArgs args){
		MinigameStates gameState = args.GetGameState();
		if(gameState == MinigameStates.Paused){
			MakePlayerVisible(false);
		}else if(gameState == MinigameStates.Playing){
			MakePlayerVisible(true);
		}
	}
	//---------------------------------------------------
	// CheckAndActOnDeath()
	// If player falls below the "Dead line" than the player dies
	//---------------------------------------------------
	private void CheckAndActOnDeath(){
		RunnerLevelManager runnerLevelManager = RunnerLevelManager.Instance;
		if(transform.position.y < runnerLevelManager.LevelTooLowYValueGameOver){
			EndGame();
		} 
	}
	private void UpdateMagnet(){
		if (magnetTime>=0)
			magnetTime-=Time.deltaTime;
		else 
			MagneticField.Instance.enabled=false;
	}
	public void EndGame(){
		RunnerLevelManager runnerLevelManager = RunnerLevelManager.Instance;
		runnerLevelManager.mCurrentLevelGroup.ReportDeath();
		AudioManager.Instance.PlayClip("runnerDie");
		RunnerGameManager.Instance.ActivateGameOver();    

	}
	
	#if UNITY_EDITOR
	//---------------------------------------------------
	// UpdateMovement()
	// Moves the player along the x axis with default speed. 
	// Check for jumping and falling physics as well.
	//---------------------------------------------------
	private void CheckKeyMovement(){
		
		if(Input.GetKey("up")) Jump(movement.jumpHeight);
		if(Input.GetKey("down") && !playerPhysics.Falling && !bDelay) {
			bDelay = true;
			Drop();
		}
		else
			bDelay = false;
	}
	#endif

	//True: if finger touches NGUI 
	/// <summary>
	/// Determines whether if the touch is touching NGUI element
	/// </summary>
	/// <returns><c>true</c> if this instance is touching NGUI; otherwise, <c>false</c>.</returns>
	/// <param name="screenPos">Screen position.</param>
	private bool IsTouchingNGUI(Vector2 screenPos){
		Ray ray = nguiCamera.ScreenPointToRay(screenPos);
		RaycastHit hit;
		int layerMask = 1 << 10; 
		bool isOnNGUILayer = false;
		
		// Raycast
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)){
			isOnNGUILayer = true;
		}
		return isOnNGUILayer;
	}
}
