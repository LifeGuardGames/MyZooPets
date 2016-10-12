public class ShooterEnemyBasic : ShooterEnemy{
	// basic ai just handles moving to the left and assigning values
	void Start(){
		LeanTween.moveX(gameObject, player.transform.position.x, moveDuration);
	}
}
