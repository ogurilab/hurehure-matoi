using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialPlayerControl : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip Punchsound;
    public float baseMoveSpeed = 50.0f;
    public float slowSpeed = 25.0f;
    public float fastSpeed = 75.0f;
    public float baseSideMoveSpeed = 50.0f;
    public float SideslowSpeed = 25.0f;
    public float moveSpeedMultiplier = 1.0f;
    private float moveSpeed;

    private AudioSource mutekiAudioSource;
    private List<GameObject> hiddenChildren = new List<GameObject>();
    private bool canDestroyHouse = false;
    private float destroyHouseDuration = 10000000000f;
    private float destroyHouseTimer = 0f;

    public float mutekiDuration = 5f;

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

    //屋根
    public Vector3 rightPosition = new Vector3(0f, 1.5f, 15f);
    public Vector3 leftPosition = new Vector3(0f, 1.5f, -15f);
    public Vector3 centerPosition = new Vector3(0f, 1.5f, 0f);
    private Vector3 targetPosition;
    private bool moveThreadRunning = true;

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

    private bool sasuslow = false;
    private float sasuslowTimer = 0f;

    private bool tutorial1 = true;
    private bool tutorial2 = false;
    private bool tutorial3 = false;
    private bool tutorial4 = true;
    private bool tutorial5 = true;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;
    public GameObject panel5;
    public GameObject panel6;
    private bool scene = false;
    public float timer = 1.5f;
    public GameObject wall;

    public FadeSceneLoader fadeSceneLoader;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        fadeSceneLoader.StartFadeIn();

        GameObject sasuKunObject = GameObject.Find("nisesasu");

        if (sasuKunObject != null)
        {
            play = sasuKunObject.GetComponent<Nisesasu>();
        }

        // StartSerialPortThread();

        previousTime = Time.time;
        lastNoDataLogTime = Time.time;
        audioSource = GetComponent<AudioSource>();

        gameManager = GameObject.Find("date");

        moveSpeed = baseMoveSpeed;

        characterTransform = transform;
    }

    void Update()
    {
        if (tutorial1 || tutorial2)
        {
            GetInput();
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            HandlePunch();
        }

        // if (play != null)
        // {
        //     ProcessForwardSerialData();
        // }

        if (scene == true)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                fadeSceneLoader.CallCoroutine();
            }
        }
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

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene("StartScene");
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                fadeSceneLoader.CallCoroutine();
            }
        }
    }

    private void GetInput()
    {
        if (tutorial1 || tutorial2)
        {
            horizontalInput = tutorial2 ? Input.GetAxis("Horizontal") : 0;
            verticalInput = tutorial1 ? Input.GetAxis("Vertical") : 0;
        }
    }

    private void HandlePunch()
    {
        if (tutorial3)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

            foreach (var hitCollider in hitColliders)
            {
                anim.SetTrigger("Sasumata");
                if (hitCollider.CompareTag("House"))
                {
                    panel2.SetActive(false);
                    Zcount += 1;
                    anim.SetTrigger("Sasumata");
                    hitCollider.GetComponent<hp>().damage(scorePoint);
                    if (Zcount >= 3 || odango < 50)
                    {
                        Destroy(hitCollider.gameObject);
                        Vector3 newPosition = hitCollider.transform.position;
                        newPosition.y = 7;
                        GameObject newob = Instantiate(
                            newObjectPrefab,
                            newPosition,
                            hitCollider.transform.rotation
                        );
                        if (tutorial4)
                        {
                            panel3.SetActive(false);
                            panel4.SetActive(true);
                            tutorial4 = false;
                        }
                        newob.SetActive(true);
                        audioSource.PlayOneShot(Punchsound);
                        Zcount = 0;

                        tutorial1 = true;
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
                        if (tutorial5)
                        {
                            panel2.SetActive(false);
                            panel5.SetActive(true);
                            tutorial5 = false;
                        }
                        wall.gameObject.SetActive(false);
                        Destroy(hitCollider.gameObject);
                        Vector3 newPosition = hitCollider.transform.position;
                        newPosition.y = 7;
                        GameObject newob = Instantiate(
                            newObjectPrefab,
                            newPosition,
                            hitCollider.transform.rotation
                        );
                        newob.SetActive(true);
                        audioSource.PlayOneShot(Punchsound);
                        tutorial1 = true;
                        panel2.SetActive(false);
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
                    tutorial1 = true;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("P_UP"))
        {
            if (tutorial5)
            {
                panel5.SetActive(true);
            }
            Destroy(collision.gameObject);
            canDestroyHouse = true;
            odango = 1;
            destroyHouseTimer = destroyHouseDuration;
            foreach (Transform child in transform)
            {
                if (child.CompareTag("P_UP"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

        if (collision.gameObject.CompareTag("tutorial1"))
        {
            transform.position = new Vector3(
                collision.transform.position.x,
                transform.position.y,
                collision.transform.position.z
            );
            tutorial1 = false;
            tutorial3 = true;
            collision.gameObject.SetActive(false);
            panel1.SetActive(false);
            panel3.SetActive(true);
        }
        else if (collision.gameObject.CompareTag("tutorial2"))
        {
            transform.position = new Vector3(
                collision.transform.position.x,
                transform.position.y,
                collision.transform.position.z
            );
            tutorial2 = true;
            tutorial1 = false;
            collision.gameObject.SetActive(false);
            panel4.SetActive(false);
            panel2.SetActive(true);
        }
        else if (collision.gameObject.CompareTag("Destroy"))
        {
            collision.gameObject.SetActive(false);
            panel3.SetActive(false);
            panel4.SetActive(true);
        }
        else if (collision.gameObject.CompareTag("Go!"))
        {
            collision.gameObject.SetActive(false);
            panel5.SetActive(false);
            panel6.SetActive(true);
        }

        if (collision.gameObject.CompareTag("Goal"))
        {
            scene = true;
        }
    }

    private void ProcessForwardSerialData()
    {
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
            if (tutorial1)
            {
                if (sasuslow)
                {
                    UpdateFalseAnimationState(accY, gyroY);
                }
                else
                {
                    AdjustMoveSpeed(accY);
                    UpdateAnimationState(accY, gyroY);
                }
            }

            HandleObjectDetection(accY, gyroY);
        }
        else
        {
            LogNoData();
        }
    }

    private void yanenoidou()
    {
        while (moveThreadRunning)
        {
            try
            {
                receivedData = moveSerialPort.ReadLine().Trim();

                if (tutorial2 == false)
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
                        }
                        else if (receivedData == "2")
                        {
                            targetPosition = leftPosition;
                        }
                        else if (receivedData == "0")
                        {
                            targetPosition = centerPosition;
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
        if (moveThreadRunning)
        {
            moveThreadRunning = false;
            if (moveThread != null)
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
        try
        {
            moveSerialPort = new SerialPort("/dev/tty.usbserial-795292008B", 19200);
            moveSerialPort.Open();
            moveSerialPort.ReadTimeout = 500;
            moveThreadRunning = true;
            moveThread = new Thread(yanenoidou);
            moveThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to open move serial port: {e.Message}");
        }
    }
}
