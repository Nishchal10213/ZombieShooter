using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.FileBase.Plugins.FPS_Character_Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class FPS_Controller : MonoBehaviour
    {
        [Header("Controller Info")]
        [SerializeField ][Tooltip("How fast can the controller walk?")]
        private float _walkSpeed = 3.0f; 
        [SerializeField][Tooltip("How fast can the controller run?")]
        private float _runSpeed = 7.0f; 
        [SerializeField][Tooltip("Set your gravity multiplier")] 
        private float _gravity = 1.0f; 
        [SerializeField][Tooltip("How high can the controller jump?")]
        private float _jumpHeight = 15.0f; 
        [SerializeField]
        private bool _isRunning = false; 
        [SerializeField]
        private bool _crouching = false; 

        private CharacterController _controller; 
        private float _yVelocity = 0.0f;
        

        [Header("Headbob Settings")]       
        [SerializeField][Tooltip("Smooth out the transition from moving to not moving")]
        private float _smooth = 20.0f; 
        [SerializeField][Tooltip("How quickly the player head bobs")]
        private float _walkFrequency = 4.8f; 
        [SerializeField][Tooltip("How quickly the player head bobs")]
        private float _runFrequency = 7.8f; 
        [SerializeField][Tooltip("How dramatic the headbob is")][Range(0.0f, 0.2f)]
        private float _heightOffset = 0.05f; 
        private float _timer = Mathf.PI / 2; 
        private Vector3 _initialCameraPos;

        [Header("Camera Settings")]
        [SerializeField]
        [Tooltip("Control the look sensitivty of the camera")]
        private float _lookSensitivity = 5.0f;

        private Camera _fpsCamera;
        private void Start()
        {
            _controller = GetComponent<CharacterController>(); 
            _fpsCamera = GetComponentInChildren<Camera>();
            _initialCameraPos = _fpsCamera.transform.localPosition;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Application.Quit();


            }

            FPSController();
            CameraController();
            HeadBobbing(); 
        }

        void FPSController()
        {
            float h = Input.GetAxis("Horizontal"); 
            float v = Input.GetAxis("Vertical"); 

            Vector3 direction = new Vector3(h, 0, v); 
            Vector3 velocity = direction * _walkSpeed; 

            if (Input.GetKeyDown(KeyCode.C) && _isRunning == false)
            {

                if (_crouching == true)
                {
                    _crouching = false;
                    _controller.height = 2.0f;
                }
                else
                {
                    _crouching = true;
                    _controller.height = 1.0f;
                }
                
            }

            if (Input.GetKey(KeyCode.LeftShift) && _crouching == false) 
            {
                velocity = direction * _runSpeed; //use the run velocity 
                _isRunning = true;
            }
            else
            {
                _isRunning = false;
            }

            if (_controller.isGrounded == true) 
            {
                if (Input.GetKeyDown(KeyCode.Space)) 
                {
                    _yVelocity = _jumpHeight;
                }
            }
            else 
            {
                _yVelocity -= _gravity; 
            }

            velocity.y = _yVelocity; 

            velocity = transform.TransformDirection(velocity);

            _controller.Move(velocity * Time.deltaTime); 
        }

        void CameraController()
        {
            float mouseX = Input.GetAxis("Mouse X"); //get mouse movement on the x
            float mouseY = Input.GetAxis("Mouse Y"); //get mouse movement on the y

            Vector3 rot = transform.localEulerAngles; 
            rot.y += mouseX * _lookSensitivity; 
            transform.localRotation = Quaternion.AngleAxis(rot.y, Vector3.up); 

            Vector3 camRot = _fpsCamera.transform.localEulerAngles; 
            camRot.x += -mouseY * _lookSensitivity; 
            _fpsCamera.transform.localRotation = Quaternion.AngleAxis(camRot.x, Vector3.right); 
        }

        void HeadBobbing()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical"); 

            if (h != 0 || v != 0) 
            {
               
                if (Input.GetKey(KeyCode.LeftShift)) 
                {
                    _timer += _runFrequency * Time.deltaTime; 
                }
                else
                {
                    _timer += _walkFrequency * Time.deltaTime; 
                }

                Vector3 headPosition = new Vector3 
                    (
                        _initialCameraPos.x + Mathf.Cos(_timer) * _heightOffset, //x value
                        _initialCameraPos.y + Mathf.Sin(_timer) * _heightOffset, //y value
                        0 // z value
                    );

                _fpsCamera.transform.localPosition = headPosition; 

                if (_timer > Mathf.PI * 2) 
                {
                    _timer = 0; 
                }
            }
            else
            {
                _timer = Mathf.PI / 2; 
                Vector3 resetHead = new Vector3 
                    (
                    Mathf.Lerp(_fpsCamera.transform.localPosition.x, _initialCameraPos.x, _smooth * Time.deltaTime), 
                    Mathf.Lerp(_fpsCamera.transform.localPosition.y, _initialCameraPos.y, _smooth * Time.deltaTime), 
                    0 
                    );

                _fpsCamera.transform.localPosition = resetHead; 
            }
        }
    }
}

