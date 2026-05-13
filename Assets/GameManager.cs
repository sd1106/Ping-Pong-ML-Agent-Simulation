using UnityEngine;

public class GameManager : MonoBehaviour
{
    float resetTimer = 0.0f;
    //This file will likely be replaced by a GameManager
    //The manager will probably handle all of the resets for game objects and the score for the game. 
    public GameObject paddleLeft;
    public GameObject paddleRight;
    public GameObject ball;

    // Start is called before the first frame update
    private void Start()
    {
        
    }
    private void Update()
    {
        resetTimer += Time.deltaTime;
        if (resetTimer > 120)
        {
            ResetAllGameObjects(); //Just to prevent the game from going on forever
            resetTimer = 0;
        }
    }

    public void ResetAllGameObjects()
    {
        paddleLeft.GetComponent<PaddleControl>().ResetAll();
        paddleRight.GetComponent<PaddleControl>().ResetAll();
        ball.GetComponent<BallMovement>().ResetBall();
        Debug.Log("RESET");
    }

    public void OnBallHitRightWall()
    {
        paddleRight.GetComponent<PaddleControl>().OnBallMissed();
        paddleLeft.GetComponent<PaddleControl>().AddReward(0.5f);
        ball.GetComponent<BallMovement>().ResetBall();
        resetTimer = 0;
    }

    public void OnBallHitLeftWall()
    {
        paddleLeft.GetComponent<PaddleControl>().OnBallMissed();
        paddleRight.GetComponent<PaddleControl>().AddReward(0.5f);
        ball.GetComponent<BallMovement>().ResetBall();
        resetTimer = 0;
    }
}