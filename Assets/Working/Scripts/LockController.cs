using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockController : MonoBehaviour {

    [SerializeField] Material defaultLockMat;
    [SerializeField] Material lockedLockMat;
    [SerializeField] Material unlockedLockMat;
    private AudioSource audioSource;
    private bool _isLocked = true;
    public bool isLocked
    {
        get
        {
            return _isLocked;
        }
        set
        {
            _isLocked = value;
            if(_isLocked)
            {
                SetLockedMat();
            }
            else
            {
                audioSource.Stop();
                audioSource.Play();
                SetUnlockedMat();
            }
        }
    }
    private MeshRenderer mr;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        mr = this.GetComponent<MeshRenderer>();
        SetDefaultMat();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDefaultMat() {
        mr.sharedMaterial = defaultLockMat;
    }

    public void SetLockedMat() {
        mr.sharedMaterial = lockedLockMat;
    }

    public void SetUnlockedMat() {
        mr.sharedMaterial = unlockedLockMat;
    }
}
