using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class Truck_controller : MonoBehaviour
{
    public GameObject body;
    public Rigidbody back_right;
    public Rigidbody back_left;
    public Rigidbody front_right;
    public Rigidbody front_left;
    public float maxDistance = 50.0f;
    public int layerMask = 1 << 8;
    public float elapsed = 0f;
    public int turningMode = 0;

    void Start()
    {
        body = GameObject.FindWithTag("Player");
        back_left = GameObject.FindWithTag("back_left").GetComponent<Rigidbody>();
        back_right = GameObject.FindWithTag("back_right").GetComponent<Rigidbody>();
        front_left = GameObject.FindWithTag("front_left").GetComponent<Rigidbody>();
        front_right = GameObject.FindWithTag("front_right").GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        RaycastHit center = new RaycastHit();
        RaycastHit left = new RaycastHit();
        RaycastHit right = new RaycastHit();
        RaycastHit centerLeft = new RaycastHit();
        RaycastHit centerRight = new RaycastHit();

        var offset = new Vector3(0f, 0.5f, 0.4f);
        float centerAngle = 0.0f;
        float leftAngle = -60.0f;
        float rightAngle = 60.0f;
        float centerLeftAngle = -30.0f;
        float centerRightAngle = 30.0f;

        float CDistance = RaycastDraw(offset, centerAngle, maxDistance, center);
        float LDistance = RaycastDraw(offset, leftAngle, maxDistance, left);
        float RDistance = RaycastDraw(offset, rightAngle, maxDistance, right);
        float CLDistance = RaycastDraw(offset, centerLeftAngle, maxDistance, centerLeft);
        float CRDistance = RaycastDraw(offset, centerRightAngle, maxDistance, centerRight);


        elapsed += Time.deltaTime;
        if (elapsed >= 5f)
        {
            elapsed = elapsed % 5f;
            turningMode = ChangeMode();
            OutputTime();
        }
        AvoidObstacles(CDistance, LDistance, RDistance, CLDistance, CRDistance, turningMode);
    }


    void OutputTime()
    {
        Debug.Log(Time.time);
    }

    public float RaycastDraw(Vector3 offset, float angle, float maxDistance, RaycastHit sensor)
    {
        float distanceInfo = 0;
        if (Physics.Raycast(transform.TransformPoint(offset), Quaternion.AngleAxis(angle, transform.up) * transform.forward, out sensor, maxDistance, layerMask))
        {
            distanceInfo = sensor.distance;
            Debug.DrawRay(transform.TransformPoint(offset), Quaternion.AngleAxis(angle, transform.up) * transform.forward * 50.0f, Color.magenta);
        }
        else
        {
            Debug.DrawRay(transform.TransformPoint(offset), Quaternion.AngleAxis(angle, transform.up) * transform.forward * 50.0f, Color.white);
        }
        return ((distanceInfo / maxDistance));
    }


    public void AvoidObstacles(float centerD, float leftD, float rightD, float centerLeftD, float centerRightD, int turningMode)
    {
        Debug.Log(turningMode);
        float[] weightArr = new float[] { };
        weightArr = ChooseTurningWeights(turningMode);

        front_left.velocity = body.transform.forward * (20f + leftD * weightArr[0] + centerLeftD * weightArr[1] + centerD * weightArr[2] + rightD * weightArr[3] + centerRightD * weightArr[4]);
        back_left.velocity = body.transform.forward * (20f + leftD * weightArr[0] + centerLeftD * weightArr[1] + centerD * weightArr[2] + rightD * weightArr[3] + centerRightD * weightArr[4]);
        front_right.velocity = body.transform.forward * (20f + leftD * weightArr[5] + centerLeftD * weightArr[6] + centerD * weightArr[7] + rightD * weightArr[8] + centerRightD * weightArr[9]);
        back_right.velocity = body.transform.forward * (20f + leftD * weightArr[5] + centerLeftD * weightArr[6] + centerD * weightArr[7] + rightD * weightArr[8] + centerRightD * weightArr[9]);

    }

    public int ChangeMode()
    {
        Random rnd = new Random();
        int changedMode = rnd.Next(0, 10);
        return changedMode;
    }

    public float[] ChooseTurningWeights(int turningMode)
    {
        float[] weightArray = new float[] { };
        float[] rightTurning = new float[] { 50f, 50f, 50f, 0, 0, 0, 0, 0, -50f, -50f };
        float[] leftTurning = new float[] { -50f, -50f, 0f, 0, 0, 0, 0, 50f, 50f, 50f };
        if (turningMode % 2 == 0)
        {
            weightArray = rightTurning;
        }
        if (turningMode % 2 == 1)
        {
            weightArray = leftTurning;
        }
        return weightArray;
    }

    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            front_left.velocity = body.transform.forward * 20f;
            front_right.velocity = body.transform.forward * 20f;
            back_left.velocity = body.transform.forward * 20f;
            back_right.velocity = body.transform.forward * 20f;
            Debug.Log("Up Key pressed");
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            front_left.velocity = -body.transform.forward * 20f;
            front_right.velocity = -body.transform.forward * 20f;
            back_left.velocity = -body.transform.forward * 20f;
            back_right.velocity = -body.transform.forward * 20f;
            Debug.Log("Down Key pressed");
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            back_left.velocity = body.transform.forward * 10f;
            back_right.velocity = body.transform.forward * 20f;
            Debug.Log("Left Key pressed");
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            back_left.velocity = body.transform.forward * 20f;
            back_right.velocity = body.transform.forward * 10f;
            Debug.Log("Right Key pressed");
        }
    }
}
