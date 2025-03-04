using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip Punchsound;
    public float baseMoveSpeed = 50.0f;
    public float slowSpeed = 20.0f;
    public float fastSpeed = 75.0f;
    public float baseSideMoveSpeed = 50.0f;
    public float SideslowSpeed = 25.0f;
    public float moveSpeedMultiplier = 1.0f;
    private float moveSpeed;

    private AudioSource mutekiAudioSource;
    private List<GameObject> hiddenChildren = new List<GameObject>();
    private bool canDestroyHouse = false;
    private float destroyHouseDuration = 10f;
    private float destroyHouseTimer = 0f;

    private Animator anim;
    private bool runFlg;
    public TextMeshProUGUI goalText;
    public bool goalOn;

    public GameObject targetObject;

    private float previousTime;
    private float lastNoDataLogTime;
    public float noDataLogInterval = 1.0f;

    private Color currentColor;
    private float lastColorChangeTime;

    private const float gyroYThresholdHigh = 200f;
    private const float gyroYThresholdLow = -200f;
    private const float accYThresholdHigh = 1.3f;
    private const float accYThresholdLow = -0.35f;

    public float range = 4;
    public int scorePoint = 1;
    private GameObject gameManager;
    public GameObject newObjectPrefab;

    int smoothingLoop_Shk = 0;
    List<float> aveShakings = new List<float>();

    [HideInInspector]
    public float Smoothed_Shake = 0;

    [SerializeField]
    int smoothTime_Shk = 10;

    private SerialPort forwardSerialPort;
    private SerialPort moveSerialPort;

    [HideInInspector]
    private Nisesasu play;
    public string receivedData;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 targetVelocity = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private string lastData = "";

    private Rigidbody rb;

    private Thread moveThread;

    public int Zcount = 0;

    public Vector3 rightPosition = new Vector3(0f, 1.5f, 15f);
    public Vector3 leftPosition = new Vector3(0f, 1.5f, -15f);
    public Vector3 centerPosition = new Vector3(0f, 1.5f, 0f);
    private Vector3 targetPosition;
    private bool moveThreadRunning = false;

    private float lastShakeTime;
    private float shakeInterval = 0.5f;
    private float accelerationDuration = 0.45f;
    private float accelerationEndTime = 0f;
    private bool isAccelerating = false;

    private float accelerationFactor = 0f;

    private Animator anim2;
    public GameObject character2;
    private Transform characterTransform;

    private int odango = 100;
    public AudioClip sound1;

    private bool sasuslow = false;
    private float sasuslowTimer = 0f;

    private TimeSlider timeSlider;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (character2 != null)
        {
            anim2 = character2.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Character2 is not set!");
        }
        rb = GetComponent<Rigidbody>();

        GameObject sasuKunObject = GameObject.Find("nisesasu");

        timeSlider = FindObjectOfType<TimeSlider>();

        if (sasuKunObject != null)
        {
            play = sasuKunObject.GetComponent<Nisesasu>();
        }
        StartSerialPortThread();
        string portName = "/dev/tty.usbserial-795292008B";
        if (System.Array.IndexOf(SerialPort.GetPortNames(), portName) >= 0)
        {
            moveSerialPort = new SerialPort(portName, 19200);
            try
            {
                moveSerialPort.Open();
                moveSerialPort.ReadTimeout = 500;
                moveThreadRunning = true;
                moveThread = new Thread(SideSerialData);
                moveThread.Start();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to open move serial port: {e.Message}");
            }
        }

        previousTime = Time.time;
        lastNoDataLogTime = Time.time;
        audioSource = GetComponent<AudioSource>();

        gameManager = GameObject.Find("date");

        moveSpeed = baseMoveSpeed;
        characterTransform = transform;
    }

    void Update()
    {
        if (!timeSlider.stopflag)
        {
            GetInput();

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                HandlePunch();
            }
        }

        // if (play != null)
        // {
        //     ProcessForwardSerialData();
        // }

        if (canDestroyHouse)
        {
            destroyHouseTimer -= Time.deltaTime;
            if (destroyHouseTimer <= 0)
            {
                canDestroyHouse = false;
                odango = 100;
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("P_UP"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
        if (sasuslow)
        {
            sasuslowTimer -= Time.deltaTime;
            if (sasuslowTimer <= 0)
            {
                sasuslow = false;
                foreach (Transform child in transform)
                {
                    if (child.CompareTag("Sdown"))
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void HandlePunch()
    {
        if (anim2 != null)
        {
            SetTriggerIfExists(anim2, "SpinAttack");
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("House"))
            {
                Zcount += 1;
                anim.SetTrigger("Sasumata");
                hitCollider.GetComponent<hp>().damage(scorePoint);
                if (Zcount >= 3 || odango < 50)
                {
                    Destroy(hitCollider.gameObject);
                    gameManager.GetComponent<date>().AddScore(scorePoint);
                    Vector3 newPosition = hitCollider.transform.position;
                    newPosition.y = 7;
                    GameObject newob = Instantiate(
                        newObjectPrefab,
                        newPosition,
                        hitCollider.transform.rotation
                    );
                    newob.SetActive(true);
                    audioSource.PlayOneShot(Punchsound);
                    Zcount = 0;

                    break;
                }
            }
            else if (hitCollider.CompareTag("House_1"))
            {
                Zcount += 1;
                anim.SetTrigger("Sasumata");
                hitCollider.GetComponent<hp>().damage(scorePoint);
                if (Zcount >= 5 || odango < 50)
                {
                    Destroy(hitCollider.gameObject);
                    gameManager.GetComponent<date>().AddScore(scorePoint);
                    Vector3 newPosition = hitCollider.transform.position;
                    newPosition.y = 7;
                    GameObject newob = Instantiate(
                        newObjectPrefab,
                        newPosition,
                        hitCollider.transform.rotation
                    );
                    newob.SetActive(true);
                    audioSource.PlayOneShot(Punchsound);
                    Zcount = 0;
                    break;
                }
            }
            else if (hitCollider.CompareTag("House_2"))
            {
                Zcount += 1;
                anim.SetTrigger("Sasumata");
                hitCollider.GetComponent<hp>().damage(scorePoint);
                if (Zcount >= 10 || odango < 50)
                {
                    Destroy(hitCollider.gameObject);
                    gameManager.GetComponent<date>().AddScore(scorePoint);
                    Vector3 newPosition = hitCollider.transform.position;
                    newPosition.y = 7;
                    GameObject newob = Instantiate(
                        newObjectPrefab,
                        newPosition,
                        hitCollider.transform.rotation
                    );
                    newob.SetActive(true);
                    audioSource.PlayOneShot(Punchsound);
                    Zcount = 0;
                    break;
                }
            }
            else if (hitCollider.CompareTag("False"))
            {
                Destroy(hitCollider.gameObject);
                gameManager.GetComponent<date>().SubtractScore(scorePoint);
                Vector3 newPosition = hitCollider.transform.position;
                newPosition.y = 7;
                GameObject newob = Instantiate(
                    newObjectPrefab,
                    newPosition,
                    hitCollider.transform.rotation
                );
                newob.SetActive(true);
                anim.SetTrigger("Sasumata");
                audioSource.PlayOneShot(Punchsound);
                moveSpeed = slowSpeed;
                sasuslow = true;
                sasuslowTimer = 3;

                StartCoroutine(RestoreSpeedAfterDelay(3f));

                foreach (Transform child in transform)
                {
                    if (child.CompareTag("Sdown"))
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("P_UP"))
        {
            Destroy(collision.gameObject);
            canDestroyHouse = true;
            odango = 1;
            destroyHouseTimer = destroyHouseDuration;
            audioSource.PlayOneShot(sound1);
            foreach (Transform child in transform)
            {
                if (child.CompareTag("P_UP"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }

    private void ProcessForwardSerialData()
    {
        if (timeSlider.stopflag)
        {
            return;
        }
        try
        {
            receivedData = play.receivedData;
            string[] parts = receivedData.Split(';');

            if (parts.Length == 2)
            {
                ProcessForwardDataParts(parts);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading from forward serial port: {e.Message}");
        }
    }

    private void ProcessForwardDataParts(string[] parts)
    {
        string[] accelValues = parts[0].Substring(4).Split(',');
        string[] gyroValues = parts[1].Substring(5).Split(',');

        if (accelValues.Length == 3 && gyroValues.Length == 3)
        {
            float accY = float.Parse(accelValues[1]);
            float gyroY = float.Parse(gyroValues[1]);

            if (sasuslow)
            {
                UpdateFalseAnimationState(accY, gyroY);
            }
            else
            {
                AdjustMoveSpeed(accY);
                UpdateAnimationState(accY, gyroY);
            }
            HandleObjectDetection(accY, gyroY);
        }
        else
        {
            LogNoData();
        }
    }

    private void SideSerialData()
    {
        while (moveThreadRunning)
        {
            try
            {
                receivedData = moveSerialPort.ReadLine().Trim();

                if (timeSlider.stopflag)
                {
                    targetPosition = centerPosition;
                    continue;
                }

                if (receivedData != lastData)
                {
                    if (receivedData == "1" || receivedData == "2" || receivedData == "0")
                    {
                        Vector3 newVelocity = Vector3.zero;
                        if (receivedData == "1")
                        {
                            targetPosition = rightPosition;
                            Debug.LogError("1");
                        }
                        else if (receivedData == "2")
                        {
                            targetPosition = leftPosition;
                            Debug.LogError("2");
                        }
                        else if (receivedData == "0")
                        {
                            targetPosition = centerPosition;
                            Debug.LogError("0");
                        }
                        lastData = receivedData;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error reading from move serial port: {e.Message}");
            }
        }
    }

    private void HandleObjectDetection(float accY, float gyroY)
    {
        if (
            (gyroY > gyroYThresholdHigh || gyroY < gyroYThresholdLow)
            && accY <= accYThresholdHigh
            && accY >= accYThresholdLow
        )
        {
            HandlePunch();
        }
    }

    private void UpdateAnimationState(float accY, float gyroY)
    {
        float smoothedGyroY = SmoothingShake(gyroY);

        runFlg =
            (smoothedGyroY <= gyroYThresholdHigh || smoothedGyroY >= gyroYThresholdLow)
            && (accY > accYThresholdHigh || accY < accYThresholdLow);
        verticalInput = runFlg ? 15 : 0;

        anim.SetBool("Run", runFlg);
    }

    private void UpdateFalseAnimationState(float accY, float gyroY)
    {
        float smoothedGyroY = SmoothingShake(gyroY);

        runFlg =
            (smoothedGyroY <= gyroYThresholdHigh || smoothedGyroY >= gyroYThresholdLow)
            && (accY > accYThresholdHigh || accY < accYThresholdLow);
        verticalInput = runFlg ? 2 : 0;

        anim.SetBool("Run", runFlg);
    }

    private void LogNoData()
    {
        if (Time.time - lastNoDataLogTime >= noDataLogInterval)
        {
            Debug.Log("データが受信されていません。");
            lastNoDataLogTime = Time.time;
        }
    }

    private void AdjustMoveSpeed(float accY)
    {
        float currentTime = Time.time;
        float interval = currentTime - lastShakeTime;

        if (isAccelerating)
        {
            if (currentTime >= accelerationEndTime)
            {
                isAccelerating = false;
                moveSpeed = baseMoveSpeed;
            }
        }
        else
        {
            if (accY > accYThresholdHigh || accY < accYThresholdLow)
            {
                if (interval < shakeInterval)
                {
                    accelerationFactor -= Mathf.Abs(accY) * 0.5f;
                    accelerationFactor = Mathf.Max(0, accelerationFactor);
                }
                else if (interval >= shakeInterval)
                {
                    accelerationFactor += Mathf.Abs(accY) * 0.1f;
                    accelerationFactor = Mathf.Min(accelerationFactor, 0.5f);
                    isAccelerating = true;
                    accelerationEndTime = currentTime + accelerationDuration;

                    lastShakeTime = currentTime;
                }
                lastShakeTime = currentTime;
            }
            moveSpeed = baseMoveSpeed * (1 + accelerationFactor);
            moveSpeed = Mathf.Min(moveSpeed, 5.0f);
        }
    }

    void FixedUpdate()
    {
        Vector3 move = Vector3.zero;

        Vector3 targetPositionWithoutX = new Vector3(
            transform.position.x,
            targetPosition.y,
            targetPosition.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPositionWithoutX,
            Time.deltaTime * baseSideMoveSpeed
        );

        if (horizontalInput > 0)
        {
            targetPosition = rightPosition;
        }
        else if (horizontalInput < 0)
        {
            targetPosition = leftPosition;
        }
        else
        {
            targetPosition = centerPosition;
        }

        move += transform.forward * verticalInput * moveSpeed * Time.deltaTime;

        rb.velocity = move / Time.deltaTime;

        runFlg = move.magnitude > 0;
        anim.SetBool("Run", runFlg);

        float zPosition = transform.position.z;

        if (zPosition > 5)
        {
            anim2.SetBool("Swing_R", true);
            anim2.SetBool("Swing_L", false);
            anim2.SetBool("Swing_C", false);
        }
        else if (zPosition < -5)
        {
            anim2.SetBool("Swing_R", false);
            anim2.SetBool("Swing_L", true);
            anim2.SetBool("Swing_C", false);
        }
        else if (move.magnitude > 0 && (zPosition > -5 && zPosition < 5))
        {
            anim2.SetBool("Swing_R", false);
            anim2.SetBool("Swing_L", false);
            anim2.SetBool("Swing_C", true);
        }
        else
        {
            anim2.SetBool("Swing_R", false);
            anim2.SetBool("Swing_L", false);
            anim2.SetBool("Swing_C", false);
        }
    }

    float SmoothingShake(float Shk)
    {
        float Fr_sum = 0;
        aveShakings.Add(Shk);

        if (smoothingLoop_Shk >= smoothTime_Shk)
        {
            aveShakings.RemoveAt(0);
        }
        else
        {
            smoothingLoop_Shk++;
        }

        for (int i = 0; i < aveShakings.Count; i++)
        {
            Fr_sum += aveShakings[i];
        }

        return Fr_sum / aveShakings.Count;
    }

    private void OnDestroy()
    {
        if (moveThread != null && moveThreadRunning)
        {
            moveThreadRunning = false;
            moveThread.Join();
        }

        if (forwardSerialPort != null && forwardSerialPort.IsOpen)
        {
            forwardSerialPort.Close();
        }

        if (moveSerialPort != null && moveSerialPort.IsOpen)
        {
            moveSerialPort.Close();
        }
    }

    private void SetTriggerIfExists(Animator animator, string triggerName)
    {
        bool exists = false;
        foreach (var param in animator.parameters)
        {
            if (param.name == triggerName)
            {
                exists = true;
                break;
            }
        }
        if (exists)
        {
            animator.SetTrigger(triggerName);
        }
    }


    private IEnumerator RestoreSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed = baseMoveSpeed;
        sasuslow = false;
    }

    void StartSerialPortThread()
    {
        Thread serialThread = new Thread(OpenSerialPorts);
        serialThread.Start();
    }

    void OpenSerialPorts()
    {

        string portName = "/dev/tty.usbserial-795292008B";
        if (System.Array.IndexOf(SerialPort.GetPortNames(), portName) >= 0)
        {
            try
            {
                moveSerialPort = new SerialPort("/dev/tty.usbserial-795292008B", 19200);
                moveSerialPort.Open();
                moveSerialPort.ReadTimeout = 500;
                moveThreadRunning = true;
                moveThread = new Thread(SideSerialData);
                moveThread.Start();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to open move serial port: {e.Message}");
            }
        }
    }
}
