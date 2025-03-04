using System;
using System.IO.Ports;
using UnityEngine;

public class SerialPortManager1 : MonoBehaviour
{
    private static SerialPortManager1 _instance;
    public static SerialPortManager1 Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SerialPortManager");
                _instance = go.AddComponent<SerialPortManager1>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private SerialPort serialPort;
    public string portName = "/dev/tty.M5StickCPlus7";
    public int baudRate = 115200;
    public bool IsOpen => serialPort != null && serialPort.IsOpen;

    public void OpenSerialPort()
    {
        if (serialPort == null)
        {
            serialPort = new SerialPort(portName, baudRate);
            try
            {
                serialPort.Open();
                Debug.Log("Serial port opened successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to open serial port: {e.Message}");
            }
        }
    }

    public string ReadLine()
    {
        if (IsOpen && serialPort.BytesToRead > 0)
        {
            try
            {
                return serialPort.ReadLine();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading from serial port: {e.Message}");
            }
        }
        return null;
    }
}