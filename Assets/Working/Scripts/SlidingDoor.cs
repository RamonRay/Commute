using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SlidingDoor : MonoBehaviour {
    public enum Direction
    {
        X,
        Y,
        Z
    }
    [SerializeField] protected Vector3 movingDirection = new Vector3(1, 0, 0);
    [SerializeField] protected Direction widthDirection = Direction.X;
    [SerializeField] protected float doorOpenTime =2f;
    [SerializeField] protected bool isOpen =false;
    protected AudioSource sfx;
    [SerializeField] AudioClip doorOpen, doorClose;
    protected  bool isOperating = false;
    protected float doorWidth;
    protected Vector3 initialPosition;
	// Use this for initialization
	void Start () {
        Init();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            OpenDoor();
            Debug.Log("DoorOpen");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            CloseDoor();
            Debug.Log("DoorClosed");
        }

    }

    protected void Init()
    {
        sfx = GetComponent<AudioSource>();
        MeshFilter _mf;
        Mesh _mesh;
        _mf = GetComponent<MeshFilter>();
        if(_mf==null)
        {
            _mf = GetComponentInChildren<MeshFilter>();
        }
        _mesh = _mf.mesh;
        Vector3 meshScale = _mesh.bounds.size;
        Vector3 transformScale = _mf.gameObject.transform.lossyScale;
        initialPosition = transform.position;
        if (widthDirection == Direction.X)
        {
            doorWidth = meshScale.x * transformScale.x;
        }
        else if (widthDirection == Direction.Y)
        {
            doorWidth = meshScale.y * transformScale.y;
        }
        else
        {
            doorWidth = meshScale.z * transformScale.z;
        }

        movingDirection = movingDirection.normalized;
    }

    public void SwtichState()
    {
        if(!isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        if (!isOperating)
        {
            isOperating = true;
            StartCoroutine("GraduallyOpen");
            try
            {
                sfx.clip = doorOpen;
                sfx.Play();
            }
            catch
            {
                //
            }
           
        }

    }
    IEnumerator GraduallyOpen()
    {
        if(isOpen)
        {
            isOperating = false;
            yield break;
        }
        float _deltaX = 0f;
        float _timeInterval = 0.02f;
        float _stepLength = doorWidth / doorOpenTime * _timeInterval;
        while(_deltaX<doorWidth)
        {
            transform.position = transform.position + movingVector(_stepLength);
            _deltaX += _stepLength;
            yield return new WaitForSeconds(_timeInterval);
        }
        isOperating = false;
        isOpen = true;
        yield return 0;
    }
    public void CloseDoor()
    {
        if (!isOperating)
        {
            isOperating = true;
            StartCoroutine(GraduallyClose());
            try
            {
                sfx.clip = doorClose;
                sfx.Play();
            }
            catch
            {
                //
            }
        }
    }

    IEnumerator GraduallyClose()
    {
        if(!isOpen)
        {
            isOperating = false;
            yield break;
        }
        float _deltaX = 0f;
        float _timeInterval = 0.02f;
        float _stepLength = doorWidth / doorOpenTime * _timeInterval;
        while (_deltaX < doorWidth)
        {
            transform.position= transform.position - movingVector(_stepLength);
            _deltaX += _stepLength;
            yield return new WaitForSeconds(_timeInterval);
        }
        isOperating = false;
        isOpen = false;
        yield return 0;
    }

    protected Vector3 movingVector(float movingLength)
    {
        return movingLength * movingDirection;
    }

}
