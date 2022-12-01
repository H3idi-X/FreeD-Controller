using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace h3idiX
{
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class FreeD_Controller : MonoBehaviour
    {
        const int kFreeDPacketLength = 29;
        public GameObject linkedObject;
        public Camera targetCamera;
        public int LocalPort = 40000;
        static UdpClient udp;
        Thread thread;

        static FreeD_Controller s_freeD_Controller;
        public float minFocalLength = 24.0f;
        public float maxFocalLength = 70.0f;
        public float focalLengthDivider = 4096.0f;
        public bool useFocalLength = true;

        public float minFocusDistance = 0.2f;
        public float maxFocusDistance = 3.0f;
        public float focusDistanceDivider = 4096.0f;
        public bool useFocusDistance = true;

        public bool useYaw = true;
        public float yawDivider = -(32768 * 256.0f);
        public bool usePitch = true;
        public float pitchDivider = (32768 * 256.0f);
        public bool useRoll = true;
        public float rollDivider = (32768 * 256.0f);

        public bool useXpos = false;
        public float posXDivider = (4096.0f);
        public bool useYpos = false;
        public float posYDivider = (4096.0f);
        public bool useZpos = false;
        public float posZDivider = (4096.0f);

        private bool isAlive = false;
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
        }

        public float pitch
        {
            get
            {
                return currentFreeD.pitch;

            }
        }
        public float yaw
        {
            get 
            {
                 return currentFreeD.yaw;
            }
        }
        public float roll
        {
            get 
            { 
                return currentFreeD.roll;
            }
        }

        public float posX
        {
            get
            {
                return currentFreeD.posX;
            }

        }
        public float posY
        {
            get
            { 
                return currentFreeD.posY;
            }
        }
        public float posZ
        {
            get
            {
                return currentFreeD.posZ;
            }

        }

        public float zoom
        {
            get 
            {
                return currentFreeD.zoom;
            }
        }

        public float focus
        {
            get
            {
                return currentFreeD.focus;
            }
        }


        public int vendorID
        {
            get { return currentFreeD.vendorID; }
        }
        public int cameraID
        {
            get { return currentFreeD.cameraID; }
        }

        FreeD currentFreeD = new FreeD();
        static FreeD recievedFreeDData = new FreeD();
        void Awake()
        {
            if ( udp == null)
            {
                udp = new UdpClient(LocalPort);
            }
            s_freeD_Controller = this;
            //        udp.Client.ReceiveTimeout = 1000;
            thread = new Thread(new ThreadStart(ThreadMethod));
            isAlive = true;
            thread.Start();
            targetCamera = GetComponent<Camera>();
        }

        private void OnDestroy()
        {
            isAlive = false;
            if (thread != null)
            {
                thread.Join();
            }

            s_freeD_Controller = null;
        }
        void Start ()
        {


        }

        void Update ()
        {
            if ( s_freeD_Controller != null)
            {
                lock (s_freeD_Controller)
                {
                    currentFreeD = recievedFreeDData;
                }
            }
            
            gameObject.transform.rotation = Quaternion.Euler(yaw /yawDivider, pitch /pitchDivider, roll/rollDivider);
            if (targetCamera != null)
            {
                if ( useFocalLength)
                {
                    targetCamera.focalLength = minFocalLength + ((maxFocalLength - minFocalLength) * (float)currentFreeD.zoom / focalLengthDivider);
                }
                if ( targetCamera.usePhysicalProperties && useFocusDistance)
                {
    #if UNITY_2022_2_OR_NEWER
                    var dst = minFocusDistance + ((maxFocusDistance - minFocusDistance) * ((float)currentFreeD.focus / focusDistanceDivider));
                    targetCamera.focusDistance = dst;
    #endif
                }
            }
            if (linkedObject != null)
            {
                linkedObject.transform.rotation = gameObject.transform.rotation;

            }

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
        static float GetPitch(byte[] byteData)
        {
            return (float)(((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)) );
        }
        static float GetRoll(byte[] byteData)
        {
            return (float)(((byteData[0] << 24) | (byteData[1] << 16) | (byteData[2] << 8)) );
        }

        static int GetZoom(byte[] byteData)
        {

            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));


            return sum;
        }
        static int GetFocusDistance(byte[] byteData)
        {
            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));
            return sum;
        }

        static int GetPosition(byte[] byteData)
        {
            int sum = ((byteData[0] << 16) | (byteData[1] << 8) | (byteData[2]));
            return sum;
        }
        private static void FromByteData(byte[] byteData)
        {
            if (byteData.Length != kFreeDPacketLength)
            {
                return ;
            }
            if (s_freeD_Controller == null)
            {
                return ;
            }
            recievedFreeDData.vendorID = byteData[0];
            recievedFreeDData.cameraID = byteData[1];
            byte[] bytePitch = new byte[3];
            bytePitch[0] = byteData[2];
            bytePitch[1] = byteData[3];
            bytePitch[2] = byteData[4];
            if (s_freeD_Controller.usePitch)
            {
                recievedFreeDData.pitch = GetPitch(bytePitch);
            }

            byte[] byteYaw = new byte[3];
            byteYaw[0] = byteData[5];
            byteYaw[1] = byteData[6];
            byteYaw[2] = byteData[7];
            if (s_freeD_Controller.useYaw)
            {
                recievedFreeDData.yaw = GetYaw(byteYaw);
            }
            byte[] byteRoll = new byte[3];
            byteRoll[0] = byteData[8];
            byteRoll[1] = byteData[9];
            byteRoll[2] = byteData[10];
            if (s_freeD_Controller.useRoll)
            {
                recievedFreeDData.roll = GetRoll(byteRoll);
            }
            byte[] byteZoom = new byte[3];
            byteZoom[0] = byteData[20];
            byteZoom[1] = byteData[21];
            byteZoom[2] = byteData[22];
            if (s_freeD_Controller.useFocalLength)
            {
                recievedFreeDData.zoom = GetZoom(byteZoom);
            }
            byte[] byteFocus = new byte[3];
            byteFocus[0] = byteData[23];
            byteFocus[1] = byteData[24];
            byteFocus[2] = byteData[25];
            if (s_freeD_Controller.useFocusDistance)
            {
                recievedFreeDData.focus = GetFocusDistance(byteFocus);
            }
            byte[] bytePosX = new byte[3];
            bytePosX[0] = byteData[11];
            bytePosX[1] = byteData[12];
            bytePosX[2] = byteData[13];
            if (s_freeD_Controller.useXpos)
            {
                recievedFreeDData.posX = GetPosition(bytePosX);
            }
            byte[] bytePosY = new byte[3];
            bytePosY[0] = byteData[11];
            bytePosY[1] = byteData[12];
            bytePosY[2] = byteData[13];
            if (s_freeD_Controller.useYpos)
            {
                recievedFreeDData.posY = GetPosition(bytePosY);
            }
            byte[] bytePosZ = new byte[3];
            bytePosZ[0] = byteData[11];
            bytePosZ[1] = byteData[12];
            bytePosZ[2] = byteData[13];
            if (s_freeD_Controller.useZpos)
            {
                recievedFreeDData.posZ = GetPosition(bytePosZ);
            }

            
        }
        private static void ThreadMethod()
        {
            Debug.Log("UDP Receive thread started");
            while (s_freeD_Controller != null && s_freeD_Controller.isAlive)
            {
                IPEndPoint remoteEP = null;
                byte[] data = udp.Receive(ref remoteEP);

                if (data.Length == kFreeDPacketLength)
                {
                    lock(s_freeD_Controller)
                    {
                        FromByteData(data);
                    }

                }

            }
        }
    }
}
