using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] Vector3[] placements;
    [SerializeField] float speed = 1f;
    [SerializeField] float pacing = 10f;
    [SerializeField] float heightChange = 0.2f;
    [Range(0f,1f)]
    [SerializeField] float bounce = 0.5f;
    [SerializeField] AudioClip[] walkingSFXs;
    private AudioSource audioSource;
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 destination;
    private Quaternion startRotation;
    private int index = 0;
    private float shakeTime = 1f;
    private bool _isMoving = false;
    private bool isMoving
    {
        get
        {
            return _isMoving;
        }
        set
        {
            _isMoving = value;
            if(isMoving)
            {
                StartCoroutine(StartMoving());
            }
            else
            {
                if (paceAngle < 5f)
                {
                    paceAngle = 0f;
                }
                else
                {
                    paceAngle = Mathf.Lerp(0f, paceAngle, 0.1f);
                }
            }
        }
    }
    private float startingHeight;
    private float paceAngle = 0f;
	// Use this for initialization
	void Start () {
        destination = transform.position;
        startingHeight = transform.position.y;
        currentRotation = transform.rotation.eulerAngles;
        targetRotation = currentRotation;
        startRotation = transform.rotation;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
	}

    // Update is called once per frame
    void Update() {

    }

    private void Move(Vector3 Destination)
    {

        Vector3 moveDirectron = new Vector3(Destination.x - transform.position.x, 0, Destination.z - transform.position.z);
        /*
        float horizontal=Input.GetAxisRaw("Horizontal");
        float vertical=Input.GetAxisRaw("Vertical");
        if(horizontal==0f&&vertical==0f)
        {
            isMoving = false;
            moveDirectron = Vector3.zero;
        }
        else
        */
        //{
            if(moveDirectron.magnitude<0.1f)
            {
                isMoving = false;
                return;
            }
        moveDirectron = moveDirectron.normalized * speed * Time.deltaTime;
        paceAngle += pacing*Time.deltaTime;
        if(paceAngle>180f)
        {
            paceAngle -= 180f;
            PlayWalkingSFX();
        }
        //}
        transform.position = new Vector3(transform.position.x + moveDirectron.x,
            startingHeight - Mathf.Sin(paceAngle / 180f * Mathf.PI)*heightChange, transform.position.z + moveDirectron.z);
    }




    public void MoveToPlacement()
    {
        if(index>=placements.Length)
        {
            Debug.LogError("Placement Out of Range");
            index = 0;
            return;
        }
        destination = placements[index];
        isMoving = true;
        PlayWalkingSFX();
        index++;
    }
    IEnumerator StartMoving()
    {
        while (isMoving)
        {
            Move(destination);
            yield return 0;
        }
        while(paceAngle>1f)
        {
            isMoving = false;
            yield return 0;
        }
        yield break;
    }
    public void CameraShake(float angle, float time)
    {
        
        shakeTime = time;
        currentRotation = transform.rotation.eulerAngles;
        try
        {
            StopAllCoroutines();
        }
        catch
        {
            Debug.LogError("No running Coroutine");
        }
        targetRotation = new Vector3(currentRotation.x, currentRotation.y, angle);
        StartCoroutine(ShakeOverTime());
    }
    IEnumerator ShakeOverTime()
    {
        float startTime = Time.time;
        while (Time.time < startTime + shakeTime)
        {
            float parameter = Mathf.Sin((Time.time - startTime) / shakeTime*0.5f*Mathf.PI) ;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), parameter);
            yield return new WaitForSeconds(0.02f);
        }
        yield break;
    }

    public void Untilted()
    {
        StartCoroutine(ReturnToNeutral());
    }

    IEnumerator ReturnToNeutral()
    {
        float _step = 8f;
        float _angle = 90f;
        float amplifier = 1f;
        Quaternion _startRotation = transform.rotation;
        while(amplifier>0.005f||_angle>_step)
        {
            transform.rotation = Quaternion.Lerp(startRotation, _startRotation, amplifier * Mathf.Abs(Mathf.Sin(_angle / 180f * Mathf.PI)));
            _angle += _step;
            while(_angle>180f)
            {
                _angle -= 180f;
                amplifier = bounce * amplifier;
            }
            yield return new WaitForSeconds(0.02f);
        }
        yield break;

    }
    private void PlayWalkingSFX()
    {
        audioSource.Stop();
        audioSource.clip = walkingSFXs[Random.Range(0, walkingSFXs.Length)];
        audioSource.Play();
    }
}
