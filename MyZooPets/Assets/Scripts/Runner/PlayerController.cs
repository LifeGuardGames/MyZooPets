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
		public float starSpeed = 50;
		//Speed when we go invicible
		public float slowdownRate = 10;
		public float currentSpeed = 0f;
		//current movement speed after it gets smoothed by acceleration
		public float targetSpeed;
		//The speed you want the character to reach to
		public float acceleration = 5f;
		//How fast does the character change speed? higher is faster
		public float jumpHeight = 9;
		[System.NonSerialized]
		public float verticalSpeed = 0f;
		public float maxFallSpeed = 100f;
		//maximum speed the player is allowed to fall
		public bool starMode = false;
		//Although both starMode and publicStarMode are public, this one is used internally
		public bool publicStarMode = false;
		//And this one is set after a delay and accessed externally through the property StarMode
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
			set{
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
	public ParticleSystem magnetSystem;
	public Animator anim;
	public float minAnimSpeed = .6f;
	public float maxAnimSpeed = 1.5f;
	public Camera nguiCamera;
	private Vector2 amountToMove;
	//How much you want the player to move
	private PlayerPhysics playerPhysics;
	//Reference to physics
	private Vector2 initialPosition;
	//Where the player start
	private GameObject floatyLocation;
	private SpriteRenderer[] spriteRendererList;
	//List of SpriteRenderers loaded by PetSpriteColorLoader
	private Color[] colorList;
	//Used by spriteRendererList to revert colors back to original
	//TODO: colorList is not actually necessary. When we reset, just go to all white instead
	private float magnetTime = 0;
	//How long until magnet is disabled
	private float magnetTimeIncrease = 5f;
	//How much extra time a magnet gives
	private IEnumerator currentColor;
	#if UNITY_EDITOR
	// used just for testing keyboard input in unity editor
	private bool bDelay = false;
	#endif

	public float Speed{
		get { return movement.currentSpeed; }
	}

	public float MinSpeed{
		get { return movement.minSpeed; }
	}

	public float MaxSpeed{
		get { return movement.maxSpeed; }
	}

	public bool StarMode{
		get { return movement.publicStarMode; }
	}

	public bool Invincible{
		get { return movement.invincible || movement.publicStarMode; }
	}

	public GameObject FloatyLocation{
		get { return floatyLocation; }
	}

	void Start(){
		playerPhysics = GetComponent<PlayerPhysics>();
		initialPosition = this.transform.position;
		floatyLocation = this.transform.Find("FloatyLocation").gameObject;
		Reset();
	}

	void Update(){
		if(RunnerGameManager.Instance.IsPaused){
			return;
		}

#if UNITY_EDITOR
		CheckKeyMovement();
#endif
		UpdateSpeed();
		CheckAndActOnDeath();
		UpdateMagnet();
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
	void FixedUpdate(){
		if(RunnerGameManager.Instance.IsPaused){
			return;
		}

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

		if(e.Position.y > Screen.height / 2){
			Jump(movement.jumpHeight);//Jump(Mathf.Clamp(touchLocationDifference,movement.minHeight,movement.maxHeight)); //In world coordinates

		}
		else{
			Drop();
		}

	}

	/// <summary>
	/// Reset player position and physics
	/// </summary>
	public void Reset(){
		transform.position = initialPosition;
		magnetTime = 0;
		MagneticField.Instance.EnableMagnet(false);
		magnetSystem.Stop();
		movement.verticalSpeed = 0f;
		movement.currentSpeed = 0f;
		anim.speed = minAnimSpeed;
		movement.ResetTargetSpeed();
		movement.Gravity = movement.targetSpeed;
		PlayAnimation();
	}

	/// <summary>
	/// Reset player speed and physics
	/// </summary>
	public void ResetSpeed(){
		anim.speed = minAnimSpeed;
		movement.ResetTargetSpeed();
		movement.Gravity = movement.targetSpeed;
	}

	/// <summary>
	/// Passes the body's renderer for coloring effects
	/// </summary>
	public void SetRenderers(SpriteRenderer[] spriteRendererList){
		this.spriteRendererList = spriteRendererList;
		colorList = new	Color[spriteRendererList.Length];
		for(int i = 0; i < colorList.Length; i++){
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
		SolidColor(c: new Color(.8f, .8f, 0, .8f));
	}

	public void StartMagnet(){ //TODO: Add some visual effect when magnet is enabled
		if(magnetTime <= 0)
			magnetTime = magnetTimeIncrease;
		else
			magnetTime += magnetTimeIncrease;
		MagneticField.Instance.EnableMagnet(true);
		magnetSystem.Play();
	}

	/// <summary>
	/// Makes the player visible.
	/// </summary>
	/// <param name="isVisible">If set to <c>true</c> is visible.</param>
	public void MakePlayerVisible(bool isVisible){
		transform.Find("Body").gameObject.SetActive(isVisible);
	}

	public void PlayAnimation(){
		anim.enabled = true;
		if(magnetTime > 0){
			magnetSystem.Play();
		}
	}

	public void PauseAnimation(){
		anim.enabled = false;
		if(magnetTime > 0){
			magnetSystem.Pause();
		}
	}
	//---------------------------------------------------
	// TriggerSlowdown()
	// Slow down the game and decrease the distance between
	// player and megahazard
	//---------------------------------------------------
	public void TriggerSlowdown(float inDivisor, string itemID){
		movement.ResetTargetSpeed();
		MegaHazard.Instance.TriggerSlowdown();
		if(DataManager.Instance.GameData.RunnerGame.RunnerItemCollided.Contains(itemID)){ //The tutorial pop up handles the flash call itself, so only activate on subsequent calls
			//StartCoroutine(FlashColor());
			FlashColor(Color.red);
		}
	}

	public void SolidColor(Color c, float time = 3f){
		if(currentColor != null){ //If there is a coroutine running right now
			StopCoroutine(currentColor);
		}
		RevertColor();
		currentColor = SolidIEnumerator(c, time);
		StartCoroutine(currentColor);
		
	}

	public void FlashColor(Color c){
		if(currentColor != null){ //If there is a coroutine running right now
			StopCoroutine(currentColor);
		}
		RevertColor();
		currentColor = FlashIEnumerator(c);
		StartCoroutine(currentColor);
	}

	private IEnumerator FlashIEnumerator(Color c){ //HACK: Both flash and solid have code extra to simply changing the color. Still, this seems better than having to pass a function to this coroutine
		movement.invincible = true;
		for(int i = 0; i < 3; i++){
			TurnColor(c);
			yield return WaitSecondsPause(.2f);

			RevertColor();
			yield return WaitSecondsPause(.3f);
		}
		movement.invincible = false;
	}

	private IEnumerator SolidIEnumerator(Color c, float time){
		movement.invincible = true;
		movement.starMode = true;
		movement.publicStarMode = true;
		TurnColor(c);
		yield return WaitSecondsPause(time);

		movement.starMode = false;
		yield return WaitSecondsPause(1f); //Give them a second to get oriented with slowdown and avoid smoke clouds

		RevertColor();
		movement.invincible = false;
		
	}

	private IEnumerator WaitSecondsPause(float time){ //Like wait for seconds, but pauses w/ RunnerGameManager
		for(float i = 0; i <= time; i += .1f){
			yield return new WaitForSeconds(.1f);
			while(RunnerGameManager.Instance.IsPaused){
				yield return 0;
			}
		}
	}

	private void TurnColor(Color c){
		for(int i = 0; i < colorList.Length; i++){
			spriteRendererList[i].color = c;
		}
	}

	private void RevertColor(){
		for(int i = 0; i < colorList.Length; i++){
			spriteRendererList[i].color = colorList[i];
		}

	}

	private void UpdateHorizontalMovement(){
		if(movement.starMode){
			movement.currentSpeed = Mathf.Lerp(movement.currentSpeed, movement.starSpeed, movement.acceleration * Time.deltaTime); 
		}
		else{
			movement.currentSpeed = Mathf.Lerp(movement.currentSpeed, movement.targetSpeed, movement.acceleration * Time.deltaTime); 
			if(!movement.starMode && movement.publicStarMode && Mathf.Abs(movement.currentSpeed / movement.targetSpeed - 1) < .1) //If we are waiting to turn of publicStarMode
				movement.publicStarMode = false;
		}
		float speedPercentage = (movement.currentSpeed - MinSpeed) / (MaxSpeed - MinSpeed);
		speedPercentage = (speedPercentage < 0) ? 0 : speedPercentage; //Make sure we are greater than 0.
		float targetAnimSpeed = minAnimSpeed + (maxAnimSpeed - minAnimSpeed) * speedPercentage; 
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

		//make sure we don't fall any faster than maxFallSpeed
		movement.verticalSpeed = Mathf.Max(movement.verticalSpeed, -movement.maxFallSpeed);
	}

	/// <summary>
	/// Calculates the jump vertical speed.
	/// </summary>
	/// <returns>The vertical speed.</returns>
	/// <param name="targetJumpHeight">Target jump height.</param>
	private float CalculateJumpVerticalSpeed(float targetJumpHeight){
		// from jump height and gravity we deduce the upwards speed for character at apex
		return Mathf.Sqrt(2 * targetJumpHeight * movement.Gravity);
	}

	//heightMultiplier multiplies movement.jumpHeight to increase speed and max heiht achieved.
	private void Jump(float height){
		bool validInput = (RunnerGameManager.Instance.AcceptInput && (!RunnerGameManager.Instance.IsPaused || (RunnerGameManager.Instance.SpecialInput && OnJump != null)));
		if(playerPhysics.Grounded && validInput){
			if(OnJump != null){
				OnJump(this, EventArgs.Empty);
			}
			AudioManager.Instance.PlayClip("runnerJumpUp");
			movement.Gravity = movement.targetSpeed;
			movement.verticalSpeed = CalculateJumpVerticalSpeed(height);
			playerPhysics.Jumping = true;
		}
	}

	private void Drop(){
		bool validInput = (RunnerGameManager.Instance.AcceptInput && (!RunnerGameManager.Instance.IsPaused || (RunnerGameManager.Instance.SpecialInput && OnDrop != null)));
		if(!validInput){
			return;
		}
		AudioManager.Instance.PlayClip("runnerJumpDown");

		if(OnDrop != null)
			OnDrop(this, EventArgs.Empty);
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
		}
		else if(gameState == MinigameStates.Playing){
			MakePlayerVisible(true);
		}
	}
	//---------------------------------------------------
	// CheckAndActOnDeath()
	// If player falls below the "Dead line" than the player dies
	//---------------------------------------------------
	private void CheckAndActOnDeath(){
		if(transform.position.y < RunnerLevelManager.Instance.LevelTooLowYValueGameOver){
			RunnerGameManager.Instance.GameOver();
		} 
	}

	private void UpdateMagnet(){
		if(magnetTime >= 0){
			magnetTime -= Time.deltaTime;
		}
		else{
			MagneticField.Instance.EnableMagnet(false);
			magnetSystem.Stop();
		}
	}
		
	
	#if UNITY_EDITOR
	//---------------------------------------------------
	// UpdateMovement()
	// Moves the player along the x axis with default speed.
	// Check for jumping and falling physics as well.
	//---------------------------------------------------
	private void CheckKeyMovement(){
		
		if(Input.GetKey("up"))
			Jump(movement.jumpHeight);
		if(Input.GetKey("down") && !playerPhysics.Falling && !bDelay){
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
