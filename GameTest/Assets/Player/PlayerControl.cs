using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public CharacterController selfplayer;
    [Header("Movement of player")]
    public float speed;
    public float gravity;
    public float jumpHeight;
   public Vector3 gravityF;
    bool OnGround = false;
    public Transform spherePosition;
    public float sphereRadius;
    public LayerMask groundlayer;
    [Header("CameraHead Control")]
    public Transform CamHeadParent;
    public Camera cam;
    float X, Y;
    public float rotationSpeed, lerpSpeed;
    public Transform CamHeadPosition;
    // Start is called before the first frame update
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
    }
    private void FixedUpdate()
    {
      
    }

    void MovePlayer()
    {
     
        if(OnGround && gravityF.y < 0)
        {
            gravityF.y = -5;
        }
        

        Vector3 move = selfplayer.transform.forward * Input.GetAxis("Vertical") + selfplayer.transform.right * Input.GetAxis("Horizontal");
        selfplayer.Move(move *speed* Time.deltaTime);
    
        if (OnGround)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                gravityF.y = jumpHeight;
            }
        }
        
        gravityF.y -= gravity * Time.deltaTime;
        selfplayer.Move(gravityF * Time.deltaTime);
        
    }

    void ControlCamHead()
    {
        CamHeadParent.transform.position = CamHeadPosition.transform.position;
        X += Input.GetAxis("Mouse X") * rotationSpeed*10f * Time.deltaTime;
        Y -= Input.GetAxis("Mouse Y") * rotationSpeed*10f * Time.deltaTime;
        Quaternion rot = Quaternion.Euler(new Vector3(Y, X, 0f));
  //      Vector3 target = Quaternion.Lerp(target, rot, lerpSpeed * Time.deltaTime);
        CamHeadParent.transform.rotation = Quaternion.Lerp(CamHeadParent.transform.rotation, rot, lerpSpeed * Time.deltaTime);
        selfplayer.transform.rotation = Quaternion.Euler(0f, CamHeadParent.transform.eulerAngles.y, 0f);
    }
    bool CheckGround()
    {
        OnGround = Physics.CheckSphere(spherePosition.transform.position, sphereRadius, groundlayer);
       
        return OnGround;
    }
}
