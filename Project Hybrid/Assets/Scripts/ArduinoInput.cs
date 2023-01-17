using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class will read Arduino data using the SerialPort in your laptop.
/// Note: vorige keer had ik dit script als MonoBehaviour, maar waarschijnlijk zal het ook gewoon als static script kunnen met []
/// </summary>
public class ArduinoInput : MonoBehaviour
{
    #region Fields
    public static bool isPortOpen;
    private SerialPort serialPort;

    private InputManager inputManager;
    private Dictionary<int, Queue<bool>> inputLists = new Dictionary<int, Queue<bool>>();
    private int inputListsCapacity = 10;
    #endregion

    #region Opening Serial Port
    private void Start()
    {
        inputManager = ServiceLocator.GetService<InputManager>();
        TryOpenSerialPort();

        for (int i = 1; i <= 5; i++)
        {
            inputLists.Add(i, new Queue<bool>(inputListsCapacity));
        }
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
        SendInput();

        if (!isPortOpen) return;

        // Optional
        if (Input.GetKeyDown(KeyCode.X)) CloseSerialPort();
    }

    // Parses data from the serial.println of the arduino.
    public void SendInput()
    {
        if (inputManager == null) return;

        //string dataString = serialPort.ReadLine();
        string dataString = "NO LASER 5";

        if (dataString.Contains("DETECTED"))
        {
            int id = 0;
            try
            {
                id = int.Parse(dataString[9].ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Could not parse Dam ID");
                return;
            }

            bool isDamOpen = true;

            Queue<bool> inputList = inputLists[id];
            if (inputList.Count >= inputListsCapacity) inputList.Dequeue();
            inputList.Enqueue(isDamOpen);

            if (IsMoreThanHalfTrue(inputList))
            {
                inputManager.ReceiveDamValue(id, isDamOpen);
            }
        }
        else if (dataString.Contains("NO LASER"))
        {
            int id = 0;
            try
            {
                id = int.Parse(dataString[9].ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Could not parse Dam ID");
                return;
            }

            bool isDamOpen = false;

            Queue<bool> inputList = inputLists[id];
            if (inputList.Count >= inputListsCapacity) inputList.Dequeue();
            inputList.Enqueue(isDamOpen);

            if (!IsMoreThanHalfTrue(inputList))
            {
                inputManager.ReceiveDamValue(id, isDamOpen);
            }

            inputManager.ReceiveDamValue(id, isDamOpen);
        }
    }

    public bool IsMoreThanHalfTrue(IEnumerable<bool> _list)
    {
        return _list.Where(x => x == true).Count() >= inputListsCapacity/2;
    }

    // Parses data from the serial.println of the arduino.
    /*public void SendInput()
    {
        if (inputManager == null) return;

        //string dataString = serialPort.ReadLine();
        string dataString = "Dam1: 1";

        if (dataString.Contains("Dam"))
        {
            int id = 0;
            try
            {
                id = int.Parse(dataString[3].ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Could not parse Dam ID");
                return;
            }

            bool isDamOpen = false;
            try
            {
                int damValue = int.Parse(dataString[6].ToString());
                isDamOpen = Convert.ToBoolean(damValue);
            }
            catch (Exception e)
            {
                Debug.Log($"Could not parse Dam Value of Dam {id}");
                return;
            }

            inputManager.ReceiveDamValue(id, isDamOpen);
        }
        else if (dataString.Contains("Button"))
        {

        }
    }*/
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
