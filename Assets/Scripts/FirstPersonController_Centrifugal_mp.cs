using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement; // neded in order to load scenes
using UnityEngine.Networking;

namespace UnityStandardAssets.Characters.FirstPerson
{
    //[RequireComponent(typeof (CharacterController))]
    public class FirstPersonController_Centrifugal_mp : NetworkBehaviour
    {
		[SerializeField] private bool m_IsWalking;
		[SerializeField] private bool m_IsCrouching;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook_Centrifugal m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Transform reference_frame;

        public Vector3 current_speed;
        public Vector3 current_forces;
        public bool freeze_velocity;
        public float radius;
        public float jump_force;

        public bool is_paused=false;
        private Vector3 translation;

        public Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private bool m_Jumping;
		private bool m_LightOn = false;
		private bool m_cantStand;
		private bool m_willStand;
        private Vector3 Gravity_analog;

        //http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html

        private bool isGroundedCustom;
        private Vector3 myNormal;
        private Vector3 surfaceNormal;
        private float distGround;
        private float deltaGround = 0.2f;
        private CapsuleCollider myCollider;

        private float moveSpeed = 6; // move speed
        private float turnSpeed = 90; // turning speed (degrees/second)
        private float lerpSpeed = 10; // smoothing speed

        private Transform myTransform;
        private Vector3 rotation;

        // Use this for initialization
        private void Start()
        {
            m_Camera = transform.Find("FirstPersonCharacter").GetComponent<Camera>();

            reference_frame = GameObject.Find("Inertial_Reference_Frame").transform;

            //disable everyone's camera and audio listener
            //they will be enabled ONLY FOR THE CLIENT'S PLAYER MODEL below
            m_Camera.enabled = false;
            m_Camera.gameObject.GetComponent<AudioListener>().enabled = false;


            myCollider = GetComponent<CapsuleCollider>();
            myNormal = transform.up;
            distGround = myCollider.bounds.extents.y - myCollider.center.y;
            myTransform = transform;

            //reference_frame = transform.parent.transform;

            //m_CharacterController = GetComponent<CharacterController>();
            //m_Camera = Camera.main;

            if (isLocalPlayer)
            {
                //enable client's camera & audio listener
                m_Camera.enabled = true;
                m_Camera.gameObject.GetComponent<AudioListener>().enabled = true;
                //hide visor on self
                transform.Find("Visor").GetComponent<Renderer>().enabled = false;

                GameObject[] g = GameObject.FindGameObjectsWithTag("terminal");
                for (int i = 0; i<g.Length; i++)
                {
                    g[i].GetComponent<mp_TerminalMenuController>().player_camera = m_Camera;
                    g[i].GetComponent<Canvas>().worldCamera = m_Camera;

                }
                
            

                m_OriginalCameraPosition = m_Camera.transform.localPosition;
                m_FovKick.Setup(m_Camera);
                m_HeadBob.Setup(m_Camera, 0);
                m_Jumping = false;
                m_MouseLook.Init(transform , m_Camera.transform);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Gravity_analog = Physics.gravity;

                //Pause = false;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false;
                }

                if(reference_frame != null)
                {
                    transform.SetParent(reference_frame.transform);
                }


                GameObject c = GameObject.Find("SkyboxCamera");
                if (c != null)
                {
                    c.GetComponent<SkyboxCamera>().MainCamera = m_Camera;
                }

            }

        }

        //look at
        // https://forum.unity3d.com/threads/make-an-object-invisible-to-other-players-in-network.97924/#post-639557
        // on how to make visor invisible to self

        // Update is called once per frame
        private void Update()
        {
            //Add a check for isLocalPlayer in the Update function, so that only the local player processes input.
            if (!isLocalPlayer)
            {
                return;
            }
            current_speed = GetComponent<Rigidbody>().velocity;
            current_forces = GetComponent<Rigidbody>().inertiaTensor;
            /*
            if (Input.GetButtonUp("MainMenu"))
            {
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                }
                SceneManager.LoadScene("MainMenu");
            }
            if (Input.GetButtonUp("Cancel"))
            {
                if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f; //this makes everything transparent
                        canvasGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
                    }
                }
                else
                {
                    Time.timeScale = 1;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 0f;
                        canvasGroup.blocksRaycasts = false;
                    }
                }
            //Pause = !Pause;
            // ^^^^ Disabled so that one player can't freeze the entire server
            //   more reasonable pause controls below
        }*/

            if (Input.GetButtonUp("Cancel"))
            {
                is_paused = !is_paused;
            }


            if (is_paused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }


            if (reference_frame != null) {
                rotation = new Vector3(-90 - Mathf.Atan2((transform.localPosition.y - reference_frame.position.y), (transform.localPosition.z - reference_frame.position.z)) * Mathf.Rad2Deg, 0, 0);
                rotation.x = Mathf.Repeat(rotation.x, 360f);
            }
            else
            {
                GetComponent<Rigidbody>().freezeRotation = true;
            }
            //v this allows camera motion via mouse
            if (!is_paused)
            {
                RotateView();
            }
            //TO DO LIST
            // reenable + debug additional functionality of script
            // add coriolis (i.e. disble forces and parentage when object/player not on ground)

            //to stop drifting when no input
            /*
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 || freeze_velocity)
            {
                Vector3 oldvelocity = GetComponent<Rigidbody>().velocity;
                GetComponent<Rigidbody>().velocity = new Vector3(0, oldvelocity.y, 0);
            }*/
            
            Ray ray;
            RaycastHit hit;

            ray = new Ray(transform.position, -myNormal);

            if (Physics.Raycast(ray, out hit))
            { // use it to update myNormal and isGrounded
                isGroundedCustom = hit.distance <= distGround + deltaGround;
                surfaceNormal = hit.normal;
            }
            else
            {
                isGroundedCustom = false;
                // assume usual ground normal to avoid "falling forever"
                surfaceNormal = Vector3.up;
            }
            myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);
            // find forward direction with new myNormal:
            Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
            Quaternion targetRot = Quaternion.LookRotation(myForward, myNormal);
            //myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed * Time.deltaTime);
            // move the character forth/back with Vertical axis:
            GetInput(out moveSpeed);
            //translation = new Vector3 (Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            translation = new Vector3 (Input.GetAxis("Horizontal") * moveSpeed, 0, Input.GetAxis("Vertical") * moveSpeed);
            translation = myTransform.TransformDirection(translation);
            //Debug.Log(moveSpeed);
            //myTransform.Translate(translation);
            //Debug.DrawLine(transform.position, (transform.position + translation), Color.red);
            //^move to fixed update?

            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump && isGroundedCustom)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            
            if (!m_PreviouslyGrounded && isGroundedCustom)
            {
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }/*
            if (!isGroundedCustom && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }
            */
            m_PreviouslyGrounded = isGroundedCustom;

            /*
			if (Input.GetButtonDown ("Crouch") && !m_IsCrouching) 
			{
				gameObject.transform.localScale = new Vector3(1, .5F, 1);
				m_WalkSpeed = 2.5f;
				m_IsWalking = true;
				m_IsCrouching = true;
			}

			if (Input.GetButtonUp ("Crouch") && m_IsCrouching && m_cantStand) {
				m_willStand = true;
			}

			if (m_willStand && !m_cantStand)
			{
				gameObject.transform.localScale = new Vector3 (1, 1, 1);
				m_WalkSpeed = 5;
				m_IsCrouching = false;
				m_willStand = false;
			}

			if (Input.GetButtonUp ("Crouch") && m_IsCrouching && !m_cantStand) {
				gameObject.transform.localScale = new Vector3 (1, 1, 1);
				m_WalkSpeed = 5;
				m_IsCrouching = false;
			}


			if (Input.GetButtonUp ("Flashlight") && !m_LightOn) {
				gameObject.transform.GetChild (0).gameObject.transform.GetChild (1).gameObject.GetComponent<Light> ().intensity = 2.5f;
				m_LightOn = true;
			}

			else if (Input.GetButtonUp ("Flashlight") && m_LightOn) {
				gameObject.transform.GetChild (0).gameObject.transform.GetChild (1).gameObject.GetComponent<Light> ().intensity = 0f;
				m_LightOn = false;
			}
            */
        }


        private void FixedUpdate()
        {

            if (reference_frame == null)
            {
                //GetComponent<Rigidbody>().freezeRotation = true;
            }

                //Add a check for isLocalPlayer in the Update function, so that only the local player processes input.
            if (!isLocalPlayer)
            {
                return;
            }
            float GravityAtRadius = 9.81F;

            //Vector3 centrifugal_force = new Vector3(0, transform.position.y - reference_frame.transform.position.y, transform.position.z - reference_frame.transform.position.z);
            //GetComponent<Rigidbody>().AddForce(centrifugal_force.normalized * GravityAtRadius);
            //Debug.DrawLine(transform.position, transform.position + centrifugal_force);

            //this is causing the phantom forces ^^^ ???
            //(floating point error accumulation?)
            //get the direction from the normal & the intensity from magnitude fo 
            
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.down * GravityAtRadius);
            //THE FALLING IS SLOWED BECAUSE OF THIS
            if (!is_paused)
            {
                //GetComponent<Rigidbody>().velocity = new Vector3(translation.x, GetComponent<Rigidbody>().velocity.y, translation.z);
                GetComponent<Rigidbody>().velocity = translation;
            }

            //(reversing the order doesnt help dont bother)
            //the first line adds a relative force which will change the velocity over time
            //the second bit will always set the velocity for that frame to the value of Vector3 translation
            //negating anything but the first frame's worth of acceleration


            //Debug.Log(GetComponent<Rigidbody>().velocity);
            // apply constant weight force according to character normal:
            //GetComponent<ConstantForce>().force = (Gravity_analog * GetComponent<Rigidbody>().mass);
            //Debug.Log(Gravity_analog.magnitude * GetComponent<Rigidbody>().mass * myNormal);

            /*
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, myCollider.radius, Vector3.down, out hitInfo,
                               myCollider.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;
            */
            //

            if (isGroundedCustom)
            {
                //m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    Debug.Log("Jump");
                    //m_MoveDir.y = m_JumpSpeed;
                    m_Jump = false;
                    m_Jumping = true;
                    Debug.Log(GetComponent<Rigidbody>().velocity);
                    GetComponent<Rigidbody>().AddRelativeForce(jump_force * transform.up);
                    Debug.Log(GetComponent<Rigidbody>().velocity);
                    Debug.Log("");
                }
            }
            /*
            else
            {
                m_MoveDir += Gravity_analog * m_GravityMultiplier*Time.fixedDeltaTime;
            }
            //m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);
            /*
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();

            //check the for clearance above character
            if(m_IsCrouching)
            {
                Ray ray = new Ray (gameObject.transform.position, Vector3.up);
                //weird decimal is calculated from the y pos of the player when crouching (0.5800051)
                m_cantStand = Physics.SphereCast (ray, .5f, 0.8799949f);
            }
            */

        }
/*
    void OnGUI()
        {
            //Add a check for isLocalPlayer in the Update function, so that only the local player processes input.
            if (!isLocalPlayer)
            {
                return;
            }
            GUI.Label(new Rect(20, 20, Screen.width / 2, 20), "Hit M to return to the main menu");
        }*/


        private void UpdateCameraPosition(float speed)
        {
            //Add a check for isLocalPlayer in the Update function, so that only the local player processes input.
            if (!isLocalPlayer)
            {
                return;
            }
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (GetComponent<Rigidbody>().velocity.magnitude > 0 && isGroundedCustom)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(GetComponent<Rigidbody>().velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            //Add a check for isLocalPlayer in the Update function, so that only the local player processes input.

            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

			m_IsWalking = (!Input.GetButton("Sprint") || m_IsCrouching);
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);
        }

        
        private void RotateView()
        {

            if (!isLocalPlayer)
            {
                return;
            }

            m_MouseLook.LookRotation (transform, m_Camera.transform, rotation);
            // ^^^ this is causing the jitter

            /////^^^ THIS LOCKS THE COLLIDER TO THE WORLD Y AXIS
            ////// MAKE IT LOCAL


        }

        /*
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(GetComponent<Rigidbody>().velocity*0.1f, hit.point, ForceMode.Impulse);
        }
        */
    }
}
