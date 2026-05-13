using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField]
    GameObject manager;
    Rigidbody rb;
    float speedIncrease = 6.0f;
    float changeSpeed = 1.05f;
    float directionChange = .05f;
    double speedIncreaseTimer = 0.0;
    Vector3 direction;

    //Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ResetBall();
    }

    public Vector3 ReturnDirection()
    {
        return direction;
    }

    public void IncreaseSpeed()
    {
        if (Vector3.Magnitude(direction) < 7.0) // speed limit to make sure we don't go out of bounds
            direction *= changeSpeed;
    }

    public void ChangeAngle()
    {
        if (Vector3.Magnitude(direction) < 7.0)
        {
            float newZ = direction.z * (1 + directionChange);
            direction = new Vector3(direction.x, 0, newZ);
        }
    }

    public void ResetBall()
    {
        //Reset all values
        rb.position = new Vector3(0, 0, Random.Range(-10, 10));
        transform.rotation = Quaternion.identity;
        speedIncrease = 6.0f;
        changeSpeed = 1.05f;
        directionChange = .05f;
        speedIncreaseTimer = 0.0;


        float xDirection = Random.Range(0.1f, 1.0f);
        if (Random.Range(0, 2) == 0)
            xDirection *= -1;
        float zDirection = Random.Range(0.1f, 1.0f);
        if (Random.Range(0, 2) == 0)
            zDirection *= -1;
        direction = new Vector3(xDirection, 0, zDirection).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speedIncrease * Time.fixedDeltaTime);
    }

    private void Update()
    {
        speedIncreaseTimer += Time.deltaTime;
        if (speedIncreaseTimer > 3)
        {
            speedIncreaseTimer = 0;
            IncreaseSpeed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TopBotWall"))
        {
            direction = Vector3.Reflect(direction, new Vector3(0, 0, 1));
        }
        else if (other.gameObject.CompareTag("Paddle"))
        {
            direction = Vector3.Reflect(direction, new Vector3(1, 0, 0));
        }
        else if (other.gameObject.CompareTag("RightWall"))
        {
            manager.GetComponent<GameManager>().OnBallHitRightWall();
        }
        else if (other.gameObject.CompareTag("LeftWall"))
        {
            manager.GetComponent<GameManager>().OnBallHitLeftWall();
        }
    }
}