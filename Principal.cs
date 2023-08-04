using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Text.Json.Nodes;
using System.Collections.Concurrent;

namespace MidiMapper
{
    public partial class Principal : Form
    {
        // Chargement depuis un JSON avec AUX par défaut
        // liste des aux à sélectionner par device
        //private string[] DEVICE_ID = { "MIDII_E61DC05C", "MIDII_DCC6F3AD" };
        //private string LOOP_IN_DEVICE_ID = "MIDII_44881789";
        //private string LOOP_OUT_DEVICE_ID = "MIDII_4488178A";
        private List<string> DEVICE_ID = new List<string>();
        private List<string> DEVICE_AUX = new List<string>();
        private string LOOP_IN_DEVICE_ID = "";
        private string LOOP_OUT_DEVICE_ID = "";

        private static byte DEVICE_CHANNEL = 0;
        private static byte RING_LED1 = 48;
        private static byte RING_LED2 = 49;
        private static byte RING_LED3 = 50;
        private static byte RING_LED4 = 51;
        private static byte RING_LED5 = 52;
        private static byte RING_LED6 = 53;
        private static byte RING_LED7 = 54;
        private static byte RING_LED8 = 55;
        Dictionary<byte, byte> ALL_LED = new Dictionary<byte, byte>() { { 1, RING_LED1 }, { 2, RING_LED2 }, { 3, RING_LED3 }, { 4, RING_LED4 }, { 5, RING_LED5 }, { 6, RING_LED6 }, { 7, RING_LED7 }, { 8, RING_LED8 } };

        private static byte PRESET_A_LED = 84;
        private static byte PRESET_B_LED = 85;
        private static byte PRESET_A_NOTE = 84;
        private static byte PRESET_B_NOTE = 85;

        private static byte PRESET_AUX_1 = 87;
        private static byte PRESET_AUX_2 = 88;
        private static byte PRESET_AUX_3 = 91;
        private static byte PRESET_AUX_4 = 92;
        private static byte PRESET_AUX_5 = 86;
        private static byte PRESET_AUX_6 = 93;
        private static byte PRESET_AUX_7 = 94;
        private static byte PRESET_AUX_8 = 95;

        private static byte AUX_1_CHANNEL = 1;
        private static byte AUX_2_CHANNEL = 2;
        private static byte AUX_3_CHANNEL = 3;
        private static byte AUX_4_CHANNEL = 4;
        private static byte AUX_5_CHANNEL = 5;
        private static byte AUX_6_CHANNEL = 6;
        private static byte AUX_7_CHANNEL = 7;
        private static byte AUX_8_CHANNEL = 8;

        private static byte BUTTON1 = 32;
        private static byte BUTTON2 = 33;
        private static byte BUTTON3 = 34;
        private static byte BUTTON4 = 35;
        private static byte BUTTON5 = 36;
        private static byte BUTTON6 = 37;
        private static byte BUTTON7 = 38;
        private static byte BUTTON8 = 39;

        private static byte BUTTON9 = 89;
        private static byte BUTTON10 = 90;
        private static byte BUTTON11 = 40;
        private static byte BUTTON12 = 41;
        private static byte BUTTON13 = 42;
        private static byte BUTTON14 = 43;
        private static byte BUTTON15 = 44;
        private static byte BUTTON16 = 45;

        JsonNode config;

        DeviceWatcher deviceWatcher;
        DeviceInformationCollection midiInputDevices;
        DeviceInformationCollection midiOutputDevices;
        ConcurrentDictionary<string, MidiInPort> midiInDevicePort = new ConcurrentDictionary<string, MidiInPort>();
        ConcurrentDictionary<string, IMidiOutPort> midiOutDevicePort = new ConcurrentDictionary<string, IMidiOutPort>();
        MidiInPort midiInLoopPort;
        IMidiOutPort midiOutLoopPort;

        private static string AUX1_1_8 = "AUX1_1_8";
        private static string AUX1_9_16 = "AUX1_9_16";
        private static string AUX1_MASTER = "AUX1_MASTER";
        private static string AUX2_1_8 = "AUX2_1_8";
        private static string AUX2_9_16 = "AUX2_9_16";
        private static string AUX2_MASTER = "AUX2_MASTER";
        private static string AUX3_1_8 = "AUX3_1_8";
        private static string AUX3_9_16 = "AUX3_9_16";
        private static string AUX3_MASTER = "AUX3_MASTER";
        private static string AUX4_1_8 = "AUX4_1_8";
        private static string AUX4_9_16 = "AUX4_9_16";
        private static string AUX4_MASTER = "AUX4_MASTER";
        private static string AUX5_1_8 = "AUX5_1_8";
        private static string AUX5_9_16 = "AUX5_9_16";
        private static string AUX5_MASTER = "AUX5_MASTER";
        private static string AUX6_1_8 = "AUX6_1_8";
        private static string AUX6_9_16 = "AUX6_9_16";
        private static string AUX6_MASTER = "AUX6_MASTER";
        private static string AUX7_1_8 = "AUX7_1_8";
        private static string AUX7_9_16 = "AUX7_9_16";
        private static string AUX7_MASTER = "AUX7_MASTER";
        private static string AUX8_1_8 = "AUX8_1_8";
        private static string AUX8_9_16 = "AUX8_9_16";
        private static string AUX8_MASTER = "AUX8_MASTER";

        Dictionary<Tuple<byte, byte, byte>, string> PRESET_LIST = new Dictionary<Tuple<byte, byte, byte>, string>() {
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_1 ), AUX1_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_1 ), AUX1_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_2 ), AUX2_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_2 ), AUX2_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_3 ), AUX3_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_3 ), AUX3_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_4 ), AUX4_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_4 ), AUX4_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_5 ), AUX5_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_5 ), AUX5_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_6 ), AUX6_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_6 ), AUX6_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_7 ), AUX7_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_7 ), AUX7_9_16 },
            {new Tuple<byte, byte, byte>( 127, 0, PRESET_AUX_8 ), AUX8_1_8 },
            {new Tuple<byte, byte, byte>( 0, 127, PRESET_AUX_8 ), AUX8_9_16 },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_1 ), AUX1_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_2 ), AUX2_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_3 ), AUX3_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_4 ), AUX4_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_5 ), AUX5_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_6 ), AUX6_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_7 ), AUX7_MASTER },
            {new Tuple<byte, byte, byte>( 127, 127, PRESET_AUX_8 ), AUX8_MASTER },
        };

        private static Dictionary<Tuple<string, byte>, byte[]> PRESET_ROTARY_CC = new Dictionary<Tuple<string, byte>, byte[]>()
        {
            { new Tuple<string, byte>(AUX1_1_8, 16), new byte[] { AUX_1_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX1_1_8, 17), new byte[] { AUX_1_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX1_1_8, 18), new byte[] { AUX_1_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX1_1_8, 19), new byte[] { AUX_1_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX1_1_8, 20), new byte[] { AUX_1_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX1_1_8, 21), new byte[] { AUX_1_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX1_1_8, 22), new byte[] { AUX_1_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX1_1_8, 23), new byte[] { AUX_1_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX1_9_16, 16), new byte[] { AUX_1_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX1_9_16, 17), new byte[] { AUX_1_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX1_9_16, 18), new byte[] { AUX_1_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX1_9_16, 19), new byte[] { AUX_1_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX1_9_16, 20), new byte[] { AUX_1_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX1_9_16, 21), new byte[] { AUX_1_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX1_9_16, 22), new byte[] { AUX_1_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX1_9_16, 23), new byte[] { AUX_1_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX1_MASTER, 16), new byte[] { AUX_1_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX1_MASTER, 17), new byte[] { AUX_1_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX1_MASTER, 18), new byte[] { AUX_1_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX1_MASTER, 19), new byte[] { AUX_1_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX1_MASTER, 20), new byte[] { AUX_1_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX1_MASTER, 21), new byte[] { AUX_1_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX1_MASTER, 22), new byte[] { AUX_1_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX1_MASTER, 23), new byte[] { AUX_1_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX2_1_8, 16), new byte[] { AUX_2_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX2_1_8, 17), new byte[] { AUX_2_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX2_1_8, 18), new byte[] { AUX_2_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX2_1_8, 19), new byte[] { AUX_2_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX2_1_8, 20), new byte[] { AUX_2_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX2_1_8, 21), new byte[] { AUX_2_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX2_1_8, 22), new byte[] { AUX_2_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX2_1_8, 23), new byte[] { AUX_2_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX2_9_16, 16), new byte[] { AUX_2_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX2_9_16, 17), new byte[] { AUX_2_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX2_9_16, 18), new byte[] { AUX_2_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX2_9_16, 19), new byte[] { AUX_2_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX2_9_16, 20), new byte[] { AUX_2_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX2_9_16, 21), new byte[] { AUX_2_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX2_9_16, 22), new byte[] { AUX_2_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX2_9_16, 23), new byte[] { AUX_2_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX2_MASTER, 16), new byte[] { AUX_2_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX2_MASTER, 17), new byte[] { AUX_2_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX2_MASTER, 18), new byte[] { AUX_2_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX2_MASTER, 19), new byte[] { AUX_2_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX2_MASTER, 20), new byte[] { AUX_2_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX2_MASTER, 21), new byte[] { AUX_2_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX2_MASTER, 22), new byte[] { AUX_2_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX2_MASTER, 23), new byte[] { AUX_2_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX3_1_8, 16), new byte[] { AUX_3_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX3_1_8, 17), new byte[] { AUX_3_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX3_1_8, 18), new byte[] { AUX_3_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX3_1_8, 19), new byte[] { AUX_3_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX3_1_8, 20), new byte[] { AUX_3_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX3_1_8, 21), new byte[] { AUX_3_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX3_1_8, 22), new byte[] { AUX_3_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX3_1_8, 23), new byte[] { AUX_3_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX3_9_16, 16), new byte[] { AUX_3_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX3_9_16, 17), new byte[] { AUX_3_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX3_9_16, 18), new byte[] { AUX_3_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX3_9_16, 19), new byte[] { AUX_3_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX3_9_16, 20), new byte[] { AUX_3_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX3_9_16, 21), new byte[] { AUX_3_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX3_9_16, 22), new byte[] { AUX_3_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX3_9_16, 23), new byte[] { AUX_3_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX3_MASTER, 16), new byte[] { AUX_3_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX3_MASTER, 17), new byte[] { AUX_3_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX3_MASTER, 18), new byte[] { AUX_3_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX3_MASTER, 19), new byte[] { AUX_3_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX3_MASTER, 20), new byte[] { AUX_3_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX3_MASTER, 21), new byte[] { AUX_3_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX3_MASTER, 22), new byte[] { AUX_3_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX3_MASTER, 23), new byte[] { AUX_3_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX4_1_8, 16), new byte[] { AUX_4_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX4_1_8, 17), new byte[] { AUX_4_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX4_1_8, 18), new byte[] { AUX_4_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX4_1_8, 19), new byte[] { AUX_4_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX4_1_8, 20), new byte[] { AUX_4_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX4_1_8, 21), new byte[] { AUX_4_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX4_1_8, 22), new byte[] { AUX_4_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX4_1_8, 23), new byte[] { AUX_4_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX4_9_16, 16), new byte[] { AUX_4_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX4_9_16, 17), new byte[] { AUX_4_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX4_9_16, 18), new byte[] { AUX_4_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX4_9_16, 19), new byte[] { AUX_4_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX4_9_16, 20), new byte[] { AUX_4_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX4_9_16, 21), new byte[] { AUX_4_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX4_9_16, 22), new byte[] { AUX_4_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX4_9_16, 23), new byte[] { AUX_4_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX4_MASTER, 16), new byte[] { AUX_4_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX4_MASTER, 17), new byte[] { AUX_4_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX4_MASTER, 18), new byte[] { AUX_4_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX4_MASTER, 19), new byte[] { AUX_4_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX4_MASTER, 20), new byte[] { AUX_4_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX4_MASTER, 21), new byte[] { AUX_4_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX4_MASTER, 22), new byte[] { AUX_4_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX4_MASTER, 23), new byte[] { AUX_4_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX5_1_8, 16), new byte[] { AUX_5_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX5_1_8, 17), new byte[] { AUX_5_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX5_1_8, 18), new byte[] { AUX_5_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX5_1_8, 19), new byte[] { AUX_5_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX5_1_8, 20), new byte[] { AUX_5_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX5_1_8, 21), new byte[] { AUX_5_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX5_1_8, 22), new byte[] { AUX_5_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX5_1_8, 23), new byte[] { AUX_5_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX5_9_16, 16), new byte[] { AUX_5_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX5_9_16, 17), new byte[] { AUX_5_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX5_9_16, 18), new byte[] { AUX_5_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX5_9_16, 19), new byte[] { AUX_5_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX5_9_16, 20), new byte[] { AUX_5_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX5_9_16, 21), new byte[] { AUX_5_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX5_9_16, 22), new byte[] { AUX_5_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX5_9_16, 23), new byte[] { AUX_5_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX5_MASTER, 16), new byte[] { AUX_5_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX5_MASTER, 17), new byte[] { AUX_5_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX5_MASTER, 18), new byte[] { AUX_5_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX5_MASTER, 19), new byte[] { AUX_5_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX5_MASTER, 20), new byte[] { AUX_5_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX5_MASTER, 21), new byte[] { AUX_5_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX5_MASTER, 22), new byte[] { AUX_5_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX5_MASTER, 23), new byte[] { AUX_5_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX6_1_8, 16), new byte[] { AUX_6_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX6_1_8, 17), new byte[] { AUX_6_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX6_1_8, 18), new byte[] { AUX_6_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX6_1_8, 19), new byte[] { AUX_6_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX6_1_8, 20), new byte[] { AUX_6_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX6_1_8, 21), new byte[] { AUX_6_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX6_1_8, 22), new byte[] { AUX_6_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX6_1_8, 23), new byte[] { AUX_6_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX6_9_16, 16), new byte[] { AUX_6_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX6_9_16, 17), new byte[] { AUX_6_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX6_9_16, 18), new byte[] { AUX_6_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX6_9_16, 19), new byte[] { AUX_6_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX6_9_16, 20), new byte[] { AUX_6_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX6_9_16, 21), new byte[] { AUX_6_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX6_9_16, 22), new byte[] { AUX_6_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX6_9_16, 23), new byte[] { AUX_6_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX6_MASTER, 16), new byte[] { AUX_6_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX6_MASTER, 17), new byte[] { AUX_6_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX6_MASTER, 18), new byte[] { AUX_6_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX6_MASTER, 19), new byte[] { AUX_6_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX6_MASTER, 20), new byte[] { AUX_6_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX6_MASTER, 21), new byte[] { AUX_6_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX6_MASTER, 22), new byte[] { AUX_6_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX6_MASTER, 23), new byte[] { AUX_6_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX7_1_8, 16), new byte[] { AUX_7_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX7_1_8, 17), new byte[] { AUX_7_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX7_1_8, 18), new byte[] { AUX_7_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX7_1_8, 19), new byte[] { AUX_7_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX7_1_8, 20), new byte[] { AUX_7_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX7_1_8, 21), new byte[] { AUX_7_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX7_1_8, 22), new byte[] { AUX_7_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX7_1_8, 23), new byte[] { AUX_7_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX7_9_16, 16), new byte[] { AUX_7_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX7_9_16, 17), new byte[] { AUX_7_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX7_9_16, 18), new byte[] { AUX_7_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX7_9_16, 19), new byte[] { AUX_7_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX7_9_16, 20), new byte[] { AUX_7_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX7_9_16, 21), new byte[] { AUX_7_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX7_9_16, 22), new byte[] { AUX_7_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX7_9_16, 23), new byte[] { AUX_7_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX7_MASTER, 16), new byte[] { AUX_7_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX7_MASTER, 17), new byte[] { AUX_7_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX7_MASTER, 18), new byte[] { AUX_7_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX7_MASTER, 19), new byte[] { AUX_7_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX7_MASTER, 20), new byte[] { AUX_7_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX7_MASTER, 21), new byte[] { AUX_7_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX7_MASTER, 22), new byte[] { AUX_7_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX7_MASTER, 23), new byte[] { AUX_7_CHANNEL, 74, RING_LED8 } },

            { new Tuple<string, byte>(AUX8_1_8, 16), new byte[] { AUX_8_CHANNEL, 1, RING_LED1 } },
            { new Tuple<string, byte>(AUX8_1_8, 17), new byte[] { AUX_8_CHANNEL, 2, RING_LED2 } },
            { new Tuple<string, byte>(AUX8_1_8, 18), new byte[] { AUX_8_CHANNEL, 3, RING_LED3 } },
            { new Tuple<string, byte>(AUX8_1_8, 19), new byte[] { AUX_8_CHANNEL, 4, RING_LED4 } },
            { new Tuple<string, byte>(AUX8_1_8, 20), new byte[] { AUX_8_CHANNEL, 5, RING_LED5 } },
            { new Tuple<string, byte>(AUX8_1_8, 21), new byte[] { AUX_8_CHANNEL, 6, RING_LED6 } },
            { new Tuple<string, byte>(AUX8_1_8, 22), new byte[] { AUX_8_CHANNEL, 7, RING_LED7 } },
            { new Tuple<string, byte>(AUX8_1_8, 23), new byte[] { AUX_8_CHANNEL, 8, RING_LED8 } },
            { new Tuple<string, byte>(AUX8_9_16, 16), new byte[] { AUX_8_CHANNEL, 11, RING_LED1 } },
            { new Tuple<string, byte>(AUX8_9_16, 17), new byte[] { AUX_8_CHANNEL, 12, RING_LED2 } },
            { new Tuple<string, byte>(AUX8_9_16, 18), new byte[] { AUX_8_CHANNEL, 13, RING_LED3 } },
            { new Tuple<string, byte>(AUX8_9_16, 19), new byte[] { AUX_8_CHANNEL, 14, RING_LED4 } },
            { new Tuple<string, byte>(AUX8_9_16, 20), new byte[] { AUX_8_CHANNEL, 15, RING_LED5 } },
            { new Tuple<string, byte>(AUX8_9_16, 21), new byte[] { AUX_8_CHANNEL, 16, RING_LED6 } },
            { new Tuple<string, byte>(AUX8_9_16, 22), new byte[] { AUX_8_CHANNEL, 17, RING_LED7 } },
            { new Tuple<string, byte>(AUX8_9_16, 23), new byte[] { AUX_8_CHANNEL, 18, RING_LED8 } },
            { new Tuple<string, byte>(AUX8_MASTER, 16), new byte[] { AUX_8_CHANNEL, 67, RING_LED1 } },
            { new Tuple<string, byte>(AUX8_MASTER, 17), new byte[] { AUX_8_CHANNEL, 68, RING_LED2 } },
            { new Tuple<string, byte>(AUX8_MASTER, 18), new byte[] { AUX_8_CHANNEL, 69, RING_LED3 } },
            { new Tuple<string, byte>(AUX8_MASTER, 19), new byte[] { AUX_8_CHANNEL, 70, RING_LED4 } },
            { new Tuple<string, byte>(AUX8_MASTER, 20), new byte[] { AUX_8_CHANNEL, 71, RING_LED5 } },
            { new Tuple<string, byte>(AUX8_MASTER, 21), new byte[] { AUX_8_CHANNEL, 72, RING_LED6 } },
            { new Tuple<string, byte>(AUX8_MASTER, 22), new byte[] { AUX_8_CHANNEL, 73, RING_LED7 } },
            { new Tuple<string, byte>(AUX8_MASTER, 23), new byte[] { AUX_8_CHANNEL, 74, RING_LED8 } },
        };
        private static Dictionary<Tuple<string, byte>, byte[]> PRESET_BUTTON_CC = new Dictionary<Tuple<string, byte>, byte[]>()
        {
            { new Tuple<string, byte>(AUX1_1_8, BUTTON1), new byte[] {1, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON2), new byte[] {1, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON3), new byte[] {1, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON4), new byte[] {1, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON5), new byte[] {1, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON6), new byte[] {1, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON7), new byte[] {1, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON8), new byte[] {1, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON9), new byte[] {1, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON10), new byte[] {1, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON11), new byte[] {1, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON12), new byte[] {1, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON13), new byte[] {1, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON14), new byte[] {1, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON15), new byte[] {1, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX1_1_8, BUTTON16), new byte[] {1, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON1), new byte[] {1, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON2), new byte[] {1, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON3), new byte[] {1, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON4), new byte[] {1, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON5), new byte[] {1, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON6), new byte[] {1, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON7), new byte[] {1, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON8), new byte[] {1, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON9), new byte[] {1, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON10), new byte[] {1, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON11), new byte[] {1, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON12), new byte[] {1, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON13), new byte[] {1, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON14), new byte[] {1, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON15), new byte[] {1, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX1_9_16, BUTTON16), new byte[] {1, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON1), new byte[] {1, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON2), new byte[] {1, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON3), new byte[] {1, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON4), new byte[] {1, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON5), new byte[] {1, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON6), new byte[] {1, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON7), new byte[] {1, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON8), new byte[] {1, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON9), new byte[] {1, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON10), new byte[] {1, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON11), new byte[] {1, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON12), new byte[] {1, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON13), new byte[] {1, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON14), new byte[] {1, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON15), new byte[] {1, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX1_MASTER, BUTTON16), new byte[] {1, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX2_1_8, BUTTON1), new byte[] {2, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON2), new byte[] {2, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON3), new byte[] {2, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON4), new byte[] {2, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON5), new byte[] {2, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON6), new byte[] {2, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON7), new byte[] {2, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON8), new byte[] {2, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON9), new byte[] {2, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON10), new byte[] {2, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON11), new byte[] {2, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON12), new byte[] {2, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON13), new byte[] {2, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON14), new byte[] {2, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON15), new byte[] {2, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX2_1_8, BUTTON16), new byte[] {2, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON1), new byte[] {2, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON2), new byte[] {2, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON3), new byte[] {2, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON4), new byte[] {2, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON5), new byte[] {2, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON6), new byte[] {2, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON7), new byte[] {2, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON8), new byte[] {2, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON9), new byte[] {2, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON10), new byte[] {2, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON11), new byte[] {2, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON12), new byte[] {2, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON13), new byte[] {2, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON14), new byte[] {2, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON15), new byte[] {2, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX2_9_16, BUTTON16), new byte[] {2, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON1), new byte[] {2, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON2), new byte[] {2, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON3), new byte[] {2, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON4), new byte[] {2, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON5), new byte[] {2, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON6), new byte[] {2, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON7), new byte[] {2, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON8), new byte[] {2, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON9), new byte[] {2, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON10), new byte[] {2, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON11), new byte[] {2, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON12), new byte[] {2, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON13), new byte[] {2, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON14), new byte[] {2, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON15), new byte[] {2, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX2_MASTER, BUTTON16), new byte[] {2, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX3_1_8, BUTTON1), new byte[] {3, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON2), new byte[] {3, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON3), new byte[] {3, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON4), new byte[] {3, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON5), new byte[] {3, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON6), new byte[] {3, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON7), new byte[] {3, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON8), new byte[] {3, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON9), new byte[] {3, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON10), new byte[] {3, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON11), new byte[] {3, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON12), new byte[] {3, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON13), new byte[] {3, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON14), new byte[] {3, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON15), new byte[] {3, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX3_1_8, BUTTON16), new byte[] {3, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON1), new byte[] {3, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON2), new byte[] {3, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON3), new byte[] {3, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON4), new byte[] {3, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON5), new byte[] {3, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON6), new byte[] {3, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON7), new byte[] {3, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON8), new byte[] {3, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON9), new byte[] {3, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON10), new byte[] {3, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON11), new byte[] {3, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON12), new byte[] {3, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON13), new byte[] {3, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON14), new byte[] {3, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON15), new byte[] {3, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX3_9_16, BUTTON16), new byte[] {3, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON1), new byte[] {3, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON2), new byte[] {3, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON3), new byte[] {3, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON4), new byte[] {3, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON5), new byte[] {3, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON6), new byte[] {3, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON7), new byte[] {3, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON8), new byte[] {3, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON9), new byte[] {3, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON10), new byte[] {3, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON11), new byte[] {3, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON12), new byte[] {3, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON13), new byte[] {3, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON14), new byte[] {3, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON15), new byte[] {3, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX3_MASTER, BUTTON16), new byte[] {3, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX4_1_8, BUTTON1), new byte[] {4, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON2), new byte[] {4, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON3), new byte[] {4, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON4), new byte[] {4, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON5), new byte[] {4, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON6), new byte[] {4, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON7), new byte[] {4, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON8), new byte[] {4, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON9), new byte[] {4, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON10), new byte[] {4, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON11), new byte[] {4, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON12), new byte[] {4, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON13), new byte[] {4, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON14), new byte[] {4, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON15), new byte[] {4, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX4_1_8, BUTTON16), new byte[] {4, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON1), new byte[] {4, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON2), new byte[] {4, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON3), new byte[] {4, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON4), new byte[] {4, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON5), new byte[] {4, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON6), new byte[] {4, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON7), new byte[] {4, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON8), new byte[] {4, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON9), new byte[] {4, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON10), new byte[] {4, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON11), new byte[] {4, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON12), new byte[] {4, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON13), new byte[] {4, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON14), new byte[] {4, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON15), new byte[] {4, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX4_9_16, BUTTON16), new byte[] {4, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON1), new byte[] {4, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON2), new byte[] {4, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON3), new byte[] {4, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON4), new byte[] {4, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON5), new byte[] {4, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON6), new byte[] {4, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON7), new byte[] {4, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON8), new byte[] {4, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON9), new byte[] {4, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON10), new byte[] {4, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON11), new byte[] {4, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON12), new byte[] {4, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON13), new byte[] {4, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON14), new byte[] {4, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON15), new byte[] {4, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX4_MASTER, BUTTON16), new byte[] {4, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX5_1_8, BUTTON1), new byte[] {5, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON2), new byte[] {5, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON3), new byte[] {5, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON4), new byte[] {5, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON5), new byte[] {5, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON6), new byte[] {5, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON7), new byte[] {5, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON8), new byte[] {5, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON9), new byte[] {5, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON10), new byte[] {5, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON11), new byte[] {5, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON12), new byte[] {5, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON13), new byte[] {5, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON14), new byte[] {5, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON15), new byte[] {5, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX5_1_8, BUTTON16), new byte[] {5, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON1), new byte[] {5, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON2), new byte[] {5, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON3), new byte[] {5, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON4), new byte[] {5, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON5), new byte[] {5, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON6), new byte[] {5, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON7), new byte[] {5, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON8), new byte[] {5, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON9), new byte[] {5, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON10), new byte[] {5, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON11), new byte[] {5, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON12), new byte[] {5, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON13), new byte[] {5, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON14), new byte[] {5, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON15), new byte[] {5, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX5_9_16, BUTTON16), new byte[] {5, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON1), new byte[] {5, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON2), new byte[] {5, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON3), new byte[] {5, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON4), new byte[] {5, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON5), new byte[] {5, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON6), new byte[] {5, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON7), new byte[] {5, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON8), new byte[] {5, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON9), new byte[] {5, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON10), new byte[] {5, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON11), new byte[] {5, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON12), new byte[] {5, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON13), new byte[] {5, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON14), new byte[] {5, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON15), new byte[] {5, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX5_MASTER, BUTTON16), new byte[] {5, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX6_1_8, BUTTON1), new byte[] {6, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON2), new byte[] {6, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON3), new byte[] {6, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON4), new byte[] {6, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON5), new byte[] {6, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON6), new byte[] {6, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON7), new byte[] {6, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON8), new byte[] {6, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON9), new byte[] {6, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON10), new byte[] {6, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON11), new byte[] {6, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON12), new byte[] {6, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON13), new byte[] {6, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON14), new byte[] {6, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON15), new byte[] {6, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX6_1_8, BUTTON16), new byte[] {6, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON1), new byte[] {6, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON2), new byte[] {6, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON3), new byte[] {6, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON4), new byte[] {6, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON5), new byte[] {6, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON6), new byte[] {6, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON7), new byte[] {6, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON8), new byte[] {6, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON9), new byte[] {6, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON10), new byte[] {6, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON11), new byte[] {6, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON12), new byte[] {6, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON13), new byte[] {6, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON14), new byte[] {6, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON15), new byte[] {6, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX6_9_16, BUTTON16), new byte[] {6, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON1), new byte[] {6, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON2), new byte[] {6, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON3), new byte[] {6, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON4), new byte[] {6, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON5), new byte[] {6, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON6), new byte[] {6, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON7), new byte[] {6, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON8), new byte[] {6, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON9), new byte[] {6, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON10), new byte[] {6, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON11), new byte[] {6, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON12), new byte[] {6, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON13), new byte[] {6, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON14), new byte[] {6, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON15), new byte[] {6, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX6_MASTER, BUTTON16), new byte[] {6, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX7_1_8, BUTTON1), new byte[] {7, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON2), new byte[] {7, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON3), new byte[] {7, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON4), new byte[] {7, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON5), new byte[] {7, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON6), new byte[] {7, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON7), new byte[] {7, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON8), new byte[] {7, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON9), new byte[] {7, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON10), new byte[] {7, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON11), new byte[] {7, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON12), new byte[] {7, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON13), new byte[] {7, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON14), new byte[] {7, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON15), new byte[] {7, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX7_1_8, BUTTON16), new byte[] {7, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON1), new byte[] {7, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON2), new byte[] {7, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON3), new byte[] {7, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON4), new byte[] {7, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON5), new byte[] {7, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON6), new byte[] {7, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON7), new byte[] {7, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON8), new byte[] {7, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON9), new byte[] {7, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON10), new byte[] {7, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON11), new byte[] {7, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON12), new byte[] {7, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON13), new byte[] {7, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON14), new byte[] {7, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON15), new byte[] {7, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX7_9_16, BUTTON16), new byte[] {7, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON1), new byte[] {7, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON2), new byte[] {7, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON3), new byte[] {7, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON4), new byte[] {7, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON5), new byte[] {7, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON6), new byte[] {7, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON7), new byte[] {7, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON8), new byte[] {7, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON9), new byte[] {7, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON10), new byte[] {7, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON11), new byte[] {7, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON12), new byte[] {7, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON13), new byte[] {7, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON14), new byte[] {7, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON15), new byte[] {7, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX7_MASTER, BUTTON16), new byte[] {7, 90, BUTTON16 } },

            { new Tuple<string, byte>(AUX8_1_8, BUTTON1), new byte[] {8, 19, BUTTON1 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON2), new byte[] {8, 20, BUTTON2 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON3), new byte[] {8, 21, BUTTON3 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON4), new byte[] {8, 22, BUTTON4 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON5), new byte[] {8, 23, BUTTON5 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON6), new byte[] {8, 24, BUTTON6 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON7), new byte[] {8, 25, BUTTON7 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON8), new byte[] {8, 26, BUTTON8 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON9), new byte[] {8, 27, BUTTON9 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON10), new byte[] {8, 28, BUTTON10 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON11), new byte[] {8, 29, BUTTON11 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON12), new byte[] {8, 30, BUTTON12 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON13), new byte[] {8, 31, BUTTON13 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON14), new byte[] {8, 32, BUTTON14 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON15), new byte[] {8, 33, BUTTON15 } },
            { new Tuple<string, byte>(AUX8_1_8, BUTTON16), new byte[] {8, 34, BUTTON16 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON1), new byte[] {8, 43, BUTTON1 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON2), new byte[] {8, 44, BUTTON2 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON3), new byte[] {8, 45, BUTTON3 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON4), new byte[] {8, 46, BUTTON4 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON5), new byte[] {8, 47, BUTTON5 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON6), new byte[] {8, 48, BUTTON6 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON7), new byte[] {8, 49, BUTTON7 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON8), new byte[] {8, 50, BUTTON8 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON9), new byte[] {8, 51, BUTTON9 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON10), new byte[] {8, 52, BUTTON10 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON11), new byte[] {8, 53, BUTTON11 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON12), new byte[] {8, 54, BUTTON12 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON13), new byte[] {8, 55, BUTTON13 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON14), new byte[] {8, 56, BUTTON14 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON15), new byte[] {8, 57, BUTTON15 } },
            { new Tuple<string, byte>(AUX8_9_16, BUTTON16), new byte[] {8, 58, BUTTON16 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON1), new byte[] {8, 75, BUTTON1 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON2), new byte[] {8, 76, BUTTON2 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON3), new byte[] {8, 77, BUTTON3 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON4), new byte[] {8, 78, BUTTON4 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON5), new byte[] {8, 79, BUTTON5 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON6), new byte[] {8, 80, BUTTON6 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON7), new byte[] {8, 81, BUTTON7 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON8), new byte[] {8, 82, BUTTON8 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON9), new byte[] {8, 83, BUTTON9 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON10), new byte[] {8, 84, BUTTON10 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON11), new byte[] {8, 85, BUTTON11 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON12), new byte[] {8, 86, BUTTON12 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON13), new byte[] {8, 87, BUTTON13 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON14), new byte[] {8, 88, BUTTON14 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON15), new byte[] {8, 89, BUTTON15 } },
            { new Tuple<string, byte>(AUX8_MASTER, BUTTON16), new byte[] {8, 90, BUTTON16 } },

        };
        private Dictionary<string, byte[]> PRESET_FADER_CC = new Dictionary<string, byte[]>()
        {
            { AUX1_1_8,     new byte[] { AUX_1_CHANNEL, 9 } },
            { AUX1_9_16,    new byte[] { AUX_1_CHANNEL, 10 } },
            { AUX1_MASTER,  new byte[] { AUX_1_CHANNEL, 99 } },
            { AUX2_1_8,     new byte[] { AUX_2_CHANNEL, 9 } },
            { AUX2_9_16,    new byte[] { AUX_2_CHANNEL, 10 } },
            { AUX2_MASTER,  new byte[] { AUX_2_CHANNEL, 99 } },
            { AUX3_1_8,     new byte[] { AUX_3_CHANNEL, 9 } },
            { AUX3_9_16,    new byte[] { AUX_3_CHANNEL, 10 } },
            { AUX3_MASTER,  new byte[] { AUX_3_CHANNEL, 99 } },
            { AUX4_1_8,     new byte[] { AUX_4_CHANNEL, 9 } },
            { AUX4_9_16,    new byte[] { AUX_4_CHANNEL, 10 } },
            { AUX4_MASTER,  new byte[] { AUX_4_CHANNEL, 99 } },
            { AUX5_1_8,     new byte[] { AUX_5_CHANNEL, 9 } },
            { AUX5_9_16,    new byte[] { AUX_5_CHANNEL, 10 } },
            { AUX5_MASTER,  new byte[] { AUX_5_CHANNEL, 99 } },
            { AUX6_1_8,     new byte[] { AUX_6_CHANNEL, 9 } },
            { AUX6_9_16,    new byte[] { AUX_6_CHANNEL, 10 } },
            { AUX6_MASTER,  new byte[] { AUX_6_CHANNEL, 99 } },
            { AUX7_1_8,     new byte[] { AUX_7_CHANNEL, 9 } },
            { AUX7_9_16,    new byte[] { AUX_7_CHANNEL, 10 } },
            { AUX7_MASTER,  new byte[] { AUX_7_CHANNEL, 99 } },
            { AUX8_1_8,     new byte[] { AUX_8_CHANNEL, 9 } },
            { AUX8_9_16,    new byte[] { AUX_8_CHANNEL, 10 } },
            { AUX8_MASTER,  new byte[] { AUX_8_CHANNEL, 99 } },
        };

        Dictionary<string, string> presetSelected = new Dictionary<string, string>();
        Dictionary<string, Dictionary<byte, byte>> presetState = new Dictionary<string, Dictionary<byte, byte>>();
        Dictionary<string, Dictionary<byte, byte>> layerValue = new Dictionary<string, Dictionary<byte, byte>>();
        Dictionary<string, byte> device1_aux = new Dictionary<string, byte>();

        Dictionary<Tuple<byte, byte>, byte> midiVariable = new Dictionary<Tuple<byte, byte>, byte>();
        Dictionary<string, ushort> midiVariableFader = new Dictionary<string, ushort>()
        {
            { AUX1_1_8,     0},
            { AUX1_9_16,    0 },
            { AUX1_MASTER,  0 },
            { AUX2_1_8,     0},
            { AUX2_9_16,    0 },
            { AUX2_MASTER,  0 },
            { AUX3_1_8,     0},
            { AUX3_9_16,    0 },
            { AUX3_MASTER,  0 },
            { AUX4_1_8,     0},
            { AUX4_9_16,    0 },
            { AUX4_MASTER,  0 },
            { AUX5_1_8,     0},
            { AUX5_9_16,    0 },
            { AUX5_MASTER,  0 },
            { AUX6_1_8,     0},
            { AUX6_9_16,    0 },
            { AUX6_MASTER,  0 },
            { AUX7_1_8,     0},
            { AUX7_9_16,    0 },
            { AUX7_MASTER,  0 },
            { AUX8_1_8,     0},
            { AUX8_9_16,    0 },
            { AUX8_MASTER,  0 },
        };

        private Boolean enumerationCompleted = false;

        public Principal()
        {
            InitializeComponent();

            loadConfig();

            deviceWatcher = DeviceInformation.CreateWatcher(MidiInPort.GetDeviceSelector());
            deviceWatcher.Added += DeviceWatcher_UpdateCompleted;
            deviceWatcher.Removed += DeviceWatcher_UpdateCompleted;
            deviceWatcher.Updated += DeviceWatcher_UpdateCompleted;
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
        private System.Windows.Forms.CheckBox displayDevice(string idDevice, string deviceName)
        {
            System.Windows.Forms.CheckBox checkboxDevice = new System.Windows.Forms.CheckBox();
            checkboxDevice.AutoSize=true;
            checkboxDevice.Location=new Point(59, 10);
            checkboxDevice.Name=idDevice;
            checkboxDevice.Size=new Size(159, 36);
            checkboxDevice.TabIndex=2;
            checkboxDevice.Text=deviceName;
            checkboxDevice.UseVisualStyleBackColor=true;

            System.Windows.Forms.ComboBox comboDevice = new System.Windows.Forms.ComboBox();
            comboDevice.FormattingEnabled=true;
            comboDevice.Location=new Point(481, 10);
            comboDevice.Name="comboBox"+idDevice;
            comboDevice.Size=new Size(242, 40);
            comboDevice.TabIndex=3;
            comboDevice.Items.Add(AUX1_1_8);
            comboDevice.Items.Add(AUX2_1_8);
            comboDevice.Items.Add(AUX3_1_8);
            comboDevice.Items.Add(AUX4_1_8);
            comboDevice.Items.Add(AUX5_1_8);
            comboDevice.Items.Add(AUX6_1_8);
            comboDevice.Items.Add(AUX7_1_8);
            comboDevice.Items.Add(AUX8_1_8);

            System.Windows.Forms.Panel checkboxPanel = new System.Windows.Forms.Panel();
            checkboxPanel.Controls.Add(comboDevice);
            checkboxPanel.Controls.Add(checkboxDevice);
            checkboxPanel.Dock=DockStyle.Top;
            checkboxPanel.Location=new Point(0, 0);
            checkboxPanel.Name="panel3";
            checkboxPanel.Size=new Size(998, 56);
            checkboxPanel.TabIndex=6;

            panel4.Controls.Add(checkboxPanel);
            return checkboxDevice;
        }
        private string extractId(string deviceID)
        {
            return deviceID.Split("#")[2].Split(".")[0];
        }
        private async void EnumerateMidiInputDevices()
        {
            // Find all input MIDI devices
            midiInputDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());
            if (midiInputDevices.Count > 0)
            {
                foreach (DeviceInformation deviceInfo in midiInputDevices)
                {
                    string idDevice = extractId(deviceInfo.Id);
                    if (!midiInDevicePort.ContainsKey(idDevice) && DEVICE_ID.Contains(idDevice) &&
                        !LOOP_IN_DEVICE_ID.Equals(idDevice) && !LOOP_OUT_DEVICE_ID.Equals(idDevice))
                    {
                        midiInDevicePort.TryAdd(idDevice, await MidiInPort.FromIdAsync(deviceInfo.Id));
                        midiInDevicePort[idDevice].MessageReceived += MidiInDevicePort_MessageReceived;
                    }
                    if (midiInLoopPort == null && LOOP_IN_DEVICE_ID.Equals(idDevice))
                    {
                        midiInLoopPort = await MidiInPort.FromIdAsync(deviceInfo.Id);
                        midiInLoopPort.MessageReceived += MidiInLoopPort_MessageReceived;
                    }
                }
                List<string> deviceList = midiInDevicePort.Keys.ToList();
                foreach (string midiIn in deviceList)
                {
                    Boolean notFound = true;
                    foreach (DeviceInformation deviceInfo in midiInputDevices)
                    {
                        string idDevice = extractId(deviceInfo.Id);
                        if (idDevice == midiIn)
                        {
                            notFound = false;
                        }
                    }
                    if (notFound)
                    {
                        midiInDevicePort[midiIn].Dispose();
                        midiInDevicePort.TryRemove(new KeyValuePair<string, MidiInPort>(midiIn, midiInDevicePort[midiIn]));
                    }
                }
            }

            _=this.panel4.Invoke(new MethodInvoker(delegate
            {
                this.panel4.Controls.Clear();
                this.comboBoxEntreeLoop.Items.Clear();

                // Return if no external devices are connected
                if (midiInputDevices.Count == 0)
                {
                    displayDevice("", "No MIDI input devices found!");
                    this.comboBoxEntreeLoop.Items.Add("No MIDI input devices found!");
                    return;
                }

                // Else, add each connected input device to the list
                foreach (DeviceInformation deviceInfo in midiInputDevices)
                {
                    System.Diagnostics.Debug.WriteLine("MidiInput {0} , {1}", deviceInfo.Id, deviceInfo.Name);
                    this.comboBoxEntreeLoop.Items.Add(deviceInfo.Name);
                    string idDevice = extractId(deviceInfo.Id);
                    if (!LOOP_IN_DEVICE_ID.Equals(idDevice) && !LOOP_OUT_DEVICE_ID.Equals(idDevice))
                    {
                        System.Windows.Forms.CheckBox check = displayDevice(idDevice, deviceInfo.Name);
                        check.CheckedChanged += midiInDeviceChanged;
                        if (midiInDevicePort.ContainsKey(idDevice))
                        {
                            check.Checked = true;
                            ((ComboBox)check.Parent.Controls[0]).SelectedItem = DEVICE_AUX[DEVICE_ID.IndexOf(idDevice)];
                        }
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

            string midiDeviceId = extractId(id);

            foreach (DeviceInformation deviceInfoInput in midiInputDevices)
            {
                string midiInputDeviceId = extractId(deviceInfoInput.Id);
                if (midiInputDeviceId == midiDeviceId)
                {
                    name = deviceInfoInput.Name;
                    break;
                }
            }
            return name;
        }
        private void initDevice(string idDevice, string preset)
        {
            if (!layerValue.ContainsKey(idDevice))
            {
                Tuple<byte, byte, byte> presetLoaded = null;
                foreach(Tuple<byte, byte, byte> auxPreset in PRESET_LIST.Keys)
                {
                    if(preset == PRESET_LIST[auxPreset])
                    {
                        presetLoaded = auxPreset;
                    }
                }
                if(presetLoaded != null)
                {
                    device1_aux.Add(idDevice, presetLoaded.Item3);
                    layerValue.Add(idDevice, new Dictionary<byte, byte>()
                            {
                                { PRESET_A_NOTE, presetLoaded.Item1 },
                                { PRESET_B_NOTE, presetLoaded.Item2 },
                            });
                }
                else
                {
                    device1_aux.Add(idDevice, PRESET_AUX_1);
                    layerValue.Add(idDevice, new Dictionary<byte, byte>()
                            {
                                { PRESET_A_NOTE, 127 },
                                { PRESET_B_NOTE, 0 },
                            });
                }
                presetSelected.Add(idDevice, PRESET_LIST[new Tuple<byte, byte, byte>(layerValue[idDevice][PRESET_A_NOTE], layerValue[idDevice][PRESET_B_NOTE], this.device1_aux[idDevice])]);
                presetState.Add(idDevice, new Dictionary<byte, byte>()
                            {
                                { PRESET_AUX_1, 0 },
                                { PRESET_AUX_2, 0 },
                                { PRESET_AUX_3, 0 },
                                { PRESET_AUX_4, 0 },
                                { PRESET_AUX_5, 0 },
                                { PRESET_AUX_6, 0 },
                                { PRESET_AUX_7, 0 },
                                { PRESET_AUX_8, 0 }
                            });
            }

            midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_A_LED, layerValue[idDevice][PRESET_A_NOTE]));
            midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_B_LED, layerValue[idDevice][PRESET_B_NOTE]));
            changePresetState(idDevice, device1_aux[idDevice], 127);
            refreshButtonsLed(idDevice);
            foreach (Tuple<string, byte> ccin in PRESET_ROTARY_CC.Keys)
            {
                if (ccin.Item1 == this.presetSelected[idDevice])
                {
                    led(idDevice, PRESET_ROTARY_CC[ccin][2], getStoreMidi(PRESET_ROTARY_CC[ccin][0], PRESET_ROTARY_CC[ccin][1]));
                }
            }
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
                    string idDevice = extractId(deviceInfo.Id);
                    if (!midiOutDevicePort.ContainsKey(idDevice) && DEVICE_ID.Contains(idDevice))
                    {
                        midiOutDevicePort.TryAdd(idDevice, await MidiOutPort.FromIdAsync(deviceInfo.Id));
                        initDevice(idDevice, DEVICE_AUX[DEVICE_ID.IndexOf(idDevice)]);
                    }
                    if (midiOutLoopPort == null && LOOP_OUT_DEVICE_ID.Equals(idDevice))
                    {
                        midiOutLoopPort = await MidiOutPort.FromIdAsync(deviceInfo.Id);
                    }
                }
                List<string> deviceList = midiOutDevicePort.Keys.ToList();
                foreach (string midiOut in deviceList)
                {
                    Boolean notFound = true;
                    foreach (DeviceInformation deviceInfo in midiOutputDevices)
                    {
                        string idDevice = extractId(deviceInfo.Id);
                        if (idDevice == midiOut)
                        {
                            notFound = false;
                        }
                    }
                    if (notFound)
                    {
                        midiOutDevicePort[midiOut].Dispose();
                        midiOutDevicePort.TryRemove(new KeyValuePair<string, IMidiOutPort>(midiOut, midiOutDevicePort[midiOut]));
                    }
                }
            }

            _=this.panel4.Invoke(new MethodInvoker(delegate
            {
                this.comboBoxSortieLoop.Items.Clear();

                // Return if no external devices are connected
                if (midiOutputDevices.Count == 0)
                {
                    this.comboBoxSortieLoop.Items.Add("No MIDI output devices found!");
                    return;
                }

                // Else, add each connected input device to the list
                foreach (DeviceInformation deviceInfo in midiOutputDevices)
                {
                    System.Diagnostics.Debug.WriteLine("MidiOutput {0} , {1}", deviceInfo.Id, deviceInfo.Name);
                    this.comboBoxSortieLoop.Items.Add(getMidiNameFromMidiInputDevices(deviceInfo));
                    string idDevice = extractId(deviceInfo.Id);

                    if (midiOutLoopPort != null && midiOutLoopPort.DeviceId == deviceInfo.Id)
                    {
                        comboBoxSortieLoop.SelectedIndex = Array.IndexOf(midiOutputDevices.ToArray(), deviceInfo);
                    }
                }
            }));
        }
        private void DeviceWatcher_UpdateCompleted(DeviceWatcher sender, object args)
        {
            if (this.enumerationCompleted)
            {
                this.EnumerateMidiInputDevices();
                this.EnumerateMidiOutputDevices();
            }
        }
        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            this.EnumerateMidiInputDevices();
            this.EnumerateMidiOutputDevices();
            this.enumerationCompleted = true;
        }
        private async void midiInDeviceChanged(object sender, EventArgs e)
        {
            if (midiInputDevices == null)
            {
                return;
            }
            System.Windows.Forms.CheckBox check = (System.Windows.Forms.CheckBox)sender;
            DeviceInformation devInfoInput = null;
            if (midiInputDevices != null)
            {
                foreach (DeviceInformation dev in midiInputDevices)
                {
                    if (extractId(dev.Id) == check.Name)
                    {
                        devInfoInput = dev;
                    }
                }
            }
            DeviceInformation devInfoOutput = null;
            if (midiOutputDevices != null)
            {
                foreach (DeviceInformation dev in midiOutputDevices)
                {
                    if (extractId(dev.Id) == check.Name)
                    {
                        devInfoOutput = dev;
                    }
                }
            }

            if (devInfoInput == null || devInfoOutput == null)
            {
                return;
            }
            string idDevice = check.Name;
            // remove event in the last device
            if (check.Checked)
            {
                if (!midiInDevicePort.ContainsKey(idDevice))
                {
                    midiInDevicePort.TryAdd(idDevice, await MidiInPort.FromIdAsync(devInfoInput.Id));

                    if (midiInDevicePort[idDevice] == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                        return;
                    }
                    midiInDevicePort[idDevice].MessageReceived += MidiInDevicePort_MessageReceived;
                }
                if (!midiOutDevicePort.ContainsKey(idDevice))
                {
                    midiOutDevicePort.TryAdd(idDevice, await MidiOutPort.FromIdAsync(devInfoOutput.Id));

                    if (midiOutDevicePort[idDevice] == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
                        return;
                    }
                    initDevice(idDevice, (string)((ComboBox)check.Parent.Controls[0]).SelectedItem);
                }
            }
            else
            {
                midiInDevicePort[idDevice].MessageReceived -= MidiInDevicePort_MessageReceived;
                midiInDevicePort.TryRemove(new KeyValuePair<string, MidiInPort>(idDevice, midiInDevicePort[idDevice]));
                midiOutDevicePort.TryRemove(new KeyValuePair<string, IMidiOutPort>(idDevice, midiOutDevicePort[idDevice]));
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
        private void led(string idDevice, byte number, byte value)
        {
            midiOutDevicePort[idDevice].SendMessage(new MidiControlChangeMessage(DEVICE_CHANNEL, number, (byte)(value*10/127+33)));// led ring 1
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
            return (midiVariable.ContainsKey(new Tuple<byte, byte>(channel, number))) ? midiVariable[new Tuple<byte, byte>(channel, number)] : (byte)0;
        }
        private void storeMidi(byte channel, byte number, byte value)
        {
            if (midiVariable.ContainsKey(new Tuple<byte, byte>(channel, number)))
            {
                midiVariable[new Tuple<byte, byte>(channel, number)] = value;
            }
            else
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
            //System.Diagnostics.Debug.WriteLine($"send {chanSend} / {ccSend}");
            midiOutLoopPort.SendMessage(new MidiControlChangeMessage(chanSend, ccSend, getStoreMidi(chanSend, ccSend)));
        }
        private void rotary(string idDevice, byte chanSend, byte ccSend, byte value, byte numLed)
        {
            rotary(chanSend, ccSend, value);
            led(idDevice, numLed, getStoreMidi(chanSend, ccSend));
        }
        private void changePresetState(string idDevice, byte number, byte value)
        {
            foreach (byte key in this.presetState[idDevice].Keys)
            {
                presetState[idDevice][key] = 0;
            }
            this.presetState[idDevice][number] = value;
            foreach (byte key in this.presetState[idDevice].Keys)
            {
                midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, key, presetState[idDevice][key]));
            }
        }
        private void refreshButtonsLed(string idDevice)
        {
            foreach (Tuple<string, byte> ccin in PRESET_BUTTON_CC.Keys)
            {
                if (ccin.Item1 == this.presetSelected[idDevice])
                {
                    midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_BUTTON_CC[ccin][2], getStoreMidi(PRESET_BUTTON_CC[ccin][0], PRESET_BUTTON_CC[ccin][1])));
                }
            }
        }
        private void readMidiMessage(MidiInPort sender, IMidiMessage receivedMidiMessage)
        {
            if (midiInDevicePort.ContainsKey(extractId(sender.DeviceId)))
            {
                string idDevice = extractId(sender.DeviceId);
                if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
                {
                    byte channel = ((MidiControlChangeMessage)receivedMidiMessage).Channel;
                    byte number = ((MidiControlChangeMessage)receivedMidiMessage).Controller;
                    byte value = ((MidiControlChangeMessage)receivedMidiMessage).ControlValue;
                    Tuple<string, byte> key = new Tuple<string, byte>(this.presetSelected[idDevice], number);
                    if (PRESET_ROTARY_CC.ContainsKey(key))
                    {
                        rotary(idDevice, PRESET_ROTARY_CC[key][0], PRESET_ROTARY_CC[key][1], value, PRESET_ROTARY_CC[key][2]);
                        foreach (string idDev in midiOutDevicePort.Keys)
                        {
                            if (idDevice != idDev && this.presetSelected[idDev] == this.presetSelected[idDevice])
                            {
                                rotary(idDev, PRESET_ROTARY_CC[key][0], PRESET_ROTARY_CC[key][1], value, PRESET_ROTARY_CC[key][2]);
                            }
                        }
                    }
                }
                if (receivedMidiMessage.Type == MidiMessageType.PitchBendChange)
                {
                    System.Diagnostics.Debug.WriteLine("<- {0} / PitchBendChange {1} ; {2}",
                        extractId(sender.DeviceId),
                        ((MidiPitchBendChangeMessage)receivedMidiMessage).Channel,
                        ((MidiPitchBendChangeMessage)receivedMidiMessage).Bend
                        );
                    if (PRESET_FADER_CC.ContainsKey(this.presetSelected[idDevice]))
                    {
                        midiVariableFader[this.presetSelected[idDevice]] = ((MidiPitchBendChangeMessage)receivedMidiMessage).Bend;
                        midiOutLoopPort.SendMessage(new MidiControlChangeMessage(
                            (byte)PRESET_FADER_CC[this.presetSelected[idDevice]][0],
                            (byte)PRESET_FADER_CC[this.presetSelected[idDevice]][1],
                            (byte)(midiVariableFader[this.presetSelected[idDevice]] / 128)));
                    }
                }
                if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
                {
                    byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                    byte number = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                    byte value = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                    // Sélection Button 9
                    if (channel == DEVICE_CHANNEL && PRESET_BUTTON_CC.ContainsKey(new Tuple<string, byte>(this.presetSelected[idDevice], number)) && value == 127)
                    {
                        byte[] cc = PRESET_BUTTON_CC[new Tuple<string, byte>(this.presetSelected[idDevice], number)];
                        storeMidi(cc[0], cc[1], (byte)(value - getStoreMidi(cc[0], cc[1])));
                        midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, cc[2], getStoreMidi(cc[0], cc[1])));
                        midiOutLoopPort.SendMessage(new MidiControlChangeMessage(cc[0], cc[1], getStoreMidi(cc[0], cc[1])));
                        foreach (string idDev in midiOutDevicePort.Keys)
                        {
                            if (idDevice != idDev && this.presetSelected[idDev] == this.presetSelected[idDevice])
                            {
                                midiOutDevicePort[idDev].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, cc[2], getStoreMidi(cc[0], cc[1])));
                            }
                        }
                    }

                    if (channel == DEVICE_CHANNEL &&
                        (number == PRESET_AUX_1 || number == PRESET_AUX_2 ||
                        number == PRESET_AUX_3 || number == PRESET_AUX_4 ||
                        number == PRESET_AUX_5 || number == PRESET_AUX_6 ||
                        number == PRESET_AUX_7 || number == PRESET_AUX_8) &&
                        value == 127)
                    {
                        changePresetState(idDevice, number, value);
                        this.device1_aux[idDevice] = number;
                        this.presetSelected[idDevice] = PRESET_LIST[new Tuple<byte, byte, byte>(layerValue[idDevice][PRESET_A_NOTE], layerValue[idDevice][PRESET_B_NOTE], this.device1_aux[idDevice])];
                        //System.Diagnostics.Debug.WriteLine($"presetSelected {this.presetSelected}");
                        refreshButtonsLed(idDevice);
                        foreach (Tuple<string, byte> ccin in PRESET_ROTARY_CC.Keys)
                        {
                            if (ccin.Item1 == this.presetSelected[idDevice])
                            {
                                led(idDevice, PRESET_ROTARY_CC[ccin][2], getStoreMidi(PRESET_ROTARY_CC[ccin][0], PRESET_ROTARY_CC[ccin][1]));
                            }
                        }
                    }

                    // Sélection Layer A
                    if (channel == DEVICE_CHANNEL && (number == PRESET_A_NOTE || number == PRESET_B_NOTE) && value == 127)
                    {

                        if (layerValue[idDevice][number] == 0 || layerValue[idDevice][PRESET_A_NOTE] == 127 && layerValue[idDevice][PRESET_B_NOTE] == 127)
                        {
                            layerValue[idDevice][PRESET_A_NOTE] = 0;
                            layerValue[idDevice][PRESET_B_NOTE] = 0;
                            layerValue[idDevice][number] = value;
                            midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_A_LED, layerValue[idDevice][PRESET_A_NOTE]));
                            midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_B_LED, layerValue[idDevice][PRESET_B_NOTE]));
                            this.presetSelected[idDevice] = PRESET_LIST[new Tuple<byte, byte, byte>(layerValue[idDevice][PRESET_A_NOTE], layerValue[idDevice][PRESET_B_NOTE], this.device1_aux[idDevice])];
                            //System.Diagnostics.Debug.WriteLine($"presetSelected {this.presetSelected}");
                            refreshButtonsLed(idDevice);
                            foreach (Tuple<string, byte> ccin in PRESET_ROTARY_CC.Keys)
                            {
                                if (ccin.Item1 == this.presetSelected[idDevice])
                                {
                                    led(idDevice, PRESET_ROTARY_CC[ccin][2], getStoreMidi(PRESET_ROTARY_CC[ccin][0], PRESET_ROTARY_CC[ccin][1]));
                                }
                            }
                        }
                        else
                        {
                            layerValue[idDevice][PRESET_A_NOTE] = value;
                            layerValue[idDevice][PRESET_B_NOTE] = value;
                            midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_A_LED, 1));
                            midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_B_LED, 1));
                            this.presetSelected[idDevice] = PRESET_LIST[new Tuple<byte, byte, byte>(layerValue[idDevice][PRESET_A_NOTE], layerValue[idDevice][PRESET_B_NOTE], this.device1_aux[idDevice])];
                            //System.Diagnostics.Debug.WriteLine($"presetSelected {this.presetSelected}");
                            refreshButtonsLed(idDevice);
                            foreach (Tuple<string, byte> ccin in PRESET_ROTARY_CC.Keys)
                            {
                                if (ccin.Item1 == this.presetSelected[idDevice])
                                {
                                    led(idDevice, PRESET_ROTARY_CC[ccin][2], getStoreMidi(PRESET_ROTARY_CC[ccin][0], PRESET_ROTARY_CC[ccin][1]));
                                }
                            }
                        }
                    }
                }
            }
            if (sender == midiInLoopPort)
            {
                if (receivedMidiMessage.Type == MidiMessageType.ControlChange)
                {
                    byte channel = ((MidiControlChangeMessage)receivedMidiMessage).Channel;
                    byte number = ((MidiControlChangeMessage)receivedMidiMessage).Controller;
                    byte value = ((MidiControlChangeMessage)receivedMidiMessage).ControlValue;
                    foreach (Tuple<string, byte> ccin in PRESET_ROTARY_CC.Keys)
                    {
                        if (PRESET_ROTARY_CC[ccin][0] == channel && PRESET_ROTARY_CC[ccin][1] == number)
                        {
                            storeMidi(channel, number, value);
                            foreach (string idDevice in midiOutDevicePort.Keys)
                            {
                                if (ccin.Item1 == this.presetSelected[idDevice])
                                {
                                    led(idDevice, PRESET_ROTARY_CC[ccin][2], getStoreMidi(PRESET_ROTARY_CC[ccin][0], PRESET_ROTARY_CC[ccin][1]));
                                }
                            }
                        }
                    }
                    foreach (Tuple<string, byte> ccin in PRESET_BUTTON_CC.Keys)
                    {
                        if (PRESET_BUTTON_CC[ccin][0] == channel && PRESET_BUTTON_CC[ccin][1] == number)
                        {
                            storeMidi(channel, number, value);
                            foreach (string idDevice in midiOutDevicePort.Keys)
                            {
                                if (ccin.Item1 == this.presetSelected[idDevice])
                                {
                                    midiOutDevicePort[idDevice].SendMessage(new MidiNoteOnMessage(DEVICE_CHANNEL, PRESET_BUTTON_CC[ccin][2], getStoreMidi(PRESET_BUTTON_CC[ccin][0], PRESET_BUTTON_CC[ccin][1])));
                                }
                            }
                        }
                    }
                }
            }
        }
        private void MidiInDevicePort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            //System.Diagnostics.Debug.WriteLine("MidiInDevicePort_MessageReceived");
            readMidiMessage(sender, args.Message);
        }
        private void MidiInLoopPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            //System.Diagnostics.Debug.WriteLine("MidiInLoopPort_MessageReceived");
            readMidiMessage(sender, args.Message);
        }
        public JsonObject loadConfig()
        {
            try
            {
                using (StreamReader r = new StreamReader("config.json"))
                {
                    String json = r.ReadToEnd();
                    r.Close();
                    JsonNode preset = JsonNode.Parse(json);
                    JsonObject root = preset.AsObject();
                    System.Diagnostics.Debug.WriteLine(json);
                    if (root.ContainsKey("devices") && root.ContainsKey("loop"))
                    {
                        if (root["devices"].AsArray().Count > 0)
                        {
                            for (int i = 0; i < root["devices"].AsArray().Count; i++) {
                                if (root["devices"][i].AsObject().ContainsKey("deviceId") && root["devices"][i].AsObject().ContainsKey("aux"))
                                {
                                    DEVICE_ID.Add(root["devices"][i]["deviceId"].GetValue<String>());
                                    DEVICE_AUX.Add(root["devices"][i]["aux"].GetValue<String>());
                                }
                            }
                        }

                        if (root["loop"].AsObject().ContainsKey("idIn"))
                        {
                            LOOP_IN_DEVICE_ID = root["loop"]["idIn"].GetValue<String>();
                        }
                        if (root["loop"].AsObject().ContainsKey("idOut"))
                        {
                            LOOP_OUT_DEVICE_ID = root["loop"]["idOut"].GetValue<String>();
                        }
                        if (this.enumerationCompleted) {
                            this.EnumerateMidiInputDevices();
                            this.EnumerateMidiOutputDevices();
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine($"  Error reading file");
            }
            return null;
        }
        public void saveConfig()
        {
            string configurationStr = @"{""devices"": [], ""loop"": {""idIn"": ""MIDII_44881789"", ""idOut"": ""MIDII_4488178A""}}";
            JsonNode configuration = JsonNode.Parse(configurationStr);
            if (this.panel4.Controls.Count > 0)
            {
                for(int i = 0; i < panel4.Controls.Count; i++)
                {
                    System.Windows.Forms.Panel chkbxPanel = (System.Windows.Forms.Panel)panel4.Controls[i];
                    CheckBox chkbx = (CheckBox)chkbxPanel.Controls[1];
                    ComboBox combo = (ComboBox)chkbxPanel.Controls[0];
                    if (chkbx.Checked)
                    {
                        JsonNode dev = JsonNode.Parse(@"{""deviceId"": ""MIDII_E61DC05C"",""aux"":""AUX1_1_8""}");
                        dev["deviceId"] = chkbx.Name;
                        dev["aux"] = (string)combo.SelectedItem;
                        configuration["devices"].AsArray().Add(dev);
                    }
                }
            }
            if(this.midiInLoopPort != null && this.midiOutLoopPort != null)
            {
                configuration["loop"]["idIn"] = extractId(this.midiInLoopPort.DeviceId);
                configuration["loop"]["idOut"] = extractId(this.midiOutLoopPort.DeviceId);
            }
            System.Diagnostics.Debug.WriteLine($"DevideID  => preset ");
            System.Diagnostics.Debug.WriteLine("Saving global configuration");
            try
            {
                using (StreamWriter w = new StreamWriter("config.json"))
                {
                    JsonObject root = configuration.AsObject();
                    //root["version"] = BOARD_VERSION;
                    System.Diagnostics.Debug.WriteLine(root.ToString());
                    w.Write(root.ToString());
                    w.Close();
                }
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine($"Error writing");
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            loadConfig();
        }

        private void save_Click(object sender, EventArgs e)
        {
            saveConfig();
        }
    }
}