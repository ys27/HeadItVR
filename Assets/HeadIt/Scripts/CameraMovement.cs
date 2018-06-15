using System.Collections;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;

namespace VRStandardAssets.Examples
{
    // In the maze scene, the camera rotates around
    // the maze in response to the user swiping.  This
    // class handles how the camera moves in response
    // to the swipe.  This script is placed on a parent
    // of the camera such that the camera pivots around
    // as this gameobject is rotated.

    public class CameraMovement : MonoBehaviour
    {
        // This enum represents the way in which the camera will rotate around the maze.
        public enum OrbitStyle
        {
            Smooth, Step, StepWithFade,
        }

        private GameObject SoundManager;
        private BackgroundSound bgScript;

        private GameObject SystemObject;
        private DatabaseScript dbscript;

        public GameObject ScoreboardObject;

        public ServerScript serverScript;

        [SerializeField]
        private OrbitStyle m_OrbitStyle;
        [SerializeField]
        private float m_RotationIncrement = 45f;           // The amount the camera rotates in response to a swipe.
        [SerializeField]
        private float m_RotationFadeDuration = 0.2f;       // If fading is enabled, this is the duration of the the fade.
        [SerializeField]
        private VRCameraFade m_CameraFade;                 // Optional reference to the camera fade script, only required if fading is enabled.
        [SerializeField]
        private VRInput m_VrInput;                         // Reference to the VRInput to subscribe to swipe events.
        [SerializeField]
        private Rigidbody m_Rigidbody;                     // Reference to the camera's rigidbody.

        private Vector3 moveDirection = Vector3.zero;
        public float gravity = 20.0F;

        private Quaternion m_StartRotation;                                 // The rotation of the camera at the start of the scene, used for reseting.
        private int speed;
        public bool sitting;
        private bool fail;

        //public GameObject seatOptionCanvas;
        //public GameObject removeVRCanvas;
        private Vector3 originalPosition;

        private void Awake()
        {
            // Store the start rotation.
            m_StartRotation = m_Rigidbody.rotation;
            speed = 15;
            sitting = false;
            originalPosition = transform.position;

            SoundManager = GameObject.Find("SoundManager");
            bgScript = SoundManager.GetComponent <BackgroundSound> ();

            SystemObject = GameObject.Find("System");
            dbscript = SystemObject.GetComponent<DatabaseScript>();
        }


        private void OnEnable()
        {
            m_VrInput.OnSwipe += HandleSwipe;
        }


        private void OnDisable()
        {
            m_VrInput.OnSwipe -= HandleSwipe;
        }

        private void HandleSwipe(VRInput.SwipeDirection swipeDirection)
        {
            //			
            // If the game isn't playing or the camera is fading, return and don't handle the swipe.
            if (m_CameraFade.IsFading)
                return;

            if (!sitting)
            { //only able to move when not sitting
              // Otherwise start rotating the camera with either a positive or negative increment.
                switch (swipeDirection)
                {
                    //				case VRInput.SwipeDirection.LEFT:
                    //					//StartCoroutine(RotateCamera(m_RotationIncrement));
                    //					transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
                    //					break;
                    //
                    //				case VRInput.SwipeDirection.RIGHT:
                    //					//StartCoroutine(RotateCamera(-m_RotationIncrement));
                    //					transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
                    //					break;
                    case VRInput.SwipeDirection.UP:
                        //StartCoroutine(RotateCamera(-m_RotationIncrement));
                        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
                        break;
                    case VRInput.SwipeDirection.DOWN:
                        //StartCoroutine(RotateCamera(-m_RotationIncrement));
                        transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
                        break;
                }
            }

            //

        }


        private IEnumerator RotateCamera(float increment)
        {
            // Determine how the camera should rotate base on it's orbit style.
            switch (m_OrbitStyle)
            {
                // If the style is smooth add a torque to the camera's rigidbody.
                case OrbitStyle.Smooth:
                    m_Rigidbody.AddTorque(transform.up * increment);
                    break;

                // If the style is step then rotate the camera's transform by a set amount.
                case OrbitStyle.Step:
                    transform.Rotate(0, increment, 0);
                    break;

                // If the style is step with a fade, wait for the camera to fade out, then step the rotation around, the wait for the camera to fade in.
                case OrbitStyle.StepWithFade:
                    yield return StartCoroutine(m_CameraFade.BeginFadeOut(m_RotationFadeDuration, false));
                    transform.Rotate(0, increment, 0);
                    yield return StartCoroutine(m_CameraFade.BeginFadeIn(m_RotationFadeDuration, false));
                    break;
            }
        }

        public IEnumerator GameStart()
        {
            //yield return StartCoroutine(m_CameraFade.BeginFadeOut(m_RotationFadeDuration, false));
            transform.parent.position = new Vector3(8.994f, 0.14f, 0);
            StartCoroutine(RotateCamera(90));

            ScoreboardObject.transform.position = new Vector3(1.22f, 6.19f, -17.32f);
            ScoreboardObject.transform.eulerAngles = new Vector3(0, 190.0f, 0);

            bgScript.stopBGM();


            serverScript.gameStarted = true;

            dbscript.startTimer();

            yield return null;

            //yield return StartCoroutine(m_CameraFade.BeginFadeIn(m_RotationFadeDuration, false));
        }


        public void Restart()
        {
            // To restart, make sure the rotation is reset and the camera is not moving or rotating.
            m_Rigidbody.rotation = m_StartRotation;
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
        }

        //sit down function
        public void sitDown(Vector3 this_position, string seatNumber)
        {
            sitting = true; //disable swipe movement and seat selection
                            //transform.position = new Vector3(this_position.x, this_position.y + 0.35f, this_position.z - 0.125f); //move the camera to the sitting position
            transform.position = new Vector3(this_position.x, this_position.y + 0.35f, this_position.z - 0.35f); //demo vector
            StartCoroutine(RotateCamera(180)); //face front
            showSeatOptions(this_position, seatNumber);
        }

        //stand up function
        public void standUp()
        {
            sitting = false; //enable swipe movement
            StartCoroutine(RotateCamera(180)); //face seats
            Vector3 this_position = transform.position;
            transform.position = new Vector3(originalPosition.x, originalPosition.y, this_position.z - 0.8f);
            DisableSeatOption(); //hide the option ui
        }

        //show seat option UI
        private void showSeatOptions(Vector3 new_position, string seatNumber)
        {
            //seatOptionCanvas.SetActive(true);
            ////cancelCanvas.gameObject.SetActive (true);
            //seatOptionCanvas.transform.position = new Vector3(new_position.x, new_position.y + 0.15f, new_position.z - 1.3f); //set position of option
            //Text temp = seatOptionCanvas.GetComponentInChildren<Text>();
            //temp.text = seatNumber + "\n $329";
            ////cancelCanvas.transform.position= new Vector3(this_position.x + 0.5f,this_position.y + 1.0f, this_position.z); //set position of option disappear
        }

        //hide seat option UI
        private void DisableSeatOption()
        {
            //seatOptionCanvas.SetActive(false);
            //cancelCanvas.gameObject.SetActive (false);
        }

        //Open native seat selection app 
        public void openNative()
        {
            //removeVRCanvas.transform.position = seatOptionCanvas.transform.position;
            //DisableSeatOption();

            //			string seatInfoText = seatOptionCanvas.GetComponentInChildren<Text>().text;
            //			seatInfoText = seatInfoText.Substring (0, 5);
            //			//Debug.Log (seatInfoText);
            //			//Send Text to native app with intent
            //			AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
            //			string bundleId = "com.ibm.seatselectionnative";
            //			AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //			AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
            //			AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");
            //
            //			AndroidJavaObject launchIntent = null;
            //			try
            //			{
            //				launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage",bundleId);
            //				launchIntent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");
            //				launchIntent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), seatInfoText);
            //			}
            //			catch (System.Exception e)
            //			{
            //				fail = true;
            //				Debug.Log (e);
            //			}
            //
            //			if (fail) { //open app in store
            //				//Application.OpenURL("https://google.com");
            //				Debug.Log ("Failed to open app");
            //			} else {//open the app
            //				removeVRCanvas.transform.position = seatOptionCanvas.transform.position;
            //				DisableSeatOption ();
            //				ca.Call("startActivity",launchIntent);
            //			}
            //				
        }
    }
}