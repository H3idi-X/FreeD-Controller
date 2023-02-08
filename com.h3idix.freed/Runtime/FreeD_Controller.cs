using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// 0.1.0
namespace h3idiX
{

    [ExecuteAlways]
    public class FreeD_Controller : MonoBehaviour
    {
        // fields bound to UI.
        [SerializeField] internal LensData lensData;
        [SerializeField] internal Vector3 freeDPosition;
        [SerializeField] internal Vector3 defaultPosition;
        [SerializeField] internal Vector3 freeDRotation;
        [SerializeField] internal int vendorID;
        [SerializeField] internal int cameraID;
        [SerializeField] internal float freeDFocusDistance;
        [SerializeField] internal float freeDFocalLength;

        [SerializeField] internal Vector3Int inputPosition;
        [SerializeField] internal Vector3Int inputRotation;
        [SerializeField] internal int inputFocusDistance;
        [SerializeField] internal int inputFocalLength;
        [SerializeField] internal Vector3 positionOffset;
        [SerializeField] internal Camera targetCamera;
        [SerializeField] internal int localPort = 40000;
        [SerializeField] internal bool positionEnabled = false;
        [SerializeField] internal bool focalLengthEnabled = true;

        [SerializeField] internal bool focusDistanceEnabled = true;
        //

        const bool rotationEnabled = true;
        const int kFreeDPacketLength = 29;


        UdpClient udpClient;
        Thread thread;

        const float yawDivider = -(32768 * 256.0f);
        const float pitchDivider = (32768 * 256.0f);
        const float rollDivider = (32768 * 256.0f);


        const float posXDivider = (16384000.0f);
        const float posYDivider = (16384000.0f);
        const float posZDivider = (16384000.0f);

        bool isAlive = false;
        bool notConnected = false;

        class WorkerClass
        {

            // param 
            private FreeD_Controller m_freeDController;

            public WorkerClass(FreeD_Controller controller)
            {
                m_freeDController = controller;
            }


            public void Worker()
            {
#pragma warning disable 168
                Debug.Log("UDP Receive thread started");
                m_freeDController.notConnected = true;
                while (m_freeDController != null && m_freeDController.isAlive)
                {

                    IPEndPoint remoteEP = null;


                    byte[] data = null;
                    try
                    {
                        data = m_freeDController.udpClient.Receive(ref remoteEP);
                        m_freeDController.notConnected = false;
                    }
                    catch (Exception e)
                    {
                        m_freeDController.notConnected = true;
                        continue;
                    }

                    if (data.Length == kFreeDPacketLength)
                    {
                        lock (m_freeDController)
                        {
                            FromByteData(m_freeDController, data);
                        }

                    }

                }
#pragma warning restore 168
            }
        }

        public bool isConnected
        {
            get { return !notConnected; }
        }

        struct FreeD
        {
            public int vendorID;
            public int cameraID;
            public float pitch;
            public float yaw;
            public float roll;
            public float posZ;
            public float posY;
            public float posX;
            public int zoom;
            public int focus;

            public int inputPitch;
            public int inputYaw;
            public int inputRoll;

            public int inputPosZ;
            public int inputPosY;
            public int inputPosX;
            public int inputZoom;
            public int inputFocus;

        }

        public float pitch
        {
            get { return currentFreeD.pitch; }
        }

        public float yaw
        {
            get { return currentFreeD.yaw; }
        }

        public float roll
        {
            get { return currentFreeD.roll; }
        }

        public float posX
        {
            get { return currentFreeD.posX; }

        }

        public float posY
        {
            get { return currentFreeD.posY; }
        }

        public float posZ
        {
            get { return currentFreeD.posZ; }

        }

        public float zoom
        {
            get { return currentFreeD.zoom; }
        }

        public float focus
        {
            get { return currentFreeD.focus; }
        }




        FreeD currentFreeD = new FreeD();
        FreeD recievedFreeDData = new FreeD();

        void Awake()
        {
            StartThread();

        }

        private void OnDestroy()
        {
            ShutdownThread();
        }

        void Start()
        {

        }

        void Update()
        {


            lock (this)
            {
                currentFreeD = recievedFreeDData;
            }

            vendorID = currentFreeD.vendorID;
            cameraID = currentFreeD.cameraID;
            inputPosition.x = currentFreeD.inputPosX;
            inputPosition.y = currentFreeD.inputPosY;
            inputPosition.z = currentFreeD.inputPosZ;
            inputRotation.x = currentFreeD.inputYaw;
            inputRotation.y = currentFreeD.inputPitch;
            inputRotation.z = currentFreeD.inputRoll;
            inputFocalLength = currentFreeD.inputZoom;
            if (lensData == null)
            {
                return;
            }

            if (lensData.focusDistanceCurve != null)
            {
                freeDFocalLength = lensData.focalLengthCurve.Evaluate(inputFocalLength);
            }

            positionOffset.x = lensData.lensOffsetCurves[0].Evaluate(inputFocalLength);
            positionOffset.y = lensData.lensOffsetCurves[1].Evaluate(inputFocalLength);
            positionOffset.z = lensData.lensOffsetCurves[2].Evaluate(inputFocalLength);
            
            inputFocusDistance = currentFreeD.inputFocus;
            if (lensData.focusDistanceCurve != null)
            {
                freeDFocusDistance = lensData.focusDistanceCurve.Evaluate(inputFocusDistance);
            }


            if (targetCamera != null)
            {
                targetCamera.transform.rotation =
                    Quaternion.Euler(yaw / yawDivider, pitch / pitchDivider, roll / rollDivider);
                freeDRotation = new Vector3(yaw / yawDivider * 180.0f/Mathf.PI, pitch / pitchDivider * 180.0f/Mathf.PI, roll / rollDivider * 180.0f/Mathf.PI);

                if (focalLengthEnabled)
                {
                    targetCamera.focalLength = freeDFocalLength;
                }

                if (targetCamera.usePhysicalProperties && focusDistanceEnabled)
                {
#if UNITY_2022_2_OR_NEWER
                    targetCamera.focusDistance = freeDFocusDistance;
#endif
                }

                if (positionEnabled)
                {
                    var rot = targetCamera.transform.rotation;
                    var matrix = new Matrix4x4();
                    matrix.SetTRS(Vector3.zero, rot, Vector3.one);
                    var offPos =  positionOffset;
                    offPos = matrix * offPos;
                    
                    float localX = posX / posXDivider;
                    float localY = posY / posYDivider;
                    float localZ = posZ / posZDivider;

                    freeDPosition = new Vector3(localX, localY, localZ); // update variables bound to UI
                    float offset = 0.0f;
                    targetCamera.transform.position = new Vector3(localX, localY + offset, localZ) + defaultPosition + offPos;
                }
            }


        }

        internal void UpdatePortNumber()
        {
            ShutdownThread();
            StartThread();
        }

        void StartThread()
        {
            WorkerClass wc = new WorkerClass(this);
            if (udpClient == null)
            {
                udpClient = new UdpClient(localPort);
                udpClient.Client.ReceiveTimeout = 1000;
            }

            //        udp.Client.ReceiveTimeout = 1000;
            thread = new Thread(new ThreadStart(wc.Worker));
            isAlive = true;
            thread.Start();

        }

        void ShutdownThread()
        {
            isAlive = false;
            if (thread != null)
            {
                thread.Join();
            }

            thread = null;

            if (udpClient != null)
                udpClient.Close();
            udpClient = null;
        }
        

        void OnApplicationQuit()
        {
            if ( thread != null)
                thread.Abort();
        }

        static float GetYaw(byte[] byteData)
        {
            return (float)(((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)  ));
        }

        static int GetYawInteger(byte[] byteData)
        {
            return ((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)  );
        }

        static float GetPitch(byte[] byteData)
        {
            return (float)(((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)) );
        }

        static int GetPitchInteger(byte[] byteData)
        {
            return ((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)) ;         
        }
        
        static float GetRoll(byte[] byteData)
        {
            return (float)(((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)) );
        }

        static int GetRollInteger(byte[] byteData)
        {
            return ((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)) ;
        }
        static int GetZoom(byte[] byteData)
        {

            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));


            return sum;
        }
        static int GetZoomInteger(byte[] byteData)
        {

            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));


            return sum;
        }

        static int GetFocusDistance(byte[] byteData)
        {
            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));
            return sum;
        }
        static int GetFocusDistanceInteger(byte[] byteData)
        {
            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));
            return sum;
        }
        
        static float GetPosition(byte[] byteData)
        {
            return (float)(((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)));
        }
        
        static int GetPositionInteger(byte[] byteData)
        {
            return ((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8));
        }
        private static void FromByteData(FreeD_Controller freeDcontroller, byte[] byteData)
        {
            if (byteData.Length != kFreeDPacketLength)
            {
                return ;
            }

            if (CheckSum(byteData) != byteData[28])
            {
                return;
            }
            

            freeDcontroller.recievedFreeDData.vendorID = byteData[0];
            freeDcontroller.recievedFreeDData.cameraID = byteData[1];
            byte[] bytePitch = new byte[3];
            bytePitch[0] = byteData[2];
            bytePitch[1] = byteData[3];
            bytePitch[2] = byteData[4];

            freeDcontroller.recievedFreeDData.pitch = GetPitch(bytePitch);
            freeDcontroller.recievedFreeDData.inputPitch = GetPitchInteger(bytePitch);

            byte[] byteYaw = new byte[3];
            byteYaw[0] = byteData[5];
            byteYaw[1] = byteData[6];
            byteYaw[2] = byteData[7];

            freeDcontroller.recievedFreeDData.yaw = GetYaw(byteYaw);
            freeDcontroller.recievedFreeDData.inputYaw = GetYawInteger(byteYaw);
            
            byte[] byteRoll = new byte[3];
            byteRoll[0] = byteData[8];
            byteRoll[1] = byteData[9];
            byteRoll[2] = byteData[10];
            
            freeDcontroller.recievedFreeDData.roll = GetRoll(byteRoll);
            freeDcontroller.recievedFreeDData.inputRoll = GetRollInteger(byteRoll);
            byte[] byteZoom = new byte[3];
            byteZoom[0] = byteData[20];
            byteZoom[1] = byteData[21];
            byteZoom[2] = byteData[22];

            freeDcontroller.recievedFreeDData.zoom = GetZoom(byteZoom);
            freeDcontroller.recievedFreeDData.inputZoom = GetZoomInteger(byteZoom);

            byte[] byteFocus = new byte[3];
            byteFocus[0] = byteData[23];
            byteFocus[1] = byteData[24];
            byteFocus[2] = byteData[25];

            freeDcontroller.recievedFreeDData.focus = GetFocusDistance(byteFocus);
            freeDcontroller.recievedFreeDData.inputFocus = GetFocusDistanceInteger(byteFocus);


            byte[] bytePosX = new byte[3];
            bytePosX[0] = byteData[11];
            bytePosX[1] = byteData[12];
            bytePosX[2] = byteData[13];
            freeDcontroller.recievedFreeDData.posX = GetPosition(bytePosX);
            freeDcontroller.recievedFreeDData.inputPosX = GetPositionInteger(bytePosX);


            byte[] bytePosY = new byte[3];
            bytePosY[0] = byteData[17];
            bytePosY[1] = byteData[18];
            bytePosY[2] = byteData[19];
            freeDcontroller.recievedFreeDData.posY = GetPosition(bytePosY);
            freeDcontroller.recievedFreeDData.inputPosY = GetPositionInteger(bytePosY);



            byte[] bytePosZ = new byte[3];
            bytePosZ[0] = byteData[14];
            bytePosZ[1] = byteData[15];
            bytePosZ[2] = byteData[16];

            freeDcontroller.recievedFreeDData.posZ = GetPosition(bytePosZ);
            freeDcontroller.recievedFreeDData.inputPosZ = GetPositionInteger(bytePosZ);

        }


        private static int CheckSum(byte[] byteData)
        {
            int sum = 64;
            for (int ii = 0; ii < 28; ii++)
            {
                sum -= byteData[ii];
            }

            int res = sum % 256;
            if (res < 0)
                res += 256;
            return res;
        }
        
    }
}
