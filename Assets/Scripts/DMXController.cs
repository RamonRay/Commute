using UnityEngine;
using System.Collections;

public class DMXController : MonoBehaviour
{
		/*
		 C# Interface for the JS controller DMXControllerJS.js
	 	*/
		private static DMXController _instance;
		private DMXControllerJS JSController;
		public static DMXController Lighting {
				get {
						if (_instance == null)
								_instance = GameObject.FindObjectOfType<DMXController> ();
						return _instance;
				}
		}

		void Awake ()
		{
				JSController = this.GetComponent<DMXControllerJS> ();
		}

		void Start ()
		{
				//Blackout ();
				//Invoke ("Blackout", 1);	// need to give the networking time to connect
		}

		void OnDisable ()
		{

		}

		void Update () 
		{
			// if the esacepe key is pressed then player is exiting - quick, send a message to the lighting manager 
			if (Input.GetKeyDown (KeyCode.Escape)) {
				UseShow ("theme");	// start up the theme lighting again
			}
		}
		
		public void Blackout ()
		{
				JSController.Blackout ();
		}
	
		public void AllOn ()
		{
				JSController.AllOn ();
		}
	
		public void TurnOn (string groupName, int red, int green, int  blue, int amber, int dimmer)
		{
				JSController.TurnOn (groupName, red, green, blue, amber, dimmer);
		}
	
		public void TurnOn (string groupName, Color32 thisColor, int amber, int dimmer)
		{
				JSController.TurnOn (groupName, thisColor, amber, dimmer);
		}
	
		public void TurnOn (string groupName, Color thisColor, int amber, int dimmer)
		{
				JSController.TurnOn (groupName, thisColor, amber, dimmer);
		}

		public void TurnOff (string groupName)
		{
				JSController.TurnOff (groupName);
		}



    /*------------------The methods below have not been tested--------------------------*/

        public void MoveVulture(int pan, int tilt, int finePan, int fineTilt)
        {
            JSController.MoveVulture(pan, tilt, finePan, fineTilt);
        }

        /*thisColor is an integer from 0-255 with ranges that cover about 5 Colors. */

        public void TurnOnWaterLight (int thisColor, int rotation, int dimmer)
		{
				JSController.TurnOnWaterLight (thisColor, rotation, dimmer);
		}

		public void UseCue (string cueName, int cueNumber)
		{
				JSController.UseCue (cueName, cueNumber);
		}

		public void UseShow (string showName)
		{
				JSController.UseShow (showName);
		}
}
