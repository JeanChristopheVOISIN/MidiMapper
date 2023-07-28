using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Threading.Channels;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;

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

        private byte device1_cc1, device1_cc19, device1_layerA, device1_layerB;
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
                    if (midiInDevicePort == null && "MIDII_E61DC05C".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
                    }
                    if (midiInLoopPort == null && "MIDII_44881789".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
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

            _=this.comboBoxEntreeDevice.Invoke(new MethodInvoker(delegate
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
                    if (midiOutDevicePort == null && "MIDII_E61DC05C".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
                    }
                    if (midiOutLoopPort == null && "MIDII_4488178A".Equals(deviceInfo.Id.Split("#")[2].Split(".")[0]))
                    {
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
                    if (channel == 0 && number == 16 && value >= 1 && value <= 10)
                    {
                        this.device1_cc1 = (byte)(((this.device1_cc1 + value) > 127) ? 127 : (this.device1_cc1 + value));
                        midiOutDevicePort.SendMessage(new MidiControlChangeMessage(0, 48, (byte)(this.device1_cc1*10/127+33)));// led ring 1
                        midiOutLoopPort.SendMessage(new MidiControlChangeMessage(1, 1, this.device1_cc1));// Track1Aux1
                    }
                    if (channel == 0 && number == 16 && value >= 65 && value <= 75)
                    {
                        this.device1_cc1 = (byte)(((this.device1_cc1 - (value - 64)) < 0) ? 0 : (this.device1_cc1 - (value - 64)));
                        midiOutDevicePort.SendMessage(new MidiControlChangeMessage(0, 48, (byte)(this.device1_cc1*10/127+33)));// led ring 1
                        midiOutLoopPort.SendMessage(new MidiControlChangeMessage(1, 1, this.device1_cc1));// Track1Aux1
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
                    if (channel == 0 && number == 89 && value == 127)
                    {
                        this.device1_cc19 = (byte)(value - this.device1_cc19);
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 89, this.device1_cc19)); // button 1
                        midiOutLoopPort.SendMessage(new MidiControlChangeMessage(1, 19, this.device1_cc19));// Button9Aux1 sendMute Track1Aux1
                    }

                    // Sélection Layer A
                    if (channel == 0 && number == 84 && value == 127)
                    {
                        this.device1_layerA = (byte)(value - this.device1_layerA);
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 84, this.device1_layerA));
                    }

                    // Sélection Layer B
                    if (channel == 0 && number == 85 && value == 127)
                    {
                        this.device1_layerB = (byte)(value - this.device1_layerB);
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 85, this.device1_layerB)); 
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
                    if (channel == 1 && number == 1)
                    {
                        this.device1_cc1 = value;
                        midiOutDevicePort.SendMessage(new MidiControlChangeMessage(0, 48, (byte)(device1_cc1*10/127+33)));// led ring 1
                    }
                    if (channel == 1 && number == 19)
                    {
                        this.device1_cc19 = value;
                        midiOutDevicePort.SendMessage(new MidiNoteOnMessage(0, 89, this.device1_cc19)); // button 1
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