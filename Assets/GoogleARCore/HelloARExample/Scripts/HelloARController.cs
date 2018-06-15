//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.HelloAR
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using GoogleARCore;
    using UnityEngine.UI;

    /// <summary>
    /// Controlls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        public Text m_camPoseText;

        public GameObject m_CameraParent;

        public float m_XZScaleFactor = 10;

        public float m_YScaleFactor = 2;

        public bool m_showPoseData = true;

        private bool trackingStarted = false;

        private Vector3 m_prevARPosePosition;

        private List<TrackedPlane> m_newPlanes = new List<TrackedPlane>();

        private List<TrackedPlane> m_allPlanes = new List<TrackedPlane>();

        private Color[] m_planeColors = new Color[] {
            new Color(1.0f, 1.0f, 1.0f),
            new Color(0.956f, 0.262f, 0.211f),
            new Color(0.913f, 0.117f, 0.388f),
            new Color(0.611f, 0.152f, 0.654f),
            new Color(0.403f, 0.227f, 0.717f),
            new Color(0.247f, 0.317f, 0.709f),
            new Color(0.129f, 0.588f, 0.952f),
            new Color(0.011f, 0.662f, 0.956f),
            new Color(0f, 0.737f, 0.831f),
            new Color(0f, 0.588f, 0.533f),
            new Color(0.298f, 0.686f, 0.313f),
            new Color(0.545f, 0.764f, 0.290f),
            new Color(0.803f, 0.862f, 0.223f),
            new Color(1.0f, 0.921f, 0.231f),
            new Color(1.0f, 0.756f, 0.027f)
        };

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update ()
        {
            _QuitOnConnectionErrors();

            if (Frame.TrackingState != FrameTrackingState.Tracking)
            {

                trackingStarted = false;  // if tracking lost or not initialized

                //m_camPoseText.text = "Lost tracking, wait ...";

                const int LOST_TRACKING_SLEEP_TIMEOUT = 15;

                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;

                return;
            }

            else
            {

               // m_camPoseText.text = "";
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Vector3 currentARPosition = Frame.Pose.position;

            if (!trackingStarted)
            {

                trackingStarted = true;

                m_prevARPosePosition = Frame.Pose.position;
            }

            //Remember the previous position so we can apply deltas

            Vector3 deltaPosition = currentARPosition - m_prevARPosePosition;

            m_prevARPosePosition = currentARPosition;

            if (m_CameraParent != null)
            {

                Vector3 scaledTranslation = new Vector3(m_XZScaleFactor * deltaPosition.x, m_YScaleFactor * deltaPosition.y, m_XZScaleFactor * deltaPosition.z);

                m_CameraParent.transform.Translate(scaledTranslation);

                if (m_showPoseData)
                {

                   // m_camPoseText.text = "Pose = " + currentARPosition + "\n" + GetComponent<FPSARCoreScript>().FPSstring + "\n" + m_CameraParent.transform.position;
                }
            }
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            // Do not update if ARCore is not tracking.
            if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
            {
                _ShowAndroidToastMessage("This device does not support ARCore.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                Application.Quit();
            }
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        /// <param name="length">Toast message time length.</param>
        private static void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
