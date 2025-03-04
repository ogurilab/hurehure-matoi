using System.IO.Ports;
using UnityEngine;

public class Nisesasu : MonoBehaviour
{
    [HideInInspector]
    public string receivedData;

    private SerialPort forwardSerialPort;

    void Start()
    {
        forwardSerialPort = new SerialPort("/dev/tty.M5StickCPlus7", 115200);
        forwardSerialPort.Open();
    }

    void Update()
    {
        if (forwardSerialPort.IsOpen)
        {
            receivedData = forwardSerialPort.ReadLine();
        }
    }
}
