using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NatCorderU.Examples;

public class SensorBugTest : MonoBehaviour
{
    public string DeviceName = "2020BLE";

    [SerializeField] float maxHealth = 24f;
    [SerializeField] Slider p1healthBar;
    [SerializeField] Slider p2healthBar;
    [SerializeField] TextMeshProUGUI P1TotalDamagedAmount, P2TotalDamagedAmount, P1ComboText, P2ComboText, P1BigHitText, P2BigHitText;
    [SerializeField] GameObject recordVideo;
    [SerializeField] GameObject SceneManager;

    [SerializeField] Button btn_Next;

    float p1_hpPercent;
    float p2_hpPercent;
    [SerializeField] TextMeshProUGUI p1_hpText;
    [SerializeField] TextMeshProUGUI p2_hpText;

    //public Text AccelerometerText;
    //public Text SensorBugStatusText;
    //public Text BigHitText;

    //public GameObject PairingMessage;
    //public GameObject TopPanel;
    //public GameObject MiddlePanel;

    //[SerializeField] Text debugText;

    //ANIMATION VARIABLES
    //Round start

    [SerializeField] Animator p1_combo, p1_counter, p1_bigHit, p2_combo, p2_counter, p2_bigHit;


    public Animator animatorReady;
    public Animator animatorStart;
    public Animator animatorP1Combo;
    public Animator animatorP2Combo;
    public Animator animatorP1BigHit;
    public Animator animatorP2BigHit;
    //WINNER ANIMATION SHIT
    public Animator animatorPlayer;
    //public Animator animatorWins;
    public TextMeshProUGUI playerName, winText;
    public Color playerOneColor;
    public Color playerTwoColor;


    string p1name, p2name;

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
    public uint[] TimerValues = { 0, 10, 15, 20, 30, 45, 60, 90, 120, 180, 0xFF };

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

    uint Player1HealthValue, Player2HealthValue;

    bool IsPlayer1Red;
    float ComboTimer1, ComboTimer2;
    bool IsCombo1On, IsCombo2On;
    [SerializeField] TextMeshProUGUI LeftPoint, RightPoint;
    int P1Points, P2Points;
    float ThreeHitTimer1, ThreeHitTimer2;
    int ThreeHitCountP1, ThreeHitCountP2;
    bool ThreeHitCombo1On, ThreeHitCombo2On;
    [SerializeField] TextMeshProUGUI LeftComboAmountText, RightComboAmountText, ConnectStatus;
    [SerializeField] GameObject ConnectInfo;
    float scanTimer = 0.0f;
    float P1HealthLoss, P2HealthLoss;
    int P1ComboHits, P2ComboHits;
    float P1BigHitHealthLoss, P2BigHitHealthLoss;

    public class DeviceInfo
    {
        public string dAddress;
        public int dSignal;
    }
    List<DeviceInfo> deviceList;

    public bool AllCharacteristicsFound { get { return !(Characteristics.Where(c => c.Found == false).Any()); } }

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
        //SensorBugStatusText.text = "game state integer: " + stateNum;

        switch (stateNum)
        {
            case 4:     // GAME STARTS
                animatorReady.SetTrigger("MatchStart");
                animatorStart.SetTrigger("MatchStart");
                startTimer();
                recordVideo.GetComponent<ReplayCam>().StartRecording();
                break;
            case 8:     // GAME ENDS
                recordVideo.GetComponent<ReplayCam>().EndGameRecord();
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
    public Characteristic GetCharacteristic(string serviceUUID, string characteristicsUUID)
    {
        //debugText.text += " " + serviceUUID + " ///--/// " +characteristicsUUID;
        return Characteristics.Where(c => IsEqual(serviceUUID, c.ServiceUUID) && IsEqual(characteristicsUUID, c.CharacteristicUUID)).FirstOrDefault();
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
                BluetoothLEHardwareInterface.Log(value);
            //if (SensorBugStatusText != null)
                //SensorBugStatusText.text = value;
        }
    }

    //Code for re-pairing with a device...
    void Reset()
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
    //Set to disconnect after connected
    public TextMeshProUGUI connectionMode;
    public Button nextButton;
    bool connection = false;

    public void ConnectState()
    {
        if (_connected)
        {
            connection = false;
            scanTimer = 0.0f;
            BluetoothLEHardwareInterface.DisconnectPeripheral(_deviceAddress, (address) =>
            {
                // since we have a callback for disconnect in the connect method above, we don't
                // need to process the callback here.
            });
            deviceList.RemoveRange(0, deviceList.Count);
            Reset();
            //ConnectStatus.text = "Disconnecting...";
            connectionMode.text = "CONNECT\nTO VEST";
            ConnectInfo.SetActive(false);
            nextButton.interactable = false;
        }
        else
        {
            connection = true;
            ConnectStatus.text = "SEARCHING FOR VEST...\nPLEASE WAIT";
            SceneManager.GetComponent<Screen_Manager>().ConnectingScreen();
            StartConnect();
        }
    }

    //Changes state of the code
    void SetState(States newState, float timeout)
    {
        _state = newState;
        _timeout = timeout;
    }

    //Initialization code?
    // Sets the state to scan mode with a timeout of 0.1f
    // if device has been found it will try to pair
    public void StartConnect()
    {
        deviceList = new List<DeviceInfo>();
        deviceList.RemoveRange(0, deviceList.Count);
        scanTimer = 0.0f;
        Reset();
        BluetoothLEHardwareInterface.Initialize(true, false, () => {

            SetState(States.Scan, 2.5f);

        }, (error) => {

            if (_state == States.SubscribingToAccelerometer)
            {
                _pairing = true;
                SensorBugStatusMessage = "Pairing to 2020 device";

                // if we get an error when trying to subscribe to the SensorBug it is
                // most likely because we just paired with it. Right after pairing you
                // have to disconnect and reconnect before being able to subscribe.
                SetState(States.Disconnect, 0.1f);
            }

            BluetoothLEHardwareInterface.Log("Error: " + error);
        });
    }

    // Fixed Update is called once per 0.2 frames
    // Use this for initialization


    // Update is called once per frame
    void Update()
    {
        //Update Health Percent on Projector Screen
        p1_hpPercent = Mathf.Round (( p1healthBar.value/ maxHealth) * 100);
        p2_hpPercent = Mathf.Round(( p2healthBar.value / maxHealth) * 100);

        p1_hpText.text = p1_hpPercent + "%";
        p2_hpText.text = p2_hpPercent + "%";

        LeftPoint.text = P1Points.ToString("D8");
        RightPoint.text = P2Points.ToString("D8");
        LeftComboAmountText.text = ThreeHitCountP1.ToString();
        RightComboAmountText.text = ThreeHitCountP2.ToString();

        P1ComboText.text = P1ComboHits.ToString();
        P1TotalDamagedAmount.text = Mathf.RoundToInt(P1HealthLoss) + "%";
        P2ComboText.text = P2ComboHits.ToString();
        P2TotalDamagedAmount.text = Mathf.RoundToInt(P2HealthLoss) + "%";

        if (IsCombo2On)
            ComboTimer2 += Time.deltaTime;
        if (IsCombo1On)
            ComboTimer1 += Time.deltaTime;

        if (ComboTimer1 > 2)
        {
            animatorP1Combo.SetBool("Combo", false);
            P1ComboHits = 0;
            P1HealthLoss = 0;
        }
        if (ComboTimer2 > 2)
        {
            animatorP2Combo.SetBool("Combo", false);
            P2ComboHits = 0;
            P2HealthLoss = 0;
        }

        if (ThreeHitCombo1On)
            ThreeHitTimer1 += Time.deltaTime;
        if (ThreeHitCombo2On)
            ThreeHitTimer2 += Time.deltaTime;

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
                        scanTimer += Time.deltaTime;
                        //debugText.text = "I ran";
                        //BigHitText.text = "Scan mode start";
                        int compare;
                        DeviceInfo device;
                        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, null, (address, deviceName, signalStrength, bytes) =>
                        {
                            //if (deviceName.Contains(DeviceName) && scanTimer <= 0.5f)
                            if (deviceName.Contains(DeviceName) && scanTimer <= 2.0f && connection == true)
                            {
                                //BigHitText.text = "Found a 2020 Armor device";
                                device = new DeviceInfo();
                                device.dAddress = address;
                                device.dSignal = signalStrength;
                                deviceList.Add(device);
                            }
                            else
                            {
                                //foreach (DeviceInfo dI in deviceList)
                                //{
                                    //BigHitText.text = BigHitText.text + "\n" + dI.dAddress + " " + dI.dSignal;
                                //}
                                BluetoothLEHardwareInterface.StopScan();
                                _deviceAddress = deviceList[0].dAddress;
                                compare = deviceList[0].dSignal;
                                for (int i = 0; i < deviceList.Count; i++)
                                {
                                    if (compare <= deviceList[i].dSignal)
                                    {
                                        _deviceAddress = deviceList[i].dAddress;
                                        compare = deviceList[i].dSignal;
                                    }
                                }
                                ConnectStatus.text = "Connecting...";
                                SetState(States.Connect, 0.5f);
                            }
                        }, true);
                        break;

                    //After a device has been found it will automatically connect with it...
                    case States.Connect:
                        SensorBugStatusMessage = "Connecting to 2020 Armor device...";

                        BluetoothLEHardwareInterface.ConnectToPeripheral(_deviceAddress, null, null, (address, serviceUUID, characteristicUUID) => {
                            //debugText.text += "";
                            //Checks first collected service and characteristic????
                            var characteristic = GetCharacteristic(serviceUUID, characteristicUUID);
                            if (characteristic != null)
                            {
                                SensorBugStatusMessage = "Checking the characteristics of 2020 Armor device...";


                                //debugText.text += characteristicUUID + " ";
                                //if first characteristic is found, it will try and find all of them and then compare... and then sets the connection
                                //status to true...? moesv on to reading pairiring status...?
                                BluetoothLEHardwareInterface.Log(string.Format("Found {0}, {1}", serviceUUID, characteristicUUID));

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
                            Reset();
                            SetState(States.Scan, 1f);
                        });
                        break;


                    //Seems that you cannot subscribe to characteristics immediately after each other, needs some time in between! Currently using 1000ms, which maybe is long... also using delay, which maybe is not good... 
                    //Consider how to improve this in the future?

                    case States.SubscribeToAccelerometer:

                        int subscriptionDelayTime = 500;
                        
                        SetState(States.SubscribingToAccelerometer, 10f);
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

                        Thread.Sleep(subscriptionDelayTime);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, GameState.ServiceUUID,
                        GameState.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                            //test++;

                            //SensorBugStatusText.text = "hi ";
                            _state = States.None;





                            //GameStateValue = BitConverter.ToString (bytes);
                            // var sBytes = BitConverter.ToUInt32(bytes, 0);
                            // SensorBugStatusText.text = sBytes + " is the game state";
                            changeGameState(bytes[0]);


                            //debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //   AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //debugText.text = sBytes + " " + test;
                        });

                        Thread.Sleep(subscriptionDelayTime);

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

                        Thread.Sleep(subscriptionDelayTime);



                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player1Health.ServiceUUID,
                        Player1Health.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {
                            //test++;


                            _state = States.None;
                            // MiddlePanel.SetActive(true);
                            //debugText.text = 

                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(bytes);

                            var sBytes = BitConverter.ToUInt32(bytes, 0);


                            Player1HealthValue = UpdatePlayerHealth(sBytes, true);

                            //check if player is dead

                            if (Player1HealthValue == 0)
                            {
                                TriggerPlayerDeath(IsPlayer1Red, true);
                            }

                            //debugText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //AccelerometerText.text = "Game Mode Selected: " + GameModeValue + "Game State Selected: " + GameStateValue + "Playtime Selected: " + PlaytimeSelectedValue;
                            //  debugText.text = "value of Player1Health:" + Player1HealthValue;


                        });

                        Thread.Sleep(subscriptionDelayTime);



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

                        });


                        //Test Players Colour
                        Thread.Sleep(subscriptionDelayTime);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player1Colour.ServiceUUID,
                        Player1Colour.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) =>
                        {

                            _state = States.None;


                            // if (BitConverter.IsLittleEndian)
                            //   Array.Reverse(bytes);

                            var sBytes = BitConverter.ToString(bytes);


                            //Player1ColourValue = Player1Colour.ServiceUUID;
                            //SensorBugStatusText.text = "P1 Color: " + sBytes;

                            CheckPlayerColor(sBytes);

                        });

                        //Test Big Hit
                        Thread.Sleep(subscriptionDelayTime);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player1Impact.ServiceUUID,
                        Player1Impact.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {

                            _state = States.None;


                            // if (BitConverter.IsLittleEndian)
                            //   Array.Reverse(bytes);

                            var sBytes = bytes[0];

                            if (sBytes >= 4)
                                CheckBigHit(IsPlayer1Red, true, sBytes);

                            CheckPlayerPoints(IsPlayer1Red, true, sBytes);

                            CheckPlayerCombo(IsPlayer1Red, true, sBytes);

                        });

                        Thread.Sleep(subscriptionDelayTime);

                        BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, Player2Impact.ServiceUUID,
                        Player2Impact.CharacteristicUUID, null, (deviceAddress, characteristric, bytes) => {

                            _state = States.None;


                            //if (BitConverter.IsLittleEndian)
                            //  Array.Reverse(bytes);

                            var sBytes = bytes[0];

                            if (sBytes >= 4)
                                CheckBigHit(IsPlayer1Red, false, sBytes);

                            CheckPlayerPoints(IsPlayer1Red, false, sBytes);

                            CheckPlayerCombo(IsPlayer1Red, false, sBytes);

                        });
                        //REMOVE CONNECT INFO
                        ConnectInfo.SetActive(false);
                        connectionMode.text = "DISCONNECT";
                        SceneManager.GetComponent<Screen_Manager>().PlayerSelect();
                        nextButton.interactable = true;
                        break;



                    case States.Disconnect:
                        SetState(States.Disconnecting, 5f);
                        if (_connected)
                        {
                            BluetoothLEHardwareInterface.DisconnectPeripheral(_deviceAddress, (address) => {
                                // since we have a callback for disconnect in the connect method above, we don't
                                // need to process the callback here.
                            });
                        }
                        else
                        {
                            Reset();
                            SetState(States.Scan, 1f);
                        }
                        break;

                    case States.Disconnecting:
                        // if we got here we timed out disconnecting, so just go to disconnected state
                        Reset();
                        SetState(States.Scan, 1f);
                        break;
                }
            }
        }
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
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

    //After this line, the first statement usually refer to player 1 being hit and player 2 gain points
    void TriggerPlayerDeath(bool IsP1Red, bool Player1GotHit)
    {
        if ((IsP1Red && Player1GotHit) || (!IsP1Red && !Player1GotHit))
        {
            winText.color = playerTwoColor;
            playerName.color = playerTwoColor;
            playerName.text = FindObjectOfType<MenuManager>().getP1name();

        }
        if ((!IsP1Red && Player1GotHit) || (IsP1Red && !Player1GotHit))
        {
            playerName.color = playerOneColor;
            winText.color = playerOneColor;
            playerName.text = FindObjectOfType<MenuManager>().getP2name();
        }
        animatorPlayer.SetTrigger("Win");
        //animatorWins.SetTrigger("Win");
    }

    void CheckBigHit(bool IsP1Red, bool Player1GotHit, int bytes)
    {
        if ((IsP1Red && Player1GotHit) || (!IsP1Red && !Player1GotHit))
        {
            p2_bigHit.SetTrigger("Hit");
            animatorP2BigHit.SetTrigger("Hit");
            //BigHitText.text = "P2 GOT A BIG HIT!";
            P2BigHitHealthLoss = bytes * 100.0f / 24.0f;
            P2BigHitText.text = Mathf.RoundToInt(P2BigHitHealthLoss) + "%";
        }

        if ((IsP1Red && !Player1GotHit) || (!IsP1Red && Player1GotHit))
        {
             p1_bigHit.SetTrigger("Hit");
            animatorP1BigHit.SetTrigger("Hit");
            //BigHitText.text = "P1 GOT A BIG HIT!";
            P1BigHitHealthLoss = bytes * 100.0f / 24.0f;
            P1BigHitText.text = Mathf.RoundToInt(P1BigHitHealthLoss) + "%";
        }
    }
    void CheckPlayerCombo(bool IsP1Red, bool Player1GotHit, int bytes)
    {
        if ((Player1GotHit && IsP1Red) || (!Player1GotHit && !IsP1Red))
        {
            if (IsCombo2On)
            {
                if (ComboTimer2 < 2)
                {
                    ComboTimer2 = 0.0f;
                    P2ComboHits += 1;
                    P2HealthLoss += bytes * 100.0f / 24.0f;
                    //SensorBugStatusText.text = "Player 2 got a combo!";
                    if (ComboTimer2 > 0.0f && ComboTimer2 <= 1.0f)
                    {
                        P2Points += 5000;
                    }
                    else if (ComboTimer2 > 1.0f && ComboTimer2 <= 2.0f)
                    {
                        P2Points += 3000;
                    }
                    //ComboTimer2 = 0.0f;
                    animatorP2Combo.SetBool("Combo", true);
                    p2_combo.SetBool("Combo", true);
                }
                else
                {
                    //SensorBugStatusText.text = "Reset combo 2 timer";
                    //animatorP2Combo.SetBool("Combo", false);
                    ComboTimer2 = 0.0f;
                    P2HealthLoss = bytes * 100.0f / 24.0f;
                    P2ComboHits = 1;
                    
                }
            }
            else
            {
                IsCombo2On = true;
                P2HealthLoss = bytes * 100.0f / 24.0f;
                P2ComboHits = 1;
            }
            P2ComboText.text = P2ComboHits.ToString();
            P2TotalDamagedAmount.text = Mathf.RoundToInt(P2HealthLoss) + "%";
            if (ThreeHitCombo2On)
            {
                if (ThreeHitTimer2 <= 3.0f)
                {
                    ThreeHitCountP2 += 1;

                    if (ThreeHitCountP2 >= 3)
                    {
                        P2Points += 10000;
                        //ThreeHitCountP2 = 0;
                        //ThreeHitTimer2 = 0.0f;
                    }
                }
                else
                {
                    ThreeHitCountP2 = 1;
                    ThreeHitTimer2 = 0.0f;
                }
            }
            else
            {
                ThreeHitCombo2On = true;
                ThreeHitCountP2 = 1;
            }
        }

        if ((Player1GotHit && !IsP1Red) || (!Player1GotHit && IsP1Red))
        {
            if (IsCombo1On)
            {
                if (ComboTimer1 < 2)
                {
                    ComboTimer1 = 0.0f;
                    P1ComboHits += 1;
                    P1HealthLoss += bytes * 100.0f / 24.0f;
                    //SensorBugStatusText.text = "Player 1 got a combo!";
                    if (ComboTimer1 > 0.0f && ComboTimer1 <= 1.0f)
                    {
                        P1Points += 5000;
                    }
                    else if (ComboTimer1 > 1.0f && ComboTimer1 <= 2.0f)
                    {
                        P1Points += 3000;
                    }
                    //ComboTimer1 = 0.0f;
                    animatorP1Combo.SetBool("Combo", true);
                    p1_combo.SetBool("Combo", true);
                }
                else
                {
                    //SensorBugStatusText.text = "Reset combo 1 timer";
                    //animatorP1Combo.SetBool("Combo", false);
                    ComboTimer1 = 0.0f;
                    P1HealthLoss = bytes * 100.0f / 24.0f;
                    P1ComboHits = 1;
                }
            }
            else
            {
                IsCombo1On = true;
                P1HealthLoss = bytes * 100.0f / 24.0f;
                P1ComboHits = 1;
            }

            P1ComboText.text = P1ComboHits.ToString();
            P1TotalDamagedAmount.text = Mathf.RoundToInt(P1HealthLoss) + "%";

            if (ThreeHitCombo1On)
            {
                if (ThreeHitTimer1 <= 3.0f)
                {
                    ThreeHitCountP1 += 1;

                    if (ThreeHitCountP1 >= 3)
                    {
                        P1Points += 10000;

                        //ThreeHitTimer1 = 0.0f;
                    }
                }
                else
                {
                    ThreeHitCountP1 = 1;
                    ThreeHitTimer1 = 0.0f;
                }
            }
            else
            {
                ThreeHitCombo1On = true;
                ThreeHitCountP1 = 1;
            }
        }
    }

    void CheckPlayerColor(string bytes)
    {
        if (bytes == "01")
            IsPlayer1Red = true;
        else
            IsPlayer1Red = false;
    }

    void CheckPlayerPoints(bool IsP1Red, bool Player1GotHit, int bytes)
    {
        //Check p2 points
        if ((IsP1Red && Player1GotHit) || (!IsP1Red && !Player1GotHit))
        {
            //Regular score
            if (bytes == 1)
            {
                P2Points += 1000;
            }
            else if (bytes == 2)
            {
                P2Points += 3000;
            }
            else if (bytes == 3)
            {
                P2Points += 6000;
            }
            else if (bytes >= 4)
            {
                P2Points += 10000;
            }
            //Counter score
            if (ComboTimer1 > 0 && ComboTimer1 <= 1.0f)
                P2Points += 5000;
        }
        //Check p1 points
        if ((IsP1Red && !Player1GotHit) || (!IsP1Red && Player1GotHit))
        {
            //Regular score
            if (bytes == 1)
            {
                P1Points += 1000;
            }
            else if (bytes == 2)
            {
                P1Points += 3000;
            }
            else if (bytes == 3)
            {
                P1Points += 6000;
            }
            else if (bytes >= 4)
            {
                P1Points += 10000;
            }
            //Counter score
            if (ComboTimer2 > 0 && ComboTimer2 <= 1.0f)
                P1Points += 5000;
        }
    }

    public void setp1name(string name)
    {
        p1name = name;
    }
    public void setp2name(string name)
    {
        p2name = name;
    }
}