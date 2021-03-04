	
	/*
	JS Controller that sends all the messages to OSCMain. 
	Check the Lighting OSC Messages.txt documentation to fully understand each function
	*/
	
	private var osc : GameObject;
	private var whichLight : String;

	function Awake () {
		osc = GameObject.Find("OSCMain");
	}

	function Update () {
	}

	public function Blackout () {
		osc.SendMessage("SendOSCMessage","/lighting operations blackout");
	}

	public function AllOn() {
		osc.SendMessage("SendOSCMessage","/lighting operations allOn");
	}

    public function TurnOn(groupName, red: int, green: int, blue: int, amber: int, dimmer: int) {
        osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " "+red+" "+green+" "+blue+" "+amber+" "+dimmer);
	}

    public function TurnOn(groupName, thisColor: Color32, amber: int, dimmer: int) {
        osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " "+thisColor.r+" "+thisColor.g+" "+thisColor.b+" "+amber+" "+dimmer);
	}

    public function TurnOn(groupName, thisColor: Color, amber: int, dimmer: int) {
        osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " "+thisColor.r*255+" "+thisColor.g*255+" "+thisColor.b*255+" "+amber+" "+dimmer);
	}

    public function TurnOff(groupName) {
        osc.SendMessage("SendOSCMessage", "/lighting color " + groupName + " 0 0 0 0 0");
	}
	
	public function MoveVulture (pan: int, tilt: int, finePan: int, fineTilt: int) {
		osc.SendMessage("SendOSCMessage","/lighting move vulture "+pan+" "+tilt+" "+finePan+" "+fineTilt);
	}
	
	public function TurnOnWaterLight(thisColor: int, rotation: int, dimmer: int) {
		osc.SendMessage("SendOSCMessage","/lighting color h20 "+thisColor+" "+rotation+" 0 0 "+dimmer);
	}	
	
    public function UseCue(cueName, cueNumber) {
    osc.SendMessage("SendOSCMessage", "/lighting cue " + cueName + " " + cueNumber);
	}
	
	public function UseShow(showName) {
		osc.SendMessage("SendOSCMessage","/lighting show " + showName);
	}

