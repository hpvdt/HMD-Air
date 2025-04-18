namespace DataDisplay.Scripts
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class HudLiteScript : MonoBehaviour
    {
        public static HudLiteScript current;


        //Config Variables
        public bool isActive = false;

        public Transform aircraft;
        public Rigidbody aircraftRB;
        //

        //Hud Display Variables
        public string activeMsg = "HUD Activated";

        public RectTransform hudPanel;

        public bool useRoll = true;
        public float rollAmplitude = 1, rollOffSet = 0, rollFilterFactor = 0.25f;
        public RectTransform horizonRoll;
        public Text horizonRollTxt;

        public bool usePitch = true;
        public float pitchAmplitude = 1,
            pitchOffSet = 0,
            pitchXOffSet = 0,
            pitchYOffSet = 0,
            pitchFilterFactor = 0.125f;
        public RectTransform horizonPitch;
        public Text horizonPitchTxt;

        public bool useHeading = true;
        public float headingAmplitude = 1, headingOffSet = 0, headingFilterFactor = 0.1f;
        public RectTransform compassHSI;
        public Text headingTxt;


        public bool useAltitude = true;
        public float altitudeAmplitude = 1, altitudeOffSet = 0, altitudeFilterFactor = 0.5f;
        public Text altitudeTxt;

        public bool useSpeed = true;
        public float speedAmplitude = 1, speedOffSet = 0, speedFilterFactor = 0.25f;
        public Text speedTxt;

        public TMP_Text energyTxt;
        //


        //All Flight Variables
        public float speed, altitude, pitch, roll, heading;


        //Internal Calculation Variables
        private Vector3 currentPosition, lastPosition, relativeSpeed, lastSpeed;


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// Inicialization
        private void Awake()
        {
            if (aircraft == null && aircraftRB != null) aircraft = aircraftRB.transform;
        }

        public void toggleHud()
        {
            hudPanel.gameObject.SetActive(!hudPanel.gameObject.activeSelf);

            if (hudPanel.gameObject.activeSelf)
            {
                current = this;
                if (aircraft == null && aircraftRB != null) aircraft = aircraftRB.transform;

                isActive = true;
            }
            else
            {
                isActive = false;
                current = null;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// Inicialization



        /////////////////////////////////////////////////////// Updates and Calculations
        private void Update()
        {
            // Return if not active
            if (!isActive || !hudPanel.gameObject.activeSelf) return;
            if (aircraft == null)
            {
                isActive = false;
                return;
            }

            //////////////////////////////////////////// Frame Calculations
            lastPosition = currentPosition;
            lastSpeed = relativeSpeed;

            if (aircraft != null && aircraftRB == null) //Mode Transform
            {
                currentPosition = aircraft.transform.position;
                relativeSpeed =
                    aircraft.transform.InverseTransformDirection((currentPosition - lastPosition) / Time.deltaTime);
            }
            else if (aircraft != null && aircraftRB != null) //Mode RB
            {
                currentPosition = aircraftRB.transform.position;
                relativeSpeed = aircraftRB.transform.InverseTransformDirection(aircraftRB.velocity);
            }
            else
            {
                currentPosition = Vector3.zero;
                relativeSpeed = Vector3.zero;
            }
            //////////////////////////////////////////// Frame Calculations


            //////////////////////////////////////////// Compass, Heading and/or HSI
            if (useHeading)
            {
                heading = Mathf.LerpAngle(heading, aircraft.eulerAngles.y + headingOffSet, headingFilterFactor) % 360f;

                //Send values to Gui and Instruments
                if (compassHSI != null) compassHSI.localRotation = Quaternion.Euler(0, 0, headingAmplitude * heading);
                //if (compassBar != null) compassBar.heading = heading;
                if (headingTxt != null) { headingTxt.text = SerialReader.heading.ToString("000"); }
                //if (headingTxt != null) { if (heading < 0) headingTxt.text = (heading + 360f).ToString("000"); else headingTxt.text = heading.ToString("000"); }

            }
            //////////////////////////////////////////// Compass, Heading and/or HSI


            //////////////////////////////////////////// Roll
            if (useRoll)
            {
                roll = Mathf.LerpAngle(roll, aircraft.rotation.eulerAngles.z + rollOffSet, rollFilterFactor) % 360;

                //Send values to Gui and Instruments
                if (horizonRoll != null) horizonRoll.localRotation = Quaternion.Euler(0, 0, rollAmplitude * roll);
                if (horizonRollTxt != null)
                {
                    //horizonRollTxt.text = roll.ToString("##");
                    if (roll > 180) horizonRollTxt.text = (roll - 360).ToString("00");
                    else if (roll < -180) horizonRollTxt.text = (roll + 360).ToString("00");
                    else horizonRollTxt.text = roll.ToString("00");
                }
                //
            }
            //////////////////////////////////////////// Roll


            //////////////////////////////////////////// Pitch
            if (usePitch)
            {
                pitch = Mathf.LerpAngle(pitch, -aircraft.eulerAngles.x + pitchOffSet, pitchFilterFactor);

                //Send values to Gui and Instruments
                if (horizonPitch != null)
                    horizonPitch.localPosition = new Vector3(
                        -pitchAmplitude * pitch * Mathf.Sin(horizonPitch.transform.localEulerAngles.z * Mathf.Deg2Rad)
                        + pitchXOffSet,
                        pitchAmplitude * pitch * Mathf.Cos(horizonPitch.transform.localEulerAngles.z * Mathf.Deg2Rad)
                        + pitchYOffSet, 0);
                if (horizonPitchTxt != null) horizonPitchTxt.text = pitch.ToString("0");
            }
            //////////////////////////////////////////// Pitch


            //////////////////////////////////////////// Altitude
            if (useAltitude)
            {
                //Original Code
                //altitude = Mathf.Lerp(altitude, altitudeOffSet + altitudeAmplitude * currentPosition.y, speedFilterFactor);

                altitude = SerialReader.Altimeter;

                //Send values to Gui and Instruments
                if (altitudeTxt != null) altitudeTxt.text = altitude.ToString("0").PadLeft(5);
            }
            //////////////////////////////////////////// Altitude


            //////////////////////////////////////////// Speed
            if (useSpeed)
            {
                speed = Mathf.Lerp(speed, speedOffSet + speedAmplitude * relativeSpeed.z, speedFilterFactor);

                speed = SerialReader.Airspeed;

                //Send values to Gui and Instruments
                if (speedTxt != null) speedTxt.text = speed.ToString("0").PadLeft(5); //.ToString("##0");
            }
            //////////////////////////////////////////// Speed


        }
        /////////////////////////////////////////////////////// Updates and Calculations

    }
}
