using System.Collections;
using System.Collections.Generic;
using Debugging;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public CharacterController selfplayer;
    [Header("Movement of player")]
    public float speed;
    public float gravity;
    public float jumpHeight;
    public Vector3 gravityF;
    bool OnGround = false;
    Vector3 move;
    public Transform spherePosition;
    public float sphereRadius;
    public LayerMask groundlayer;

    public Vector4 StartColor, AffectedColor;
    [Header("CameraHead Control")]
    public Transform CamHeadParent;
    public Camera cam;
    float X, Y;
    public float rotationSpeed;
    public float lerpSpeed;
    public Transform CamHeadPosition;
    public DebuggingScreen debuggingScreen;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.fixedDeltaTime = 1 / 100f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        MovePlayer();
        ControlCamHead();
        Run();
        AdjustFOWCam();
    }
    private void FixedUpdate()
    {

    }

    void MovePlayer()
    {

        if (OnGround && gravityF.y < 0)
        {
            gravityF.y = -5;
        }


        move = selfplayer.transform.forward * Input.GetAxis("Vertical") + selfplayer.transform.right * Input.GetAxis("Horizontal");
        selfplayer.Move(move * speed * Time.deltaTime);

        if (OnGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gravityF.y = jumpHeight;
            }
        }

        gravityF.y -= gravity * Time.deltaTime;
        selfplayer.Move(gravityF * Time.deltaTime);

        // debuggingScreen.text.text = selfplayer.transform.position.ToString();
    }

    void ControlCamHead()
    {
        CamHeadParent.transform.position = CamHeadPosition.transform.position;
        X += Input.GetAxis("Mouse X") * rotationSpeed * 10f * Time.deltaTime;
        Y -= Input.GetAxis("Mouse Y") * rotationSpeed * 10f * Time.deltaTime;
        Y = Mathf.Clamp(Y, -70f, 70f);
        Quaternion rot = Quaternion.Euler(new Vector3(Y, X, 0f));
        CamHeadParent.transform.rotation = Quaternion.Lerp(CamHeadParent.transform.rotation, rot, lerpSpeed * Time.deltaTime);
        selfplayer.transform.rotation = Quaternion.Euler(0f, CamHeadParent.transform.eulerAngles.y, 0f);
    }
    bool CheckGround()
    {
        OnGround = Physics.CheckSphere(spherePosition.transform.position, sphereRadius, groundlayer);

        return OnGround;
    }

    bool canRun = false;
    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && move != Vector3.zero)
        {
            canRun = true;
            speed = 3.5f;
        }
        else
        {
            canRun = false;
            speed = 1.5f;
        }
    }

    float fow;
    void AdjustFOWCam()
    {
        if (canRun)
        {
            fow = Mathf.Lerp(fow, 70f, 8f * Time.deltaTime);
        }
        else
        {
            fow = Mathf.Lerp(fow, 60f, 8f * Time.deltaTime);
        }
        cam.fieldOfView = fow;
    }
}
