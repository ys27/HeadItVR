using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BallShooter : MonoBehaviour
{
    public GameObject Target;
    public float firingAngle = 70.0f;
    public float gravity = 9.8f; //change for different speed

    public Transform Projectile;
    private Transform myTransform;

    private GameObject goalLineGameobj;
    private goalLineTech goalLineScript;

    private GameObject SystemObject;
    private DatabaseScript dbscript;

    public Text attemptText;
    private int attempts;

    private AudioSource kickAudio;

    void Awake()
    {
        myTransform = transform;

        goalLineGameobj = GameObject.Find("GoalLineTechnology");
        goalLineScript = goalLineGameobj.GetComponent<goalLineTech>();

        SystemObject = GameObject.Find("System");
        dbscript = SystemObject.GetComponent<DatabaseScript>();

        kickAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        //Vector3 speed = calculateBestThrowSpeed(myTransform.position, Target.transform.position, 1.5f);
        //Projectile.GetComponent<Rigidbody>().AddForce(speed, ForceMode.VelocityChange);
        //StartCoroutine(SimulateProjectile());
        // StartCoroutine(ThrowObject());
        
    }

    //Coroutine to throw the ball
    public IEnumerator ShootBall()
    {
        yield return new WaitForSeconds(1f);
        Projectile.position = myTransform.position; //reset position to ball thrower
        Vector3 speed = calculateBestThrowSpeed(myTransform.position, Target.transform.position, 1.5f); //change timeToTarget for faster ball throw
        Projectile.GetComponent<Rigidbody>().AddForce(speed, ForceMode.VelocityChange); //add force to rigidbody

        goalLineScript.fadeGoalSound();
        goalLineScript.goalEnable = true;

        dbscript.incrementGoalAttempt();

        attempts++;
        attemptText.text = "Attempts: " + attempts;

        kickAudio.Play();

        yield return null;
    }

    private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
    {
        // calculate vectors
        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = toTarget;
        toTargetXZ.y = 0;

        // calculate xz and y
        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        // calculate starting speeds for xz and y. Physics forumulase deltaX = v0 * t + 1/2 * a * t * t
        // where a is "-gravity" but only on the y plane, and a is 0 in xz plane.
        // so xz = v0xz * t => v0xz = xz / t
        // and y = v0y * t - 1/2 * gravity * t * t => v0y * t = y + 1/2 * gravity * t * t => v0y = y / t + 1/2 * gravity * t
        float t = timeToTarget;
        
        float v0y = y / t + 0.5f * Physics.gravity.magnitude * t;
        float v0xz = xz / t;
        
        // create result vector for calculated starting speeds
        Vector3 result = toTargetXZ.normalized;        // get direction of xz but with magnitude 1
        result *= v0xz;                                // set magnitude of xz to v0xz (starting speed in xz plane)
        result.y = v0y;                                // set y to v0y (starting speed of y plane)

        return result;
    }


    //Use for non rigidbody
    IEnumerator SimulateProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            Debug.Log("Sending");
        
        // Short delay added before Projectile is thrown
       

        // Move projectile to the position of throwing object + add some offset if needed.
        Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
      //  float test_distance = 2s ^ 2 * Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) * Mathf.Cos(2 * firingAngle * Mathf.Deg2Rad) / gravity;
        float target_Distance = Vector3.Distance(Projectile.position, Target.transform.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        Projectile.rotation = Quaternion.LookRotation(Target.transform.position - Projectile.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
        }
    }
}