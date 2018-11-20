using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SensorBugTest : MonoBehaviour
{
	public string DeviceName = "2020BLE";
    [SerializeField] float maxHealth = 24f;
    [SerializeField] Slider p1healthBar;
    [SerializeField] Slider p2healthBar;


	//public Text AccelerometerText;
	public Text SensorBugStatusText;
    public Text BigHitText;

    //public GameObject PairingMessage;
    //public GameObject TopPanel;
    //public GameObject MiddlePanel;

    //[SerializeField] Text debugText;

    //ANIMATION VARIABLES
    //Round start
    public Animator animatorReady;
    public Animator animatorStart;
    public Animator animatorP1Combo;
    public Animator animatorP2Combo;
    public Animator animatorP1BigHit;
    public Animator animatorP2BigHit;
    //WINNER ANIMATION SHIT
    public Animator animatorPlayer;
    public Animator animatorWins;
    public Text playerName;
    public Text winText;
    public Color playerOneColor;
    public Color playerTwoColor;


    int test = 0;

	public class Characteristic
	{
		public string ServiceUUID;
		public string CharacteristicUUID;
		public bool Found;
	}

	public static List<Characteristic> Characteristics = new List<Characteristic>
	{

        //GAME SERVICE
        //Game Mode Characteristic 
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2dcfbc481", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c206ffc481", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2dcfbc481", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c27800c581", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2dcfbc481", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c2ae01c581", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2dcfbc481", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c2da02c581", Found = false },
        
        //ADD OTHER SERVICES AND CHARACTERISTICS HERE
        //Player 1 Characteristics
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2e60ac581", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c2160dc581", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2e60ac581", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c2600ec581", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2e60ac581", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c28c0fc581", Found = false },

        //Player 2 Characteristics
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2b810c581", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c20a14c581", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2b810c581", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c25415c581", Found = false },
        new Characteristic { ServiceUUID = "5914fb69-9252-55a3-e811-66c2b810c581", CharacteristicUUID = "5914fb69-9252-55a3-e811-66c29e16c581", Found = false },

    };

    //a time of 0xFF means that the game time selected is INFINITE
    public uint[] TimerValues = {0, 10, 15, 20, 30, 45, 60, 90, 120, 180, 0xFF };

	public Characteristic GameMode = Characteristics[0];
	public Characteristic GameState = Characteristics[1];
	public Characteristic PlayTimeSelected = Characteristics[2];
    public Characteristic DifficultySelected = Characteristics[3];

    public Characteristic Player1Health = Characteristics[4];
    public Characteristic Player1Impact = Characteristics[5];
    public Characteristic Player1Colour = Characteristics[6];

    public Characteristic Player2Health = Characteristics[7];
    public Characteristic Player2Impact = Characteristics[8];
    public Characteristic Player2Colour = Characteristics[9];

    //Characteristic values???
    string GameModeValue;
    string GameStateValue;
    //uint PlaytimeSelectedValue;
    string DifficultySelectedValue;

    int timerAmount = 0;

    uint Player1HealthValue;
    uint Player2HealthValue;

    int ComboMeter = 2;
    int BigHitMeter = 4;
    bool IsPlayer1Red;
    float ComboTimer1;
    float ComboTimer2;
    bool IsCombo1On;
    bool IsCombo2On;

    public bool AllCharacteristicsFound { get { return !(Characteristics.Where (c => c.Found == false).Any ()); } }

    public uint UpdatePlayerHealth(UInt32 health, bool isP1)
    {
        uint numLeds = 0;
        //convert health into just number of LEDs
        while (health > 0)
        {
            health = health >> 1;
            numLeds++;
        }

        setHealthBar((int)numLeds, isP1);
        //SensorBugStatusText.text = numLeds + " health";
        return numLeds;

    }

    void changeGameState(int stateNum)
    {
        SensorBugStatusText.text = "game state integer: " + stateNum;

        switch (stateNum)
        {
            case 4:     // GAME STARTS
                animatorReady.SetTrigger("MatchStart");
                animatorStart.SetTrigger("MatchStart");
                startTimer();
                break;
            case 8:     // GAME ENDS
                endGame();
                break;
            default:
                break;
        }
    }

    void startTimer()
    {
        GetComponent<UIBehaiviour>().startTimer();
    }

    void endGame()
    {

    }

    void setTimerAmount(int amount)
    {
        //timerAmount = amount;
        GetComponent<UIBehaiviour>().resetTimer(amount);
    }

    
    //checks if the found service and characterstic matches one of the ones in the array?
	public Characteristic GetCharacteristic (string serviceUUID, string characteristicsUUID)
	{
        //debugText.text += " " + serviceUUID + " ///--/// " +characteristicsUUID;
		return Characteristics.Where (c => IsEqual (serviceUUID, c.ServiceUUID) && IsEqual (characteristicsUUID, c.CharacteristicUUID)).FirstOrDefault ();
	}

	enum States
	{
		None,
		Scan,
		Connect,
		ReadPairingStatus,
		WaitPairingStatus,
		ConfigureAccelerometer,
		SubscribeToAccelerometer,
		SubscribingToAccelerometer,
		Disconnect,
		Disconnecting,
	}

	private bool _connected = false;
	private float _timeout = 0f;
	private States _state = States.None;
	private string _deviceAddress;
	private bool _pairing = false;

	private byte[] _accelerometerConfigureBytes = new byte[] { 0x01, 0x01 };

	string SensorBugStatusMessage
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
				BluetoothLEHardwareInterface.Log (value);
			if (SensorBugStatusText != null)
				SensorBugStatusText.text = value;
		}
	}

    //Code for re-pairing with a device...
	void Reset ()
	{
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_deviceAddress = null;

		if (!_pairing)
		{
			//PairingMessage.SetActive (true);
			//TopPanel.SetActive (false);
			//MiddlePanel.SetActive (false);

			SensorBugStatusMessage = "";
		}

		_pairing = false;
	}
    
    //Changes state of the code
	void SetState (States newState, float timeout)
	{
		_state = newState;
		_timeout = timeout;
	}

    //Initialization code?
    // Sets the state to scan mode with a timeout of 0.1f
    // if device has been found it will try to pair
	void StartProcess ()
	{
		Reset ();
		BluetoothLEHardwareInterface.Initialize (true, false, () => {

			SetState (States.Scan, 0.1f);

		}, (error) => {

			if (_state == States.SubscribingToAccelerometer)
			{
				_pairing = true;
				SensorBugStatusMessage = "Pairing to 2020 device";

				// if we get an error when trying to subscribe to the SensorBug it is
				// most likely because we just paired with it. Right after pairing you
				// have to disconnect and reconnect before being able to subscribe.
				SetState (States.Disconnect, 0.1f);
			}

			BluetoothLEHardwareInterface.Log ("Error: " + error);
		});
	}

	// Use this for initialization
	void Start ()
	{
		StartProcess ();
	}

	// Update is called once per frame
	void Update ()
	{
        
        if (IsCombo2On)
        {
            ComboTimer2 += Time.deltaTime;
        }

        if (IsCombo1On)
        {
            ComboTimer1 += Time.deltaTime;
        }

        if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				_timeout = 0f;

				switch (_state)
				{
				case States.None:
					break;
                
                //Scanning will look for a device that has the device name defined at the top of the code, currently "BLE2020"
				case States.Scan:
                        //debugText.text = "I ran";
                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, deviceName) => {

                        //debugText.text += deviceName;
                        

						if (deviceName.Contains (DeviceName))
						{
							SensorBugStatusMessage = "Found a 2020 Armor device";

							BluetoothLEHardwareInterface.StopScan ();

							//PairingMessage.SetActive (false);
							//TopPanel.SetActive (true);

							// found a device with the name we want
							// this example does not deal with finding more than one
							_deviceAddress = address;
							SetState (States.Connect, 0.5f);
						}

					}, null, true);
					break;

                //After a device has been found it will automatically connect with it...
				case States.Connect:
					SensorBugStatusMessage = "Connecting to 2020 Armor device...";

					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddress, null, null, (address, serviceUUID, characteristicUUID) => {
                        //debugText.text += "";
                        //Checks first collected service and characteristic????
						var characteristic = GetCharacteristic (serviceUUID, characteristicUUID);
						if (characteristic != null)
                        {
                            SensorBugStatusMessage = "Checking the characteristics of 2020 Armor device...";


                            //debugText.text += characteristicUUID + " ";
                            //if first characteristic is found, it will try and find all of them and then compare... and then sets the connection
                            //status to true...? moesv on to reading pairiring status...?
							BluetoothLEHardwareInterface.Log (string.Format ("Found {0}, {1}", serviceUUID, characteristicUUID));

							characteristic.Found = true;

							if (AllCharacteristicsFound)
							{
                                //debugText.text += " CONNECTED";
								_connected = true;

                                // SetState (States.ReadPairingStatus, 3f);
                                SetState(States.SubscribeToAccelerometer, 3f);
							}
						}
					}, (disconnectAddress) => {
						SensorBugStatusMessage = "Disconnected from SensorBug";
						Reset ();
						SetState (States.Scan, 1f);
					});
					break;


                //Seems that you cannot subscribe to characteristics immediately after each other, needs some time in between! Currently using 1000ms, which maybe is long... also using delay, which maybe is not good... 
                //Consider how to improve this in the future?

				case States.SubscribeToAccelerometer:
					SetState (States.SubscribingToAccelerometer, 10f);
					SensorBugStatusMessage = "Subscribing to Game Mode...";
                        //MAKE SURE THAT THIS IS ONLY HAPPENING ONCE...
                    _state = States.None;
                    test++;
                        //debugText.text = " " +  test;

                        //debugText.text = " Subscribing to playtime...";
                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, PlayTimeSelected.ServiceUUID,
                        PlayTimeSelected.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                        //test++;




                        _state = States.None;
                            //MiddlePanel.SetActive(true);
                        //debugText.text = 

                         //PlaytimeSelectedValue = BitConverter.ToString(bytes);

                        // PlaytimeSelectedValue = TimerValues[bytes[0]];
                            setTimerAmount((int)TimerValues[bytes[0]]);
                        //AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                        });

                        //debugText.text = " Tried to subscribe to playtime...";

                        Thread.Sleep(1000);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddress, GameState.ServiceUUID, 
                        GameState.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                            //test++;

                            SensorBugStatusText.text = "hi ";
                        _state = States.None;



                          

						//GameStateValue = BitConverter.ToString (bytes);
                           // var sBytes = BitConverter.ToUInt32(bytes, 0);
                           // SensorBugStatusText.text = sBytes + " is the game state";
                            changeGameState(bytes[0]);


                            //debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //   AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //debugText.text = sBytes + " " + test;
                        });

                        Thread.Sleep(1000);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, GameMode.ServiceUUID,
                        GameMode.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                            //test++;


                            _state = States.None;
                            // MiddlePanel.SetActive(true);
                            //debugText.text = 

                            //GameModeValue = BitConverter.ToString(bytes);
                            var sBytes = BitConverter.ToUInt32(bytes, 0);



                            // debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            // AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //debugText.text = sBytes + " " + test;
                        });

                        Thread.Sleep(1000);

                        

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player1Health.ServiceUUID,
                        Player1Health.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                            //test++;


                            _state = States.None;
                           // MiddlePanel.SetActive(true);
                            //debugText.text = 

                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(bytes);

                            var sBytes = BitConverter.ToUInt32(bytes,0);

                            

                            Player1HealthValue = UpdatePlayerHealth(sBytes, true);

                            //check if player is dead
                            if(Player1HealthValue == 0)
                            {
                                TriggerPlayerDeath(IsPlayer1Red, true);
                            }

                            //debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //  debugText.text = "value of Player1Health:" + Player1HealthValue;


                            CheckPlayerCombo(IsPlayer1Red, true);
                        });

                        Thread.Sleep(1000);

                        

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player2Health.ServiceUUID,
                        Player2Health.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                            //test++;


                            _state = States.None;
                          //  MiddlePanel.SetActive(true);
                            //debugText.text = 

                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(bytes);

                            var sBytes = BitConverter.ToUInt32(bytes, 0);


                            Player2HealthValue = UpdatePlayerHealth(sBytes, false);
                            //check if player is dead
                            if (Player2HealthValue == 0)
                            {
                                TriggerPlayerDeath(IsPlayer1Red, false);
                            }

                            //debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            // debugText.text = "value of Player2Health:" + Player2HealthValue;

                            //Test player 2 got hit
                            CheckPlayerCombo(IsPlayer1Red, false);
                           
                        });


                        //Test Players Colour
                        Thread.Sleep(1000);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player1Colour.ServiceUUID,
                        Player1Colour.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) =>
                        {

                            _state = States.None;

                            
                            // if (BitConverter.IsLittleEndian)
                            //   Array.Reverse(bytes);

                            var sBytes = BitConverter.ToString(bytes);

                            CheckPlayerColor(sBytes);
                        });

                        //Test Big Hit
                        Thread.Sleep(1000);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player1Impact.ServiceUUID,
                        Player1Impact.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {

                            _state = States.None;


                            // if (BitConverter.IsLittleEndian)
                            //   Array.Reverse(bytes);

                            var sBytes = bytes[0];


                            if (sBytes >= BigHitMeter)
                                CheckBigHit(IsPlayer1Red, true);
                            
                        });

                        Thread.Sleep(1000);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player2Impact.ServiceUUID,
                        Player2Impact.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {

                            _state = States.None;


                             if (BitConverter.IsLittleEndian)
                               Array.Reverse(bytes);

                            var sBytes = bytes[0];


                            if (sBytes >= BigHitMeter)
                                CheckBigHit(IsPlayer1Red, false);
                            
                        });

                        break;



				case States.Disconnect:
					SetState (States.Disconnecting, 5f);
					if (_connected)
					{
						BluetoothLEHardwareInterface.DisconnectPeripheral (_deviceAddress, (address) => {
							// since we have a callback for disconnect in the connect method above, we don't
							// need to process the callback here.
						});
					}
					else
					{
						Reset ();
						SetState (States.Scan, 1f);
					}
					break;

				case States.Disconnecting:
					// if we got here we timed out disconnecting, so just go to disconnected state
					Reset ();
					SetState (States.Scan, 1f);
					break;
				}
			}
		}
	}

	bool IsEqual (string uuid1, string uuid2)
	{
		return (uuid1.ToUpper ().CompareTo (uuid2.ToUpper ()) == 0);
	}

    void TriggerPlayerDeath(bool IsP1Red, bool Player1GotHit)
    {
        if((IsP1Red && Player1GotHit) || (!IsP1Red && !Player1GotHit))
        {
            winText.color = playerTwoColor;
            playerName.color = playerTwoColor;
            playerName.text = "Christoph Sonnen";
        }
        if((!IsP1Red && Player1GotHit) || (IsP1Red && !Player1GotHit))
        {
            playerName.color = playerOneColor;
            winText.color = playerOneColor;
            playerName.text = "Ali Ghafour";
        }
        animatorPlayer.SetTrigger("Win");
        animatorWins.SetTrigger("Win");

        //if (Player1HealthValue == 0)
        //{
        //    if (IsPlayer1Red)
        //    {
        //        //Player 2 WINS
        //        winText.color = playerTwoColor;
        //        playerName.color = playerTwoColor;
        //        playerName.text = "Christoph Sonnen";
        //    }
        //    else
        //    {
        //        //Player 1 Wins
        //        winText.color = playerOneColor;
        //        playerName.color = playerOneColor;
        //        playerName.text = "Ali Ghafour";
        //    }
        //}
        //else
        //{
        //    if (IsPlayer1Red)
        //    {
        //        //Player 1 WINS
        //        playerName.color = playerOneColor;
        //        winText.color = playerOneColor;
        //        playerName.text = "Ali Ghafour";
        //    }
        //    else
        //    {
        //        //Player 2 WINS
        //        winText.color = playerTwoColor;
        //        playerName.color = playerTwoColor;
        //        playerName.text = "Christoph Sonnen";
        //    }
        //}

    }

    void setHealthBar(int amount, bool isP1)
    {
        //float healthPerc = amount / maxHealth;

        if (IsPlayer1Red)
        {
            //SensorBugStatusText.text = "Player 1 is red!";
            if (isP1)
            {
                p1healthBar.value = amount;
            }
            else
            {
                p2healthBar.value = amount;
            }
        }
        else
        {
            //SensorBugStatusText.text = "Player 2 is red!";
            if (isP1)
            {
                p2healthBar.value = amount;
            }
            else
            {
                p1healthBar.value = amount;
            }
        }
    }

    void CheckBigHit(bool IsP1Red, bool Player1GotHit)
    {
        if((IsP1Red && Player1GotHit)||(!IsP1Red && !Player1GotHit))
        {
            animatorP2BigHit.SetTrigger("Hit");
            BigHitText.text = "P2 GOT A BIG HIT!";
        }

        if((IsP1Red && !Player1GotHit)||(!IsP1Red && Player1GotHit))
        {
            animatorP1BigHit.SetTrigger("Hit");
            BigHitText.text = "P1 GOT A BIG HIT!";
        }
    }

    void CheckPlayerCombo(bool IsP1Red, bool Player1GotHit)
    {
        if((Player1GotHit && IsP1Red) || (!Player1GotHit && !IsP1Red))
        {
            if (IsCombo2On)
            {
                if (ComboTimer2 < ComboMeter)
                {
                    animatorP2Combo.SetTrigger("Combo");
                    SensorBugStatusText.text = "Player 2 got a combo!";
                    ComboTimer2 = 0;
                }
                else
                {
                    SensorBugStatusText.text = "Reset combo 2 timer";
                    ComboTimer2 = 0;
                }
            }
            else
                IsCombo2On = true;
        }

        if((Player1GotHit && !IsP1Red) || (!Player1GotHit && IsP1Red))
        {
            if (IsCombo1On)
            {
                if (ComboTimer1 < ComboMeter)
                {
                    animatorP1Combo.SetTrigger("Combo");
                    SensorBugStatusText.text = "Player 1 got a combo!";
                    ComboTimer1 = 0;
                }
                else
                {
                    SensorBugStatusText.text = "Reset combo 1 timer";
                    ComboTimer1 = 0;
                }
            }
            else
                IsCombo1On = true;
        }
    }

    void CheckPlayerColor(string bytes)
    {
        if (bytes == "01")
            IsPlayer1Red = true;
        else
            IsPlayer1Red = false;
    }
}
