using System;
using System.IO.Ports;
using UnityEngine;

/// <summary>
/// This class will read Arduino data using the SerialPort in your laptop.
/// Note: vorige keer had ik dit script als MonoBehaviour, maar waarschijnlijk zal het ook gewoon als static script kunnen met []
/// </summary>
public class ArduinoInput : MonoBehaviour
{
    #region Fields
    private SerialPort serialPort;
    public static bool isPortOpen;
    #endregion

    #region Opening Serial Port
    private void Start()
    {
        TryOpenSerialPort();
    }

    private void TryOpenSerialPort()
    {
        try
        {
            serialPort = new SerialPort("Com3", 9600);
            serialPort.Open();
            isPortOpen = true;
        }
        catch (Exception e)
        {
            // When port failed to open, set isPortOpen to false
            isPortOpen = false;
        }
    }
    #endregion

    #region Using the Arduino Data
    private void Update()
    {
        if (!isPortOpen) return;

        // Optional
        if (Input.GetKeyDown(KeyCode.X)) CloseSerialPort();
    }

    // Parses data from the serial.println of the arduino.
    private float GetWaterLevel()
    {
        throw new System.NotImplementedException();

        // Example:
/*        string dataString = serialPort.ReadLine();
        if (dataString[0] == 'X')
        {
            HandleJoystickInput(dataString);
        }*/
    }
    #endregion

    #region Parsing Examples
    // Parsing Examples
    /*    private void HandleButtonInput(string dataString)
        {
            string[] input = dataString.Split(',', ' ');

            leftInput = float.Parse(input[0]);
            rightInput = float.Parse(input[1]);

            Debug.Log(getHorizontal());
        }*/

    /*    public static void HandleJoystickInput(string dataString)
        {
            string[] input = dataString.Split(',');
            Debug.Log(input.Length);

            // Removing the "X: " from the string
            horizontal = float.Parse(input[0].Substring(3));
            vertical = float.Parse(input[1].Substring(3));

            if (horizontal > 1) horizontal = 1;
            if (horizontal < -1) horizontal = -1;
            if (vertical > 1) vertical = 1;
            if (vertical < -1) vertical = -1;
        }*/
    #endregion

    #region Closing the Serial Port
    public void CloseSerialPort()
    {
        if (isPortOpen)
        {
            serialPort.Close();
            isPortOpen = false;
        }

        serialPort.Dispose();
    }

    private void OnDestroy()
    {
        CloseSerialPort();
    }

    private void OnApplicationQuit()
    {
        CloseSerialPort();
    }
    #endregion
}
