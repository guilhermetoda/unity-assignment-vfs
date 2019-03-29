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

    private void UpdateRotations()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(0f, mouseX, 0f);
        

        _CurrentAngle += mouseY;
        if (_CurrentAngle < 90f && _CurrentAngle > -90f) 
        {
            _CamPivot.Rotate(-mouseY, 0, 0);
        }
        else 
        {
            _CurrentAngle = Mathf.Clamp(_CurrentAngle, -90f, 90f);
        }
        
        //_CamPivot.Rotate(Mathf.Clamp(-mouseY, -0.47f, 0.47f), 0, 0);

        //_CamPivot.Rotate(Mathf.Clamp(mouseY, -0.47f, 0.47f), 0f, 0f);
 
        //Camera rotation only allowed if game us not paused
        //_CamPivot.transform.rotation = Quaternion.Euler(-rotY, 0f, 0f);
        //_CamPivot.Rotate(-mouseY, 0, 0);
        
        // if (_CamPivot.transform.rotation.eulerAngles.x > -85f && _CamPivot.transform.rotation.eulerAngles.x < 85f) 
        // {
        //     _CamPivot.Rotate(-mouseY, 0, 0);
        // }
        // else 
        // {
            
        // }
        
        
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

    private void ApplyMovePhysics()
    {
        
        float _SelectedSpeed = _MoveSpeed;
        //Player moving backwards
        if (_ZInput < 0) 
        {
            _BackwardsSpeed = _MoveSpeed / 3; 
            _SelectedSpeed = _BackwardsSpeed;
        }
        
        if (_XInput != 0) 
        {
            _SidewaysSpeed = (_MoveSpeed*2) / 3;
            _SelectedSpeed = _SidewaysSpeed;
        }
        Debug.Log(_SelectedSpeed);
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
