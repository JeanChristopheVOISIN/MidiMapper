using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Threading.Channels;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using Microsoft.VisualBasic;

namespace MidiMapper
{
    public partial class Principal : Form
    {
        private static string globalPresetData = @"{""id"": 0,""version"":"""", ""name"": """",""desc"": """",""cmd"": [],""led"": [],""buttons"": [], ""pedals"": []}";
        JsonNode preset = JsonNode.Parse(globalPresetData);

        DeviceWatcher deviceWatcher;
        DeviceInformationCollection midiInputDevices;
        DeviceInformationCollection midiOutputDevices;
        MidiInPort midiInDevicePort;
        IMidiOutPort midiOutDevicePort;
        MidiInPort midiInLoopPort;
        IMidiOutPort midiOutLoopPort;

        private byte device1_layerA = 127, device1_layerB = 0, device1_button17 = 127, device1_button18 = 0;
        private byte device1_c1cc19;
        Dictionary<Tuple<byte, byte>, byte> midiVariable = new Dictionary<Tuple<byte, byte>, byte>();
        public Principal()
        {
            InitializeComponent();

            deviceWatcher = DeviceInformation.CreateWatcher(MidiInPort.GetDeviceSelector());
            deviceWatcher.Added += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Removed += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Updated += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Start();
        }
        ~Principal()
        {
            deviceWatcher.Stop();
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }
        private async void EnumerateMidiInputDevices()
        {
            // Find all input MIDI devices
            midiInputDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());
            if (midiInputDevices.Count > 0)
            {
                foreach (DeviceInformation deviceInfo in midiInputDevices)
                {
                    if (midiInDevicePort == null && "MIDII_E61DC05C".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
                        midiInDevicePort = await MidiInPort.FromIdAsync(deviceInfo.Id);
                        midiInDevicePort.MessageReceived += MidiInDevicePort_MessageReceived;
                    }
                    if (midiInLoopPort == null && "MIDII_44881789".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
                        midiInLoopPort = await MidiInPort.FromIdAsync(deviceInfo.Id);
                        midiInLoopPort.MessageReceived += MidiInLoopPort_MessageReceived;
                    }
                }
            }

            _=this.comboBoxEntreeDevice.Invoke(new MethodInvoker(delegate
            {
                this.comboBoxEntreeDevice.Items.Clear();
                this.comboBoxEntreeLoop.Items.Clear();

                // Return if no external devices are connected
                if (midiInputDevices.Count == 0)
                {
                    this.comboBoxEntreeDevice.Items.Add("No MIDI input devices found!");
                    this.comboBoxEntreeLoop.Items.Add("No MIDI input devices found!");
                    return;
                }

                // Else, add each connected input device to the list
                foreach (DeviceInformation deviceInfo in midiInputDevices)
                {
                    System.Diagnostics.Debug.WriteLine("MidiInput {0} , {1}", deviceInfo.Id, deviceInfo.Name);
                    this.comboBoxEntreeDevice.Items.Add(deviceInfo.Name);
                    this.comboBoxEntreeLoop.Items.Add(deviceInfo.Name);
                    if (midiInDevicePort != null && midiInDevicePort.DeviceId == deviceInfo.Id)
                    {
                        comboBoxEntreeDevice.SelectedIndex = Array.IndexOf(midiInputDevices.ToArray(), deviceInfo);
                    }
                    if (midiInLoopPort != null && midiInLoopPort.DeviceId == deviceInfo.Id)
                    {
                        comboBoxEntreeLoop.SelectedIndex = Array.IndexOf(midiInputDevices.ToArray(), deviceInfo);
                    }
                }
            }));
        }
        private string getMidiNameFromMidiInputDevices(DeviceInformation deviceInfo)
        {
            string id = deviceInfo.Id;
            string name = deviceInfo.Name;

            string midiDeviceId = id.Split("#")[2].Split(".")[0];

            foreach (DeviceInformation deviceInfoInput in midiInputDevices)
            {
                string midiInputDeviceId = deviceInfoInput.Id.Split("#")[2].Split(".")[0];
                if(midiInputDeviceId == midiDeviceId)
                {
                    name = deviceInfoInput.Name;
                    break;
                }
            }
            return name;
        }
        private async void EnumerateMidiOutputDevices()
        {

            // Find all output MIDI devices
            string midiOutportQueryString = MidiOutPort.GetDeviceSelector();
            midiOutputDevices = await DeviceInformation.FindAllAsync(midiOutportQueryString);
            if (midiOutputDevices.Count > 0)
            {
                foreach (DeviceInformation deviceInfo in midiOutputDevices)
                {
                    if (midiOutDevicePort == null && "MIDII_E61DC05C".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
                        midiOutDevicePort = await MidiOutPort.FromIdAsync(deviceInfo.Id);
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 84, this.device1_layerA));
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 85, this.device1_layerB));
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 87, this.device1_button17));
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 88, this.device1_button18));
                        led(48, 1);
                        led(49, 1);
                        led(50, 1);
                        led(51, 1);
                        led(52, 1);
                        led(53, 1);
                        led(54, 1);
                        led(55, 1);
                    }
                    if (midiOutLoopPort == null && "MIDII_4488178A".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
                        midiOutLoopPort = await MidiOutPort.FromIdAsync(deviceInfo.Id);
                    }
                }
            }

            _=this.comboBoxSortieDevice.Invoke(new MethodInvoker(delegate
            {
                this.comboBoxSortieDevice.Items.Clear();
                this.comboBoxSortieLoop.Items.Clear();

                // Return if no external devices are connected
                if (midiOutputDevices.Count == 0)
                {
                    this.comboBoxSortieDevice.Items.Add("No MIDI output devices found!");
                    this.comboBoxSortieLoop.Items.Add("No MIDI output devices found!");
                    return;
                }

                // Else, add each connected input device to the list
                foreach (DeviceInformation deviceInfo in midiOutputDevices)
                {
                    System.Diagnostics.Debug.WriteLine("MidiOutput {0} , {1}", deviceInfo.Id, deviceInfo.Name);
                    this.comboBoxSortieDevice.Items.Add(getMidiNameFromMidiInputDevices(deviceInfo));
                    this.comboBoxSortieLoop.Items.Add(getMidiNameFromMidiInputDevices(deviceInfo));
                    if (midiOutDevicePort != null && midiOutDevicePort.DeviceId == deviceInfo.Id)
                    {
                        comboBoxSortieDevice.SelectedIndex = Array.IndexOf(midiOutputDevices.ToArray(), deviceInfo);
                    }
                    if (midiOutLoopPort != null && midiOutLoopPort.DeviceId == deviceInfo.Id)
                    {
                        comboBoxSortieLoop.SelectedIndex = Array.IndexOf(midiOutputDevices.ToArray(), deviceInfo);
                    }
                }
            }));
        }
        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            this.EnumerateMidiInputDevices();
            this.EnumerateMidiOutputDevices();
        }
        private async void midiInDevicePortListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (midiInputDevices == null)
            {
                return;
            }

            DeviceInformation devInfo = midiInputDevices[comboBoxEntreeDevice.SelectedIndex];

            if (devInfo == null)
            {
                return;
            }
            // remove event in the last device
            if(midiInDevicePort != null)
            {
                midiInDevicePort.MessageReceived -= MidiInDevicePort_MessageReceived;
            }
            midiInDevicePort = await MidiInPort.FromIdAsync(devInfo.Id);

            if (midiInDevicePort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                return;
            }
            midiInDevicePort.MessageReceived += MidiInDevicePort_MessageReceived;
        }
        private async void midiOutDevicePortListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (midiOutputDevices == null)
            {
                return;
            }

            DeviceInformation devInfo = midiOutputDevices[comboBoxSortieDevice.SelectedIndex];

            if (devInfo == null)
            {
                return;
            }

            midiOutDevicePort = await MidiOutPort.FromIdAsync(devInfo.Id);

            if (midiOutDevicePort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
                return;
            }

        }
        private async void midiInLoopPortListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (midiInputDevices == null)
            {
                return;
            }

            DeviceInformation devInfo = midiInputDevices[comboBoxEntreeLoop.SelectedIndex];

            if (devInfo == null)
            {
                return;
            }
            // remove event in the last device
            if (midiInLoopPort != null)
            {
                midiInLoopPort.MessageReceived -= MidiInLoopPort_MessageReceived;
            }
            midiInLoopPort = await MidiInPort.FromIdAsync(devInfo.Id);

            if (midiInLoopPort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                return;
            }
            midiInLoopPort.MessageReceived += MidiInLoopPort_MessageReceived;
        }
        private async void midiOutLoopPortListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (midiOutputDevices == null)
            {
                return;
            }

            DeviceInformation devInfo = midiOutputDevices[comboBoxSortieLoop.SelectedIndex];

            if (devInfo == null)
            {
                return;
            }

            midiOutLoopPort = await MidiOutPort.FromIdAsync(devInfo.Id);

            if (midiOutLoopPort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
                return;
            }

        }
        private void led(byte number, byte value)
        {
            midiOutDevicePort.SendMessage(new MidiControlChangeMessage(0, number, (byte)(value*10/127+33)));// led ring 1
        }
        private byte add(byte variable, byte value)
        {
            return (byte)(((variable + value) > 127) ? 127 : (variable + value));
        }
        private byte sub(byte variable, byte value)
        {
            return (byte)(((variable - (value - 64)) < 0) ? 0 : (variable - (value - 64)));
        }
        private byte getStoreMidi(byte channel, byte number)
        {
            return (midiVariable.ContainsKey(new Tuple<byte,byte>(channel,number))) ? midiVariable[new Tuple<byte, byte>(channel, number)] : (byte)0;
        }
        private void storeMidi(byte channel, byte number, byte value)
        {
            if(midiVariable.ContainsKey(new Tuple<byte, byte>(channel, number)))
            {
                midiVariable[new Tuple<byte, byte>(channel, number)] = value;
            } else
            {
                midiVariable.Add(new Tuple<byte, byte>(channel, number), value);
            }
        }
        private void rotary(byte chanSend, byte ccSend, byte value)
        {
            if (value >= 1 && value <= 10)
            {
                storeMidi(chanSend, ccSend, add(getStoreMidi(chanSend, ccSend), value));
            }
            if (value >= 65 && value <= 75)
            {
                storeMidi(chanSend, ccSend, sub(getStoreMidi(chanSend, ccSend), value));
            }
            midiOutLoopPort.SendMessage(new MidiControlChangeMessage(chanSend, ccSend, getStoreMidi(chanSend, ccSend)));
        }
        private void rotary(byte chanSend, byte ccSend, byte value, byte numLed)
        {
            rotary(chanSend, ccSend, value);
            led(numLed, getStoreMidi(chanSend, ccSend));
        }
        private void readMidiMessage(MidiInPort sender, IMidiMessage receivedMidiMessage)
        {
            if (sender == midiInDevicePort)
            {
                if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
                {
                    //System.Diagnostics.Debug.WriteLine("<- {0} / ControlChange {1} , {2} , {3}",
                    //    sender.DeviceId.Split("#")[2].Split(".")[0],
                    //    ((MidiControlChangeMessage)receivedMidiMessage).Channel,
                    //    ((MidiControlChangeMessage)receivedMidiMessage).Controller,
                    //    ((MidiControlChangeMessage)receivedMidiMessage).ControlValue);
                    byte channel = ((MidiControlChangeMessage)receivedMidiMessage).Channel;
                    byte number = ((MidiControlChangeMessage)receivedMidiMessage).Controller;
                    byte value = ((MidiControlChangeMessage)receivedMidiMessage).ControlValue;
                    if (channel == 0 && number == 16)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 1, value, 48);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 11, value, 48);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 1, value, 48);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 11, value, 48);
                        }
                    }

                    if (channel == 0 && number == 17)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 2, value, 49);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 12, value, 49);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 2, value, 49);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 12, value, 49);
                        }
                    }

                    if (channel == 0 && number == 18)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 3, value, 50);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 13, value, 50);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 3, value, 50);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 13, value, 50);
                        }
                    }

                    if (channel == 0 && number == 19)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 4, value, 51);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 14, value, 51);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 4, value, 51);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 14, value, 51);
                        }
                    }

                    if (channel == 0 && number == 20)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 5, value, 52);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 15, value, 52);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 5, value, 52);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 15, value, 52);
                        }
                    }

                    if (channel == 0 && number == 21)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 6, value, 53);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 16, value, 53);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 6, value, 53);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 16, value, 53);
                        }
                    }

                    if (channel == 0 && number == 22)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 7, value, 54);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 17, value, 54);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 7, value, 54);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 17, value, 54);
                        }
                    }

                    if (channel == 0 && number == 23)
                    {
                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127)
                        {
                            rotary(1, 8, value, 55);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button17 == 127)
                        {
                            rotary(1, 18, value, 55);
                        }

                        if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127)
                        {
                            rotary(2, 8, value, 55);
                        }
                        if (this.device1_layerA == 0  && this.device1_layerB == 127 && this.device1_button18 == 127)
                        {
                            rotary(2, 18, value, 55);
                        }
                    }
                }
                if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
                {
                    //System.Diagnostics.Debug.WriteLine("<- {0} / NoteOn {1} , {2} , {3}",
                    //    sender.DeviceId.Split("#")[2].Split(".")[0],
                    //    ((MidiNoteOnMessage)receivedMidiMessage).Channel,
                    //    ((MidiNoteOnMessage)receivedMidiMessage).Note,
                    //    ((MidiNoteOnMessage)receivedMidiMessage).Velocity);
                    byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                    byte number = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                    byte value = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                    // Sélection Button 9
                    if (channel == 0 && number == 89 && value == 127)
                    {
                        this.device1_c1cc19 = (byte)(value - this.device1_c1cc19);
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 89, this.device1_c1cc19)); // button 1
                        midiOutLoopPort.SendMessage(new MidiControlChangeMessage(1, 19, this.device1_c1cc19));// Button9Aux1 sendMute Track1Aux1
                    }

                    // Sélection Button 17
                    if (channel == 0 && number == 87 && value == 127)
                    {
                        this.device1_button17 = value;
                        this.device1_button18 = 0;
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 87, this.device1_button17));
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 88, this.device1_button18));
                        if (this.device1_layerA == 127 && this.device1_layerB == 0)
                        {
                            led(48, getStoreMidi(1, 1));
                            led(49, getStoreMidi(1, 2));
                            led(50, getStoreMidi(1, 3));
                            led(51, getStoreMidi(1, 4));
                            led(52, getStoreMidi(1, 5));
                            led(53, getStoreMidi(1, 6));
                            led(54, getStoreMidi(1, 7));
                            led(55, getStoreMidi(1, 8));
                        }
                        if (this.device1_layerA == 0 && this.device1_layerB == 127)
                        {
                            led(48, getStoreMidi(1, 11));
                            led(49, getStoreMidi(1, 12));
                            led(50, getStoreMidi(1, 13));
                            led(51, getStoreMidi(1, 14));
                            led(52, getStoreMidi(1, 15));
                            led(53, getStoreMidi(1, 16));
                            led(54, getStoreMidi(1, 17));
                            led(55, getStoreMidi(1, 18));
                        }
                    }
                    // Sélection Button 18
                    if (channel == 0 && number == 88 && value == 127)
                    {
                        this.device1_button17 = 0;
                        this.device1_button18 = value;
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 87, this.device1_button17));
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 88, this.device1_button18));
                        if (this.device1_layerA == 127 && this.device1_layerB == 0)
                        {
                            led(48, getStoreMidi(2, 1));
                            led(49, getStoreMidi(2, 2));
                            led(50, getStoreMidi(2, 3));
                            led(51, getStoreMidi(2, 4));
                            led(52, getStoreMidi(2, 5));
                            led(53, getStoreMidi(2, 6));
                            led(54, getStoreMidi(2, 7));
                            led(55, getStoreMidi(2, 8));
                        }
                        if (this.device1_layerA == 0 && this.device1_layerB == 127)
                        {
                            led(48, getStoreMidi(2, 11));
                            led(49, getStoreMidi(2, 12));
                            led(50, getStoreMidi(2, 13));
                            led(51, getStoreMidi(2, 14));
                            led(52, getStoreMidi(2, 15));
                            led(53, getStoreMidi(2, 16));
                            led(54, getStoreMidi(2, 17));
                            led(55, getStoreMidi(2, 18));
                        }
                    }

                    // Sélection Layer A
                    if (channel == 0 && number == 84 && value == 127)
                    {
                        if(this.device1_layerA == 0 || this.device1_layerA == 127 && this.device1_layerB == 127)
                        {
                            this.device1_layerA = value;
                            this.device1_layerB = 0;
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 84, this.device1_layerA));
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 85, this.device1_layerB));
                            if (this.device1_button17 == 127)
                            {
                                led(48, getStoreMidi(1, 1));
                                led(49, getStoreMidi(1, 2));
                                led(50, getStoreMidi(1, 3));
                                led(51, getStoreMidi(1, 4));
                                led(52, getStoreMidi(1, 5));
                                led(53, getStoreMidi(1, 6));
                                led(54, getStoreMidi(1, 7));
                                led(55, getStoreMidi(1, 8));
                            }
                            if (this.device1_button18 == 127)
                            {
                                led(48, getStoreMidi(2, 1));
                                led(49, getStoreMidi(2, 2));
                                led(50, getStoreMidi(2, 3));
                                led(51, getStoreMidi(2, 4));
                                led(52, getStoreMidi(2, 5));
                                led(53, getStoreMidi(2, 6));
                                led(54, getStoreMidi(2, 7));
                                led(55, getStoreMidi(2, 8));
                            }
                        }
                        else
                        {
                            this.device1_layerA = value;
                            this.device1_layerB = value;
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 84, 1));
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 85, 1));
                            led(48, 0);
                            led(49, 0);
                            led(50, 0);
                            led(51, 0);
                            led(52, 0);
                            led(53, 0);
                            led(54, 0);
                            led(55, 0);
                        }
                    }

                    // Sélection Layer B
                    if (channel == 0 && number == 85 && value == 127)
                    {
                        if (this.device1_layerB == 0 || this.device1_layerA == 127 && this.device1_layerB == 127)
                        {
                            this.device1_layerB = value;
                            this.device1_layerA = 0;
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 84, this.device1_layerA));
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 85, this.device1_layerB));
                            if (this.device1_button17 == 127)
                            {
                                led(48, getStoreMidi(1, 11));
                                led(49, getStoreMidi(1, 12));
                                led(50, getStoreMidi(1, 13));
                                led(51, getStoreMidi(1, 14));
                                led(52, getStoreMidi(1, 15));
                                led(53, getStoreMidi(1, 16));
                                led(54, getStoreMidi(1, 17));
                                led(55, getStoreMidi(1, 18));
                            }
                            if (this.device1_button18 == 127)
                            {
                                led(48, getStoreMidi(2, 11));
                                led(49, getStoreMidi(2, 12));
                                led(50, getStoreMidi(2, 13));
                                led(51, getStoreMidi(2, 14));
                                led(52, getStoreMidi(2, 15));
                                led(53, getStoreMidi(2, 16));
                                led(54, getStoreMidi(2, 17));
                                led(55, getStoreMidi(2, 18));
                            }
                        }
                        else
                        {
                            this.device1_layerA = value;
                            this.device1_layerB = value;
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 84, 1));
                            midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 85, 1));
                            led(48, 0);
                            led(49, 0);
                            led(50, 0);
                            led(51, 0);
                            led(52, 0);
                            led(53, 0);
                            led(54, 0);
                            led(55, 0);
                        }
                    }
                }
            }
            if (sender == midiInLoopPort)
            {
                if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
                {
                    //System.Diagnostics.Debug.WriteLine("<- {0} / ControlChange {1} , {2} , {3}",
                    //    sender.DeviceId.Split("#")[2].Split(".")[0],
                    //    ((MidiControlChangeMessage)receivedMidiMessage).Channel,
                    //    ((MidiControlChangeMessage)receivedMidiMessage).Controller,
                    //    ((MidiControlChangeMessage)receivedMidiMessage).ControlValue);
                    byte channel = ((MidiControlChangeMessage)receivedMidiMessage).Channel;
                    byte number = ((MidiControlChangeMessage)receivedMidiMessage).Controller;
                    byte value = ((MidiControlChangeMessage)receivedMidiMessage).ControlValue;
                    var listChannel = new List<byte> { 1, 2 };
                    if (listChannel.Contains(channel))
                    {
                        var listNumber = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8 };
                        if (listNumber.Contains(number))
                        {
                            storeMidi(channel, number, value);
                            if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button17 == 127 && channel == 1)
                            {
                                led((byte)(47 + number), getStoreMidi(channel, number));
                            }
                            if (this.device1_layerA == 127 && this.device1_layerB == 0 && this.device1_button18 == 127 && channel == 2)
                            {
                                led((byte)(47 + number), getStoreMidi(channel, number));
                            }
                        }
                        listNumber = new List<byte> { 11, 12, 13, 14, 15, 16, 17, 18 };
                        if (listNumber.Contains(number))
                        {
                            storeMidi(channel, number, value);
                            if (this.device1_layerA == 0 && this.device1_layerB == 127 && this.device1_button17 == 127 && channel == 1)
                            {
                                led((byte)(37 + number), getStoreMidi(channel, number));
                            }
                            if (this.device1_layerA == 0 && this.device1_layerB == 127 && this.device1_button18 == 127 && channel == 2)
                            {
                                led((byte)(37 + number), getStoreMidi(channel, number));
                            }
                        }
                    }
                    if (channel == 1 && number == 19)
                    {
                        this.device1_c1cc19 = value;
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 89, this.device1_c1cc19)); // button 1
                    }
                }
            }

            //if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / NoteOn {1} , {2} , {3}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        ((MidiNoteOnMessage)receivedMidiMessage).Channel,
            //        ((MidiNoteOnMessage)receivedMidiMessage).Note,
            //        ((MidiNoteOnMessage)receivedMidiMessage).Velocity);
            //}
            //else if (receivedMidiMessage.Type == MidiMessageType.NoteOff)
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / NoteOff {1} , {2} , {3}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        ((MidiNoteOnMessage)receivedMidiMessage).Channel,
            //        ((MidiNoteOnMessage)receivedMidiMessage).Note,
            //        ((MidiNoteOnMessage)receivedMidiMessage).Velocity);
            //}
            //else if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / ControlChange {1} , {2} , {3}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        ((MidiControlChangeMessage)receivedMidiMessage).Channel,
            //        ((MidiControlChangeMessage)receivedMidiMessage).Controller,
            //        ((MidiControlChangeMessage)receivedMidiMessage).ControlValue);
            //}
            //else if (receivedMidiMessage.Type == MidiMessageType.ProgramChange)
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / ProgramChange {1} ; {2}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        ((MidiProgramChangeMessage)receivedMidiMessage).Channel,
            //        ((MidiProgramChangeMessage)receivedMidiMessage).Program);
            //}
            //else if (receivedMidiMessage.Type == MidiMessageType.SongSelect)
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / SongSelect {1}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        ((MidiSongSelectMessage)receivedMidiMessage).Song);
            //}
            //else if (receivedMidiMessage.Type == MidiMessageType.PitchBendChange)
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / PitchBendChange {1} ; {2}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        ((MidiPitchBendChangeMessage)receivedMidiMessage).Channel,
            //        ((MidiPitchBendChangeMessage)receivedMidiMessage).Bend
            //        );
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("<- {0} / {1}",
            //        sender.DeviceId.Split("#")[2].Split(".")[0],
            //        receivedMidiMessage.ToString());
            //}
            //System.Diagnostics.Debug.WriteLine("-> Send to device {0}", receivedMidiMessage);
            //midiOutDevicePort.SendMessage(receivedMidiMessage);
        }
        private void MidiInDevicePort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            readMidiMessage(sender, args.Message);
        }
        private void MidiInLoopPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            readMidiMessage(sender, args.Message);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-> ControlChange {0} , {1} , {2}", 0, byte.Parse(textBox1.Text), byte.Parse(textBox2.Text));
            Windows.Devices.Midi.IMidiMessage midiMessageToSend = new MidiControlChangeMessage(0, byte.Parse(textBox1.Text), byte.Parse(textBox2.Text));
            midiOutDevicePort.SendMessage(midiMessageToSend);
        }
        private void buttonLoop_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-> ControlChange {0} , {1} , {2}", 0, byte.Parse(textBox1.Text), byte.Parse(textBox2.Text));
            Windows.Devices.Midi.IMidiMessage midiMessageToSend = new MidiControlChangeMessage(0, byte.Parse(textBox1.Text), byte.Parse(textBox2.Text));
            midiOutLoopPort.SendMessage(midiMessageToSend);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-> NoteOn {0} , {1} , {2}", 0, byte.Parse(textBox4.Text), byte.Parse(textBox3.Text));
            Windows.Devices.Midi.IMidiMessage midiMessageToSend = new MidiNoteOnMessage(0, byte.Parse(textBox4.Text), byte.Parse(textBox3.Text));
            midiOutDevicePort.SendMessage(midiMessageToSend);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-> ProgramChange {0} , {1}", 0, byte.Parse(textBox6.Text));
            Windows.Devices.Midi.IMidiMessage midiMessageToSend = new MidiProgramChangeMessage(0, byte.Parse(textBox6.Text));
            midiOutDevicePort.SendMessage(midiMessageToSend);
        }
        private void buttonPitch_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-> PitchBendChange {0} , {1}", 0, ushort.Parse(textBox6.Text));
            Windows.Devices.Midi.IMidiMessage midiMessageToSend = new MidiPitchBendChangeMessage(0, ushort.Parse(textBox6.Text));
            midiOutDevicePort.SendMessage(midiMessageToSend);
        }
    }
}