using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour {
    [SerializeField] Material unlit, lit;
    private bool isLit=false;
    public bool IsLit
    { get
        {
            return isLit;
        }
      set
        {
            bool oldIsLit = isLit;
            isLit = value;
            if(oldIsLit!=isLit)
            {
                if(isLit)
                {
                    Highlight();
                }
                else
                {
                    DeHighlight();
                }
            }
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Highlight()
    {
            GetComponent<MeshRenderer>().material = lit;
    }
    public void DeHighlight()
    {
            GetComponent<MeshRenderer>().material = unlit;
    }
}
