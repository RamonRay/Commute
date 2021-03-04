using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPad : MonoBehaviour {
    [SerializeField] string password;
    [SerializeField] Material[] inputedKeypadMaterial;
    [SerializeField] Material passwordMatchMaterial;
    [SerializeField] AudioClip keyPressed,passwordMatchSFX,passwordWrongSFX;
    [SerializeField] float passwordFinishedVolumn;
    private AudioSource audioSource;
    private string inputedPassword="";
    private bool isActive = true;

	// Use this for initialization
	void Start () {
        audioSource = gameObject.AddComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            GetInput();
        }

	}

    private void GetInput()
    {
        for(int i=256;i<266;i++)
        {
            if(Input.GetKeyDown((KeyCode)i))
            {
                int inputedInt = i - 256;
                InputNumber(inputedInt);
            }
        }
    }

    public void InputNumber(int _number)
    {
        inputedPassword += _number;
        UpdateMaterial();
        PlaySFX(keyPressed, 1f);
        Debug.Log("Keypad input " + _number + "," + "Current password " + inputedPassword);
        if (inputedPassword.Length == password.Length)
        {
            ComparePassword();
        }
    }

    public void InputNextNumber()
    {
        if(inputedPassword.Length==password.Length)
        {
            return;
        }
        int _nextNumber = password[inputedPassword.Length]-48;
        InputNumber(_nextNumber);
    }


    private void ComparePassword()
    {
        if(inputedPassword==password)
        {
            StartCoroutine(PassWordMatch());
        }
        else
        {
            StartCoroutine(PassWordNotMatch());
        }
        inputedPassword = "";
    }
    private void UpdateMaterial()
    {
        MeshRenderer _mr = GetComponent<MeshRenderer>();
        _mr.material = inputedKeypadMaterial[inputedPassword.Length];
    }

    IEnumerator PassWordMatch()
    {
        isActive = false;
        yield return new WaitForSeconds(1f);
        //Play Sound
        PlaySFX(passwordMatchSFX,passwordFinishedVolumn);
        MeshRenderer _mr = GetComponent<MeshRenderer>();
        _mr.material = passwordMatchMaterial;
        yield break;
    }

    IEnumerator PassWordNotMatch()
    {
        isActive = false;
        yield return new WaitForSeconds(1f);
        //Play password wrong sound
        PlaySFX(passwordWrongSFX,passwordFinishedVolumn);
        inputedPassword = "";
        UpdateMaterial();
        isActive = true;
        yield break;
    }

    private void PlaySFX(AudioClip clip,float volumn)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

}
