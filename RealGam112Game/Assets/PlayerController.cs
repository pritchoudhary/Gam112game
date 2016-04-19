using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Slider VelocitySlider;
    public Text winText;
    public Text speedText;
    public Rigidbody rb;
    public Rigidbody motorRb;

    public GameObject playerCar;
    public WheelCollider fWheel1;
    public WheelCollider fWheel2;
    public WheelCollider rWheel1;
    public WheelCollider rWheel2;
    public Transform fWheel1Trans;
    public Transform fWheel2Trans;
    public Transform rWheel1Trans;
    public Transform rWheel2Trans;

    public ParticleSystem particleSystem;

    WheelFrictionCurve wheelFric;

    WheelHit hit;

    public float motorMax = 100f;
    private float motor = 0f;

    private bool groundMat;

    // Keep track of current time
    public float currentTime = 60;
    public float maxCurrentTime;
    public float score = 0;

    public float acceleration = 0f;
    public float normalizing = 0f;

    private Vector3 cameraPos;
    public float cameraDistance = 10;

    public GameObject gameOverElements;
    public GameObject winElements;

    float maxRot = 55;
    float minRot = 315;


    // Use this for initialization
    void Start ()
    {
        maxCurrentTime = currentTime;

        gameOverElements.SetActive(false);
        winElements.SetActive(false);

        rb = playerCar.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if(playerCar.transform.position.z > 0)
        //{
        //    Vector3 newPos = new Vector3(playerCar.transform.position.x, playerCar.transform.position.y, 0);

        //    playerCar.transform.position = newPos;
        //}

        normalizing = 100 / VelocitySlider.maxValue;
        acceleration = VelocitySlider.value * normalizing;
        //Debug.Log(acceleration);

        currentTime -= Time.deltaTime;

        if (playerCar != null)
        {
            fWheel1Trans.Rotate(-fWheel1.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            fWheel2Trans.Rotate(fWheel2.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            rWheel1Trans.Rotate(-rWheel1.rpm / 60 * 360 * Time.deltaTime, 0, 0);
            rWheel2Trans.Rotate(rWheel2.rpm / 60 * 360 * Time.deltaTime, 0, 0);

            

            groundMat = rWheel1.GetGroundHit(out hit);

            if (groundMat)
            {
                wheelFric = rWheel1.forwardFriction;

                particleSystem.emissionRate = (-rWheel1.rpm / 100);

                if (hit.collider.transform.CompareTag("Asphalt"))
                {
                    if (acceleration <= 90) // TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Make a variable with all the magic numbers >D@!#
                    {
                        wheelFric.stiffness = 1f;
                    }
                    else
                    {
                        wheelFric.stiffness = -(acceleration - 90) / (190 - 90) + 1;
                    }
                    particleSystem.startColor = Color.black;
                }
                else if (hit.collider.transform.CompareTag("Gravel"))
                {
                    if (acceleration <= 70)
                    {
                        wheelFric.stiffness = 1f;
                    }
                    else
                    {
                        wheelFric.stiffness = -(acceleration - 70) / (170 - 70) + 1;
                    }
                    particleSystem.startColor = Color.gray;
                }
                else if (hit.collider.transform.CompareTag("Dirt"))
                {
                    if (acceleration <= 50)
                    {
                        wheelFric.stiffness = 1f;
                    }
                    else
                    {
                        wheelFric.stiffness = -(acceleration - 50) / (150 - 50) + 1;
                    }
                    particleSystem.startColor = Color.blue;
                }
                else if (hit.collider.transform.CompareTag("Grass"))
                {
                    if (acceleration <= 30)
                    {
                        wheelFric.stiffness = 1f;
                    }
                    else
                    {
                        wheelFric.stiffness = -(acceleration - 30) / (130 - 30) + 1;
                    }
                    particleSystem.startColor = Color.green;

                }
                else if (hit.collider.transform.CompareTag("Finish"))
                {
                    Time.timeScale = 0;
                    score = (maxCurrentTime - currentTime);
                    //Debug.Log(score);
                    winElements.SetActive(true);
                    winText.text = "Win! \nTime: " + score.ToString("F2") + "s";
                }

                rWheel1.forwardFriction = wheelFric;
                rWheel2.forwardFriction = wheelFric;
                fWheel1.forwardFriction = wheelFric;
                fWheel2.forwardFriction = wheelFric;
            }

            else if (!groundMat)
            {
                particleSystem.emissionRate = 0;
            }

            // Make camera follow player
            cameraPos = playerCar.transform.position;
            // Make camera not be on the player
            cameraPos.z = cameraDistance;
			cameraPos.y += 3;

            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, cameraPos, 1);

            speedText.text = rb.velocity.magnitude.ToString("F2") + " m/s";

            // Max rotation
            Debug.Log(playerCar.transform.eulerAngles.x);

            
        }

        else if (playerCar == null)
        {
            //Time.timeScale = 0;
            gameOverElements.SetActive(true);
        }

    }

    void FixedUpdate()
    {
        if (playerCar != null)
        {
            motor = VelocitySlider.value;

            rWheel1.motorTorque = motorMax * -motor;
            rWheel2.motorTorque = motorMax * -motor;

            if (playerCar.transform.eulerAngles.x > 55 && playerCar.transform.eulerAngles.x < 300)
            {
                playerCar.transform.rotation = Quaternion.Euler(maxRot, 90, 0);
                motorRb.AddForce(-motorRb.transform.up * 100);
                //Debug.Log("working");
            }
            playerCar.transform.rotation = Quaternion.Euler(playerCar.transform.eulerAngles.x, 90, 0);
            //if (playerCar.transform.eulerAngles.x < minRot && playerCar.transform.eulerAngles.x > 180)
            //{
            //    playerCar.transform.rotation = Quaternion.Euler(minRot, 90, 0);
            //}
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        Application.LoadLevel("MainScene");
    }
}
