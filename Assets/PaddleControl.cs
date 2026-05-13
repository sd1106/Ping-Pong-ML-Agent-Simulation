using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PaddleControl : Agent
{
    public Transform ball;
    public Rigidbody rb;
    private float moveSpeed = 650f;
    private BallMovement ballMovement;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        ballMovement = ball.GetComponent<BallMovement>();
    }

    public override void OnEpisodeBegin()
    {
        if (gameObject.name == "Right Player")
            transform.localPosition = new Vector3(25, 0, 0);
        if (gameObject.name == "Left Player")
            transform.localPosition = new Vector3(-25, 0, 0);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearDamping = 1f;
    }

    private float PredictBallZ()
    {
        Vector3 ballPos = ball.localPosition;
        Vector3 ballDir = ballMovement.ReturnDirection();

        if (gameObject.name == "Right Player" && ballDir.x <= 0) return ballPos.z;
        if (gameObject.name == "Left Player" && ballDir.x >= 0) return ballPos.z;
        if (Mathf.Abs(ballDir.x) < 0.001f) return ballPos.z;

        float distX = transform.localPosition.x - ballPos.x;
        float travelRatio = distX / ballDir.x;
        float rawZ = ballPos.z + ballDir.z * travelRatio;

        return SimulateBounce(rawZ, 10f);
    }

    private float SimulateBounce(float rawZ, float halfHeight)
    {
        float range = halfHeight * 2f;
        rawZ -= -halfHeight;
        rawZ = Mathf.Abs(rawZ);
        float mod = rawZ % (range * 2f);
        if (mod > range)
            mod = range * 2f - mod;
        return mod - halfHeight;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 ballDir = ballMovement.ReturnDirection();
        float speed = Mathf.Max(ballDir.magnitude, 0.001f);
        float predictedZ = PredictBallZ();

        sensor.AddObservation(transform.localPosition.z / 10f);
        sensor.AddObservation(ball.localPosition.x / 25f);
        sensor.AddObservation(ball.localPosition.z / 10f);
        sensor.AddObservation(ballDir.x / speed);
        sensor.AddObservation(ballDir.z / speed);
        sensor.AddObservation(speed / 7f);
        sensor.AddObservation(predictedZ / 10f);
        sensor.AddObservation((predictedZ - transform.localPosition.z) / 10f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = actions.ContinuousActions[0];
        rb.AddForce(new Vector3(0, 0, moveZ) * moveSpeed * Time.deltaTime);
        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.ContinuousActions.Array[0] = Input.GetAxis("Vertical"); //W/S or Up/Down Arrows
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            AddReward(1.0f);
            ballMovement.ChangeAngle();
            Debug.Log(gameObject.name + " hit ball! +1 reward");
        }
    }

    public void OnBallMissed()
    {
        AddReward(-2.0f);
        Debug.Log(gameObject.name + " missed! -2 reward");
        EndEpisode();
    }

    public void ResetAll()
    {
        if (gameObject.name == "Right Player")
            transform.localPosition = new Vector3(25, 0, 0);
        if (gameObject.name == "Left Player")
            transform.localPosition = new Vector3(-25, 0, 0);
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearDamping = 1f;
        EndEpisode();
    }
}