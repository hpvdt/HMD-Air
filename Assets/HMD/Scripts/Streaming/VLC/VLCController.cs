namespace HMD.Scripts.Streaming.VLC
{
    using System;
    using Util;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UI;
    
    ///This script controls all the GUI for the VLC Unity Canvas Example
    ///It sets up event handlers and updates the GUI every frame
    ///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
    public class VlcController : MonoBehaviour
    {
        public VlcDisplay display;
        public DashPanels dashPanels;

        //GUI Elements
        //public RawImage screen;
        //public AspectRatioFitter screenAspectRatioFitter;
        public Button rewind10Button;
        public Button ffw10Button;

        public Slider seekBar;

        public Button playButton;
        public Button pauseButton;
        public Button stopButton;
        public Button fileButton;
        
        public Button consoleButton;
        public GameObject consoleGroup;
        public InputField pathInputField; // TODO: this won't be on the dashUI, will be moved to HUD
        public Button pathEnterButton;
        
        public Button trackButton;
        public GameObject trackGroup;
        
        public Button volumeButton;
        public GameObject volumeGroup;
        public Slider volumeBar;
        
        public Text currentTimecode;

        public Slider aspectRatioBar;
        public GameObject aspectRatioText;

        private bool _isDraggingAspectRatioBar = false;

        //Configurable Options
        public int maxVolume = 100; //The highest volume the slider can reach. 100 is usually good but you can go higher.

        //State variables
        private volatile bool _isPlaying = false; //We use VLC events to track whether we are playing, rather than relying on IsPlaying 

        private volatile bool _isDraggingSeekBar = false; //We advance the seek bar every frame, unless the user is dragging it

        ///Unity wants to do everything on the main thread, but VLC events use their own thread.
        ///These variables can be set to true in a VLC event handler indicate that a function should be called next Update.
        ///This is not actually thread safe and should be gone soon!
        // private bool _shouldUpdateTracks = false; //Set this to true and the Tracks menu will regenerate next frame
        // private bool _shouldClearTracks = false; //Set this to true and the Tracks menu will clear next frame

        // private List<Button> _videoTracksButtons = new List<Button>();
        // private List<Button> _audioTracksButtons = new List<Button>();
        // private List<Button> _textTracksButtons = new List<Button>();

        private void Start()
        {
            display.controller = this;
            dashPanels.controller = this;

            BindUI();

            display.Stop();
        }

        private void BindUI()
        {
            if (display?.vlcFeed.Player is null)
            {
                Debug.LogError("VLC Player mediaPlayer not found");
                return;
            }

            var player = display.vlcFeed.Player;
            
            //VLC Event Handlers
            player.Playing += (_, _) =>
            {
                //Always use Try/Catch for VLC Events
                try
                {
                    //Because many Unity functions can only be used on the main thread, they will fail in VLC event handlers
                    //A simple way around this is to set flag variables which cause functions to be called on the next Update
                    _isPlaying = true; //Switch to the Pause button next update
                    // _shouldUpdateTracks = true; //Regenerate tracks next update
                }
                catch (Exception ex)
                {
                    Debug.LogError("Exception caught in mediaPlayer.Play: \n" + ex);
                }
            };

            player.Paused += (_, _) =>
            {
                //Always use Try/Catch for VLC Events
                try
                {
                    _isPlaying = false; //Switch to the Play button next update
                }
                catch (Exception ex)
                {
                    Debug.LogError("Exception caught in mediaPlayer.Paused: \n" + ex);
                }
            };

            player.Stopped += (_, _) =>
            {
                //Always use Try/Catch for VLC Events
                try
                {
                    _isPlaying = false; //Switch to the Play button next update
                    // _shouldClearTracks = true; //Clear tracks next update
                }
                catch (Exception ex)
                {
                    Debug.LogError("Exception caught in mediaPlayer.Stopped: \n" + ex);
                }
            };

            //Buttons
            rewind10Button.onClick.AddListener(() =>
            {
                Debug.Log("Rewind10Button");
                display.vlcFeed.SeekBack10();
            });
            ffw10Button.onClick.AddListener(() =>
            {
                Debug.Log("FFW10Button");
                display.vlcFeed.SeekForward10();
            });
            pauseButton.onClick.AddListener(() => { display.Pause(); });
            playButton.onClick.AddListener(() => { display.Play(); });
            stopButton.onClick.AddListener(() => { display.Stop(); });
            
            consoleButton.onClick.AddListener(() =>
            {
                if (ToggleElement(consoleGroup))
                    pathInputField.Select();
            });
            fileButton.onClick.AddListener(() => { display.PromptUserFilePicker(); });
            
            trackButton.onClick.AddListener(() =>
            {
                ToggleElement(trackGroup);
                // SetupTrackButtons();
            });
            volumeButton.onClick.AddListener(() => { ToggleElement(volumeGroup.gameObject); });
            pathEnterButton.onClick.AddListener(() =>
            {
                ToggleElement(consoleGroup);
                display.vlcFeed.Open(pathInputField.text);
            });

            //Seek Bar Events
            var seekBarEvents = seekBar.GetComponent<EventTrigger>();

            var seekBarPointerDown = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };
            seekBarPointerDown.callback.AddListener((_) => { _isDraggingSeekBar = true; });
            seekBarEvents.triggers.Add(seekBarPointerDown);

            var seekBarPointerUp = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerUp
                };
            seekBarPointerUp.callback.AddListener((_) =>
            {
                _isDraggingSeekBar = false;
                display.vlcFeed.SetTime((long)((double)display.vlcFeed.Duration * seekBar.value));
            });
            seekBarEvents.triggers.Add(seekBarPointerUp);
            
            // Aspect Ratio Bar Events
            {
                var events = aspectRatioBar.GetComponent<EventTrigger>();

                // TODO: the following drag & drop with EventTrigger should have a shared class
                var pointerDown = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };
                pointerDown.callback.AddListener((_) => { _isDraggingAspectRatioBar = true; });
                events.triggers.Add(pointerDown);

                var pointerUp = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerUp
                };

                void SyncV(float fromSlider)
                {
                    var ln = fromSlider;
                    // var arDecimal = Mathf.Round(fromSlider * 100f) / 100f;

                    // Get the aspect ratio fraction from the decimal
                    var frac = Frac.FromLn(ln);

                    display.AspectRatio = frac;

                    // var updater = new AspectRatioUpdater(mainDisplay);
                    // updater.SyncText();
                }

                pointerUp.callback.AddListener((_) =>
                {
                    if (_isDraggingAspectRatioBar) SyncV(aspectRatioBar.value);
                    _isDraggingAspectRatioBar = false;
                });
                events.triggers.Add(pointerUp);
                aspectRatioBar.onValueChanged.AddListener(SyncV);
            }

            //Volume Bar
            volumeBar.wholeNumbers = true;
            volumeBar.maxValue = maxVolume; //You can go higher than 100 but you risk audio clipping
            volumeBar.value = display.vlcFeed.Volume;
            volumeBar.onValueChanged.AddListener((_) => { display.vlcFeed.SetVolume((int)volumeBar.value); });
        }


        private void Update()
        {
            //Update screen aspect ratio. Doing this every frame is probably more than is necessary.

            //if(vlcPlayer.texture != null)
            //	screenAspectRatioFitter.aspectRatio = (float)vlcPlayer.texture.width / (float)vlcPlayer.texture.height;

            UpdatePlayPauseButton(_isPlaying);

            UpdateSeekBar();
        }

        //Show the Pause button if we are playing, or the Play button if we are paused or stopped
        private void UpdatePlayPauseButton(bool playing)
        {
            pauseButton.gameObject.SetActive(playing);
            playButton.gameObject.SetActive(!playing);
        }

        //Update the position of the Seek slider to the match the VLC Player
        private void UpdateSeekBar()
        {
            var mm = display;
            // Get the current playback time as a TimeSpan object
            var currentTime = display.vlcFeed.Time;
            var currentTimeSpan = TimeSpan.FromMilliseconds(currentTime);

            // Format the TimeSpan object as a string in the desired format
            var timecode = currentTimeSpan.ToString(@"hh\:mm\:ss");

            currentTimecode.text = timecode;

            if (!_isDraggingSeekBar)
            {
                var duration = display.vlcFeed.Duration;
                if (duration > 0)
                    seekBar.value = (float)((double)display.vlcFeed.Time / duration);
            }
        }

        //Enable a GameObject if it is disabled, or disable it if it is enabled
        private bool ToggleElement(GameObject element)
        {
            var toggled = !element.activeInHierarchy;
            element.SetActive(toggled);
            return toggled;
        }
    }
}
