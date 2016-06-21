using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement; // neded in order to load scenes

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
		[SerializeField] private bool m_IsWalking;
		[SerializeField] private bool m_IsCrouching;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();

        private Camera m_Camera;
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

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, 0);
            m_Jumping = false;
			m_MouseLook.Init(transform , m_Camera.transform);
        }


        // Update is called once per frame
        private void Update()
        {
            if (Input.GetButtonUp("MainMenu"))
            {
                SceneManager.LoadScene("MainMenu");
            }

            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump && m_CharacterController.isGrounded)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;


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
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();

			//check the for clearance above character
			if(m_IsCrouching)
			{
				Ray ray = new Ray (gameObject.transform.position, Vector3.up);
				//weird decimal is calculated from the y pos of the player when crouching (0.5800051)
				m_cantStand = Physics.SphereCast (ray, .5f, 0.8799949f);
			}

        }

        void OnGUI()
        {
            GUI.Label(new Rect(20, 20, Screen.width / 2, 20), "Hit M to return to the main menu");
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
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
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

			m_IsWalking = (!Input.GetButton("Sprint") || m_IsCrouching);
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


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
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
