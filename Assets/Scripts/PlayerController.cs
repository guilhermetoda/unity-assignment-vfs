using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : AUnit
{

	#region GlobalVariables

    [Header("Movement")]
    [SerializeField]
    private float _MoveSpeed = 5f;

    [SerializeField]
    private KeyCode _SprintKey = KeyCode.LeftShift;

    [SerializeField]
    private float _SprintMult = 5f;

    [Header("Jumping")]
    [SerializeField]
    private float _JumpSpeed = 12f;

    [SerializeField]
    private KeyCode _JumpKey = KeyCode.Space;

    [Header("Camera")]
    [SerializeField]
    private Transform _CamPivot;

    private float _CurrentAngle = 0f;

    private float _BackwardsSpeed;
    private float _SidewaysSpeed;

    private Transform _CamTransform;
    private float _XInput;
    private float _ZInput;
    private bool _JumpPressed;
    private float _SpeedMult = 1f;

    #endregion GlobalVariables


    #region UnityFunctions 

    protected override void UnitAwake()
    {
        _CamTransform = _CamPivot.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
		Cursor.lockState = CursorLockMode.Locked;
        CameraZoom();
        if (GetIsAlive()) 
        {
            ReadMoveInputs();
            UpdateRotations();    
            SetAnimValues();
            ShootLasers();
        }
        
    }

    private void FixedUpdate()
    {
        if (GetIsAlive()) 
        {
            ApplyMovePhysics();
        }
        
    }

    #endregion UnityFunctions


    #region ClassFunctions 

    // Update Camera Rotation
    private void UpdateRotations()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(0f, mouseX, 0f);

        // increase the angle using the Input from the Mouse
        _CurrentAngle += mouseY;
        // Checks if the angle is between 90f and -90f
        if (_CurrentAngle < 90f && _CurrentAngle > -90f) 
        {
            // Rotates only with the Angle is inside of the allowed range.
            _CamPivot.Rotate(-mouseY, 0, 0);
        }
        else 
        {
            // if the angle is higher or lower, clamp the angle!
            _CurrentAngle = Mathf.Clamp(_CurrentAngle, -90f, 90f);
        }

    }

    private void ReadMoveInputs()
    {
        _XInput = Input.GetAxis("Horizontal");
        _ZInput = Input.GetAxis("Vertical");
        _SpeedMult = Input.GetKey(_SprintKey) ? _SprintMult : 1f;

        if (Input.GetKeyDown(_JumpKey))
        {
            _JumpPressed = true;
            _Anim.SetTrigger("Jump");
        }
    }

    private void ShootLasers()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = new Ray(_CamTransform.position, _CamTransform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (CanSee(hit.point, hit.transform)) 
                {
                    ShootLasersFromEyes(hit.point, hit.transform);
                }
            }
        }
    }

    // Function that applies the Movement using the inputs read from the Player
    private void ApplyMovePhysics()
    {
        // SelectedSpeed starts with the move speed
        float _SelectedSpeed = _MoveSpeed;
        
        //Checks if the Player is moving backwards
        if (_ZInput < 0) 
        {
            // Applying the backward speed (1/3 of the Speed)
            _BackwardsSpeed = _MoveSpeed * 0.33f; 
            _SelectedSpeed = _BackwardsSpeed;
        }
        // If the player is moving sideways 
        if (_XInput < 0f || _XInput > 0f) 
        {
            // Applying the sideway speed (2/3 of the speed)
            _SidewaysSpeed = _MoveSpeed * 0.66f;
            _SelectedSpeed = _SidewaysSpeed;
        }
        // Applying the velocity with the Inputs
        // I don't need to use the MathClamp, because, the max value of the Inputs is 1 or -1.
        var newVel = new Vector3(_XInput, 0f, _ZInput) * _SelectedSpeed * _SpeedMult;
        newVel = transform.TransformVector(newVel);
        
        newVel.y = _JumpPressed ? _JumpSpeed : _RB.velocity.y;
        _RB.velocity = newVel;

        _JumpPressed = false;
    }

    private void CameraZoom()
    {
        var newZoom = _CamTransform.localPosition;
        newZoom.z += Input.mouseScrollDelta.y;
        newZoom.z = Mathf.Clamp(newZoom.z, -24, 0);
        _CamTransform.localPosition = newZoom;
    }

    private void SetAnimValues()
    {
        _Anim.SetFloat("SideMovement", _XInput * _SpeedMult);
        _Anim.SetFloat("ForwardMovement", _ZInput * _SpeedMult);
        _Anim.SetFloat("SpeedMult", _SpeedMult);
    }

    #endregion ClassFunctions

}
