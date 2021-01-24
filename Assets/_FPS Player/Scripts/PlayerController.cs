using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status { idle, moving, crouching, sliding, climbingLadder, wallRunning, grappling, grabbedLedge, climbingLedge, vaulting, crowdControlled }

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Status status;

    [SerializeField]
    private LayerMask vaultLayer;
    [SerializeField]
    private LayerMask ledgeLayer;
    [SerializeField]
    private LayerMask ladderLayer;
    [SerializeField]
    private LayerMask wallrunLayer;

    //everything except npc and player
    int grappleMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 12 | 1 << 13 | 1 << 16 | 1 << 17 | 1 << 18;

    int layermask = 1<<18;

    GameObject vaultHelper;

    Vector3 wallNormal = Vector3.zero;
    Vector3 ladderNormal = Vector3.zero;
    Vector3 pushFrom;
    Vector3 slideDir;
    Vector3 vaultOver;
    Vector3 vaultDir;
    Vector3 ledgeNormal;
  

    public PlayerMovement movement;
    public Rigidbody rb;
    public GameObject grapplePoint;
    public SpringJoint sj;
    PlayerInput playerInput;
    AnimateLean animateLean;
    public AudioClip[] footstepClips;
    public AudioSource footstepSource;
    bool canInteract;
    bool canGrabLedge;
    bool controlledSlide;
    bool sprinting;
    public bool offGrapple;
   // [HideInInspector]
    public bool canSprint = true;

    float rayDistance;
    float slideLimit;
    float slideTime;
    float climbTime;
    float radius;
    float height;
    float halfradius;
    float grappleDistance;
    float grappleSpeed;
    public float halfheight;
    public float ledgeSpeed;


    int wallDir = 1;


    //Headbob
    private float timer = 0.0f;
    float bobbingSpeed = .05f;
    float bobbingAmount = .007f;
    float headBobAmount = .1f;
    float midpoint = .583f;
    Transform cam;
    Transform headTransform;

    private void Start()
    {
        instance = this;
        CreateVaultHelper();
        playerInput = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();

        grapplePoint = Instantiate(grapplePoint);

        if (GetComponentInChildren<AnimateLean>())
            animateLean = GetComponentInChildren<AnimateLean>();

        sj = grapplePoint.GetComponent<SpringJoint>();

        slideLimit = movement.controller.slopeLimit - .1f;
        radius = movement.controller.radius;
        height = movement.controller.height;
        halfradius = radius / 2f;
        halfheight = height / 2f;
        rayDistance = halfheight + radius + .1f;
       
        cam = Camera.main.transform;
        headTransform = GameObject.Find("CameraHolder").transform;
       // StartCoroutine(Footsteps());
    }

    /******************************* UPDATE ******************************/
    void Update()
    {
        //Debug.Log("STATUS: " + instance.status + "  " + (int)instance.status);
        //Debug.Log("CAN INTERACT: " + canInteract);
        //Updates
        UpdateInteraction();
        UpdateMovingStatus();
        UpdatePlayerManager();


       
            //Check for movement updates
            CheckSliding();
            CheckCrouching();
            CheckForWallrun();
            CheckLadderClimbing();
            UpdateLedgeGrabbing();
            CheckForVault();
            //Add new check to change status right here

        
        //Misc
        UpdateLean();

        //Headbob
        if (status == Status.moving && movement.grounded && Time.timeScale == 1 && !PlayerManager.instance.mist.isMist)
        {
            //waitForClip();
            if (sprinting)
            {
                bobbingSpeed = .185f;
                bobbingAmount = .008f;
                headBobAmount = .04f;
            } else
            {
                bobbingSpeed = .05f;
                bobbingAmount = .006f;
                headBobAmount = .02f;
            }
            float waveslice = 0.0f;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            //Debug.Log(vertical);
            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
            {
                timer = 0.0f;
                footstepSource.clip = footstepClips[0];
                footstepSource.PlayOneShot(footstepClips[0]);

                //Debug.Log("Walking");
            }
            else
            {
                waveslice = Mathf.Sin(timer);
                timer = timer + bobbingSpeed;
                if (timer > Mathf.PI * 2)
                {
                    timer = timer - (Mathf.PI * 2);
                }
            }

            if (waveslice != 0)
            {
                float translateChange = waveslice * bobbingAmount;
                float translateChange2 = waveslice * headBobAmount;
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
                translateChange = totalAxes * translateChange;
                translateChange2 = totalAxes * translateChange2;
                headTransform.localPosition = new Vector3(headTransform.localPosition.x,   translateChange2,
                    headTransform.localPosition.z);
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, midpoint + translateChange,
                    cam.transform.localPosition.z);
                
            }
            else
            {
                headTransform.localPosition = new Vector3(headTransform.localPosition.x,0,
                   headTransform.localPosition.z);
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, midpoint, cam.transform.localPosition.z);
                
            }
            
        }
        else if (!PlayerManager.instance.mist.isMist)
        {
            headTransform.localPosition = new Vector3(headTransform.localPosition.x,0,
                  headTransform.localPosition.z);
           cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(0,midpoint,0), Time.deltaTime);
            timer = 0;
           // footstepSource.clip = footstepClips[0];
            //waitForClip();
        }

    }

    void UpdateInteraction()
    {
        if (!canInteract)
        {
            if (movement.grounded || movement.moveDirection.y < 0)
                canInteract = true;
        }
        else if ((int)status >= 6)
            canInteract = false;
    }

    void UpdateMovingStatus()
    {
        if ((int)status <= 1)
        {
            status = Status.idle;
            //footstepSource.Stop();
           // footstepSource.clip = null;
            if (playerInput.input.magnitude > 0.02f)
                status = Status.moving;
          //  footstepSource.clip = footstepClips[0];
            //footstepSource.PlayOneShot(footstepClips[0]);

        }
    }

    void UpdatePlayerManager()
    {
        if(status == Status.crouching)
        {
            PlayerManager.instance.crouching = true;
        }
        else
        {
            PlayerManager.instance.crouching = false;
        }
        if (playerInput.run && canSprint && movement.grounded)
        {
            if (sprinting)
            {
                sprinting = false;
            }
            else
            {
                sprinting = true;
            }
        }
        if(playerInput.input == Vector2.zero)
        {
            sprinting = false;
        }
        if(status != Status.moving && status != Status.idle || movement.controller.isGrounded)
        {
            offGrapple = false;
        }
    }

    void UpdateLean()
    {
        if (animateLean == null) return;
        Vector2 lean = Vector2.zero;
        if (status == Status.wallRunning)
            lean.x = wallDir;
        if (status == Status.sliding && controlledSlide)
            lean.y = -1;
        else if (status == Status.grabbedLedge || status == Status.vaulting)
            lean.y = 1;
        animateLean.SetLean(lean);
    }
    /*********************************************************************/


    /******************************** MOVE *******************************/
    void FixedUpdate()
    {
        switch (status)
        {
            case Status.sliding:
                SlideMovement();
               // footstepSource.clip = footstepClips[2];
                //footstepSource.Play();
                break;
            case Status.climbingLadder:
                LadderMovement();
                break;
            case Status.grabbedLedge:
                GrabbedLedgeMovement();
                break;
            case Status.climbingLedge:
                ClimbLedgeMovement();
                break;
            case Status.wallRunning:
                WallrunningMovement();
                break;
            case Status.vaulting:
                VaultMovement();
                break;
            case Status.grappling:
                grappleMovement();

                break;
            case Status.crowdControlled:
                grappleMovement();
                break;
            default:
                DefaultMovement();
               // footstepSource.PlayOneShot(footstepClips[0]);
                break;
        }

       // StartCoroutine(Footsteps());
    }

    void DefaultMovement()
    {
        //uncrouch the player if they begin sprinting
        if (sprinting && status == Status.crouching)
            Uncrouch();

        if (offGrapple)
        {
           movement.Move(playerInput.input, 1); 
        }
        else
        {
           // footstepSource.clip = footstepClips[0];
            movement.Move(playerInput.input, sprinting, (status == Status.crouching));
           // footstepSource.Play();
        }
        if (movement.grounded && playerInput.Jump())
        {
            if (status == Status.crouching)
                Uncrouch();
            footstepSource.Stop();
            //footstepSource.clip = footstepClips[1];
           // footstepSource.Play();
            movement.Jump(Vector3.up, 1f);
            playerInput.ResetJump();
        }
    }
    /*********************************************************************/

    /****************************** SLIDING ******************************/
    void SlideMovement()
    {
        if (movement.grounded && playerInput.Jump())
        {
            if (controlledSlide)
                slideDir = transform.forward;
            movement.Jump(slideDir + Vector3.up, 1f);
            playerInput.ResetJump();
            slideTime = 0;
        }

        movement.Move(slideDir, movement.slideSpeed, 1f);
        if (slideTime <= 0)
        {
                //footstepSource.Stop();
                Crouch();
        }
        //footstepSource.PlayOneShot(footstepClips[2]);
    }

    void CheckSliding()
    {
        //Check to slide when running
        if(playerInput.crouch && canSlide())
        {
            slideDir = transform.forward;
            movement.controller.height = halfheight;
            controlledSlide = true;
            slideTime = 1f;
            footstepSource.PlayOneShot(footstepClips[2]);
        }

        //Lower slidetime
        if (slideTime > 0)
        {
            status = Status.sliding;
            slideTime -= Time.deltaTime;
        }

        if (Physics.Raycast(transform.position, -Vector3.up, out var hit, rayDistance))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > slideLimit && movement.moveDirection.y < 0)
            {
                Vector3 hitNormal = hit.normal;
                slideDir = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref slideDir);
                controlledSlide = false;
                status = Status.sliding;
            }
        }
    }

    bool canSlide()
    {
        if (!movement.grounded) return false;
        if (playerInput.input.magnitude <= 0.02f || !sprinting) return false;
        if (slideTime > 0 || status == Status.sliding) return false;
        return true;
    }
    /*********************************************************************/

    /***************************** CROUCHING *****************************/
    void CheckCrouching()
    {
        if (!movement.grounded || (int)status > 2) return;

        if(playerInput.crouch)
        {
            if (status != Status.crouching)
                Crouch();
            else
                Uncrouch();
        }
    }

    void Crouch()
    {
        //footstepSource.clip = footstepClips[3];
      
        StartCoroutine(PlayerManager.instance.StealthVignette());
        
        movement.controller.height = halfheight;
        status = Status.crouching;
         
    }

    void Uncrouch()
    {
        if (status != Status.sliding && status != Status.idle) 
        {
            StartCoroutine(PlayerManager.instance.StealthVignette());
        }

        movement.controller.height = height;
        status = Status.moving;
        //StartCoroutine(PlayerManager.instance.StealthVignette());
    }
    /*********************************************************************/

    /************************** CLAMBERING **************************/
    bool canClamber()
    {
        if (climbTime > 0 || status == Status.climbingLadder || movement.moveDirection.y <= 0 || !sprinting) return false;
        return true;
    }
    void LadderMovement()
    {
        if (climbTime > 0)
            climbTime -= Time.deltaTime;
        Vector3 input = playerInput.input;
        Vector3 move = Vector3.Cross(Vector3.up, ladderNormal).normalized;

            move *= input.x;

            move.y = movement.walkSpeed * climbTime; // need to be a speed that slowly deminishes as the clamber comes to an end

            if (playerInput.Jump())
            {
                movement.Jump((-ladderNormal + Vector3.up * 2f).normalized, 1f);
                playerInput.ResetJump();
                status = Status.moving;
                climbTime = 0;
            }

            movement.Move(move, 1f, 0f);

        if (climbTime <= 0)
        {
            status = Status.moving;
        }
    }

    void CheckLadderClimbing()
    {
//        Debug.Log(climbTime);
        
        if (!canInteract)
            return;
        //Check for ladder all across player (so they cannot use the side)
        bool right = Physics.Raycast(transform.position + (transform.right * halfradius), transform.forward, radius + 0.125f, ladderLayer);
        bool left = Physics.Raycast(transform.position - (transform.right * halfradius), transform.forward, radius + 0.125f, ladderLayer);

        if (Physics.Raycast(transform.position, transform.forward, out var hit, radius + 0.125f, ladderLayer) && right && left)
        {
            ladderNormal = -hit.normal;
            if (hasObjectInfront(0.05f, ladderLayer) && playerInput.input.y > 0.02f && canClamber())
            {
                canInteract = false;
                status = Status.climbingLadder;
                climbTime = 1f;
                Debug.Log("ladder");
            }
        }
    }
    /*********************************************************************/

    /**************************** WALLRUNNING ****************************/
    void WallrunningMovement()
    {
        if (instance.status != Status.wallRunning)
            return;

        Vector3 input = playerInput.input;
        float s = (input.y > 0) ? input.y : 0;

        Vector3 move = wallNormal * s;

        if (playerInput.Jump())
        {
            movement.Jump(((Vector3.up * (s + 1.5f)) + (wallNormal * 2f * s) + (transform.right * -wallDir * 1.25f)).normalized, (s + 0.5f) * 1.25f);
            playerInput.ResetJump();
            status = Status.moving;
        }

        if (!hasWallToSide(wallDir) || movement.grounded)
            status = Status.moving;

        movement.Move(move, movement.runSpeed *1.25f, (1f - s) + (s / 4f)/2);
    }

    void CheckForWallrun()
    {
       // if (status == Status.grappling)
         //   return;

        if (!canInteract || movement.grounded || movement.moveDirection.y >= 0)
            return;

        int wall = 0;
        if (hasWallToSide(1))
            wall = 1;
        else if (hasWallToSide(-1))
            wall = -1;

        if (wall == 0) return;

        if(Physics.Raycast(transform.position + (transform.right * wall * radius), transform.right * wall, out var hit, halfradius, wallrunLayer))
        {
            wallDir = wall;
            wallNormal = Vector3.Cross(hit.normal, Vector3.up) * -wallDir;
            if (status == Status.grappling)
            {
                StopGrapple();
            }
            status = Status.wallRunning;
        }
    }

    bool hasWallToSide(int dir)
    {
        //Check for ladder in front of player
        Vector3 top = transform.position + (transform.right * 0.25f * dir);
        Vector3 bottom = top - (transform.up * radius);
        top += (transform.up * radius);

        return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.right * dir, 0.05f, wallrunLayer).Length >= 1);
    }
    /*********************************************************************/

    /******************** LEDGE GRABBING AND CLIMBING ********************/
    void GrabbedLedgeMovement()
    {
        if (playerInput.Jump())
        {
            movement.Jump((Vector3.up + transform.forward).normalized, 1f);
            playerInput.ResetJump();
            status = Status.moving;
        }
        if (playerInput.input.x > 0)
        {
            movement.Move(new Vector3(ledgeNormal.z,0,ledgeNormal.x), movement.crouchSpeed / 2, 0f); //inch to the right
        }
        if (playerInput.input.x < 0)
        {
            movement.Move(-new Vector3(ledgeNormal.z, 0, ledgeNormal.x), movement.crouchSpeed / 2, 0f); //inch to the right
        }
        else
        {
            movement.Move(Vector3.zero, 0f, 0f); //Stay in place
        }
    }

    void ClimbLedgeMovement()
    {
        Vector3 dir = pushFrom - transform.position;
        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 move = Vector3.Cross(dir, right).normalized;

        movement.Move(move, movement.walkSpeed * ledgeSpeed, 0f);
        if (new Vector2(dir.x, dir.z).magnitude < 0.125f)
            status = Status.idle;
    }

    void CheckLedgeGrab()
    {
        //Check for ledge to grab onto 
        Vector3 dir = transform.TransformDirection(new Vector3(0, -0.5f, 1).normalized);
        Vector3 pos = transform.position + (Vector3.up * height / 3f) + (transform.forward * radius / 2f);
       
        bool right = Physics.Raycast(pos + (transform.right * radius / 2f), dir, radius + 0.125f, ledgeLayer);
        bool left = Physics.Raycast(pos - (transform.right * radius / 2f), dir, radius + 0.125f, ledgeLayer);
        Debug.DrawRay(pos + (transform.right * radius / 2f), dir, Color.red, radius + 0.125f);
        Debug.DrawRay(pos - (transform.right * radius / 2f), dir, Color.red, radius + 0.125f);
        if (Physics.Raycast(pos, dir, out var hit, radius + 0.125f, ledgeLayer) && right && left)
        {
            Vector3 rotatePos = transform.InverseTransformPoint(hit.point);
            rotatePos.x = 0; rotatePos.z = 1;
            pushFrom = transform.position + transform.TransformDirection(rotatePos); //grab the position with local z = 1
            rotatePos.z = radius * 2f;

            Vector3 checkCollisions = transform.position + transform.TransformDirection(rotatePos); //grab it closer now

            //Check if you would be able to stand on the ledge
            if (!Physics.SphereCast(checkCollisions, radius, Vector3.up, out hit, height - radius))
            {
                canInteract = false;
                status = Status.grabbedLedge;
                
            }
        }
    }

    void UpdateLedgeGrabbing()
    {
        if (status == Status.grappling)
            return;

        if (movement.grounded || movement.moveDirection.y > 0)
            canGrabLedge = true;

        if (status != Status.climbingLedge)
        {
            if (canGrabLedge && !movement.grounded)
            {
                if (movement.moveDirection.y < 0)
                    CheckLedgeGrab();
            }

            if (status == Status.grabbedLedge)
            {
                canGrabLedge = false;
                Vector2 down = playerInput.down;
                if (down.y == -1)
                    status = Status.moving;
                else if (down.y == 1)
                    status = Status.climbingLedge;
            }
        }
    }
    /*********************************************************************/

    /***************************** VAULTING ******************************/
    void VaultMovement()
    {
        Vector3 dir = vaultOver - transform.position;
        Vector3 localPos = vaultHelper.transform.InverseTransformPoint(transform.position);
        Vector3 move = (vaultDir + (Vector3.up * -(localPos.z - radius) * height)).normalized;

        if(localPos.z > halfheight)
        {
            movement.controller.height = height;
            status = Status.moving;
        }

        movement.Move(move, movement.runSpeed, 0f);
    }

    void CheckForVault()
    {
        if (status == Status.vaulting) return;

        float checkDis = 0.05f;
        checkDis += (movement.controller.velocity.magnitude / 16f); //Check farther if moving faster
        if(hasObjectInfront(checkDis, vaultLayer) && playerInput.Jump())
        {
            if (Physics.SphereCast(transform.position + (transform.forward * (radius - 0.25f)), 0.25f, transform.forward, out var sphereHit, checkDis, vaultLayer))
            {
                if (Physics.SphereCast(sphereHit.point + (Vector3.up * halfheight), radius, Vector3.down, out var hit, halfheight - radius, vaultLayer))
                {
                    //Check above the point to make sure the player can fit
                    if (Physics.SphereCast(hit.point + (Vector3.up * radius), radius, Vector3.up, out var trash, height-radius))
                        return; //If cannot fit the player then do not vault

                    vaultOver = hit.point;
                    vaultDir = transform.forward;
                    SetVaultHelper();

                    canInteract = false;
                    status = Status.vaulting;
                    movement.controller.height = radius;
                }
            }
        }
    }

    void CreateVaultHelper()
    {
        vaultHelper = new GameObject();
        vaultHelper.transform.name = "(IGNORE) Vault Helper";
    }

    void SetVaultHelper()
    {
        vaultHelper.transform.position = vaultOver;
        vaultHelper.transform.rotation = Quaternion.LookRotation(vaultDir);
    }
    /*********************************************************************/

    /***************************** GRAPPLING ******************************/
    public Pendulum pendulum;
    Vector3 tetherPoint;
    Vector3 dirToTether;
    Vector3 previousPos;
    float grappleArmLength;
    float grappleRange = 40f;

    void grappleMovement()
    {

        sj.maxDistance -= grappleSpeed;
        grappleDistance = Vector3.Distance(transform.position, grapplePoint.transform.position);

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * 20f, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * 20f, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * 20f, ForceMode.Force);
        }

        if(instance.status != Status.grappling && instance.status != Status.crowdControlled)
        {
            StopGrapple();
        } else if (grappleDistance < 2.5f)
        {
            StopGrapple();
        }
        if (grappleSpeed < .275f)
        {
            grappleSpeed += .0125f;
        }
    }

    public void SetUpGrapple()
    {
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, grappleRange, grappleMask))
        {
            Debug.Log(hit.transform.gameObject.name);
            Debug.Log(grappleArmLength);
            tetherPoint = hit.point;
            instance.status = Status.grappling;
            grapplePoint.transform.position = tetherPoint;
            if (rb != null)
                Destroy(rb);
            rb = gameObject.AddComponent<Rigidbody>();
            rb.velocity = movement.controller.velocity;
            sj.connectedBody = rb;
            sj.maxDistance = Vector3.Distance(transform.position, grapplePoint.transform.position)-1f;
            grappleDistance = Vector3.Distance(transform.position, grapplePoint.transform.position);
            grappleSpeed = .1f;
            StartCoroutine(ThrowGrapple(hit.point));
            movement.grounded = false;
            movement.controller.enabled = false;
            canInteract = false;
            footstepSource.PlayOneShot(footstepClips[1]);
            
        }
    }
    IEnumerator ThrowGrapple(Vector3 hitPoint)
    {
        PowerSwap.instance.ShadowHand.transform.parent = null;
        float t = 0;
        Vector3 startPoint = PowerSwap.instance.ShadowHand.transform.position;
        while (t < 1)
        {
            PowerSwap.instance.ShadowHand.transform.position = Vector3.LerpUnclamped(startPoint, hitPoint, 
                PlayerManager.instance.linearCurve.Evaluate(t));
            t += Time.deltaTime * 5;
            yield return 0;
        }

    }

    public void StopGrapple()
    {
        //movement.moveDirection = rb.velocity;
        movement.controller.enabled = true;
        movement.moveDirection = rb.velocity;
        canInteract = true;
        status = Status.moving;
        offGrapple = true;
        sj.connectedBody = null;
        StartCoroutine(ReturnGrapple());
        Destroy(rb);
    }
    IEnumerator ReturnGrapple()
    {
        PowerSwap.instance.ShadowHand.transform.parent = PowerSwap.instance.shadowParent;
      float t = 0;
        Vector3 startPoint = PowerSwap.instance.ShadowHand.transform.localPosition;
        Vector3 startRot = PowerSwap.instance.ShadowHand.transform.eulerAngles;
        while (t < 1)
        {
            PowerSwap.instance.ShadowHand.transform.localPosition = Vector3.LerpUnclamped(startPoint, PowerSwap.instance.ShadowHandStartPos,
                PlayerManager.instance.linearCurve.Evaluate(t));
            PowerSwap.instance.ShadowHand.transform.localEulerAngles = Vector3.LerpUnclamped(startRot, 
                PlayerManager.instance.leftHand.transform.localEulerAngles, PlayerManager.instance.linearCurve.Evaluate(t));
            t += Time.deltaTime * 5;
            yield return 0;
        }
        PowerSwap.instance.ShadowHand.transform.localPosition = PowerSwap.instance.ShadowHandStartPos;
        PowerSwap.instance.ShadowHand.transform.localEulerAngles = PlayerManager.instance.leftHand.transform.localEulerAngles;
    }

        /**************************Knocked*Back********************************/

        public void GetKnockedBack(Transform knockBackSource)
    {
        Vector3 Dir = (transform.position - knockBackSource.position).normalized;
        if (rb != null)
            Destroy(rb);
        rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(new Vector3(Dir.x,.65f,Dir.y)*12.5f, ForceMode.Impulse);
        status = Status.crowdControlled;
        movement.controller.enabled = false;
        canInteract = false;
        movement.grounded = false;
        StartCoroutine(EndKnockBack());
    }

    IEnumerator EndKnockBack()
    {
        yield return 0;
        StopGrapple();
    }


    /*********************************************************************/

    bool hasObjectInfront(float dis, LayerMask layer)
    {
        Vector3 top = transform.position + (transform.forward * 0.25f);
        Vector3 bottom = top - (transform.up * halfheight);

        return (Physics.CapsuleCastAll(top, bottom, 0.25f, transform.forward, dis, layer).Length >= 1);
    }
    void waitForClip()
    {
        AudioClip clipToWait = footstepSource.clip;
        float timer;
        footstepSource.Play();
        if (footstepSource.isPlaying)
        {
            footstepSource.Stop();
            timer = clipToWait.length;
            timer = timer - Time.deltaTime;
            if (timer == 0)
            {
                footstepSource.Play();
            }

        }

        if (!footstepSource.isPlaying)
        {
            footstepSource.Play();
        }
    }



    IEnumerator Footsteps()
    {
        while (status != null)
        {
            if (status == Status.moving)
            {
                footstepSource.clip = footstepClips[0];
                footstepSource.PlayOneShot(footstepClips[0]);
                yield return new WaitForSeconds(.5f);
                // yield break;
            }

            if (status == Status.crouching)
            {
                footstepSource.clip = footstepClips[0];
                footstepSource.PlayOneShot(footstepClips[0]);
                yield return new WaitForSeconds(1.8f);
                //  yield break;
            }

            if (status == Status.idle)
            {
                footstepSource.clip = null;
                yield return new WaitForSeconds(.3f);
                // yield break;

            }

            if (status == Status.sliding)
            {
                footstepSource.clip = footstepClips[2];
                footstepSource.PlayOneShot(footstepClips[2]);
                yield return new WaitForSeconds(5f);
                // yield break;
            }

            //yield break;
        }
    }


}
