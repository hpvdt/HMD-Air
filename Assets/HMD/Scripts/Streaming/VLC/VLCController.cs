using System;
using HMD.Scripts.Util;
using HMDCommons.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HMD.Scripts.Streaming.VLC
{
    ///This script controls all the GUI for the VLC Unity Canvas Example
    ///It sets up event handlers and updates the GUI every frame
    ///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
    public class VLCController : ControllerLike
    {
        public VLCScreen screen;

        //GUI Elements
        //public RawImage screen;
        //public AspectRatioFitter screenAspectRatioFitter;
        [Required] public Button rewind10Button;
        [Required] public Button ffw10Button;

        [Required] public Slider seekBar;

        [Required] public Button fileButton;

        [Required] public InputField pathInputField; // TODO: this won't be on the dashUI, will be moved to HUD
        [Required] public Button pathEnterButton;

        [Required] public Slider volumeBar;
        [Required] public int maxVolume = 100;
        //The highest volume the slider can reach. 100 is usually good but you can go higher.

        [Required] public Text currentTimeCode;

        [Required] public Slider aspectRatioBar;
        [Required] public GameObject aspectRatioText;

        private bool _isDraggingAspectRatioBar; // TODO: cleanup, onValueChange is totally good enough

        //Configurable Options

        //State variables
        private volatile bool _isPlaying;
        //We use VLC events to track whether we are playing, rather than relying on IsPlaying 

        private volatile bool _isDraggingSeekBar; //We advance the seek bar every frame, unless the user is dragging it

        ///Unity wants to do everything on the main thread, but VLC events use their own thread.
        ///These variables can be set to true in a VLC event handler indicate that a function should be called next Update.
        ///This is not actually thread safe and should be gone soon!
        // private bool _shouldUpdateTracks = false; //Set this to true and the Tracks menu will regenerate next frame
        // private bool _shouldClearTracks = false; //Set this to true and the Tracks menu will clear next frame

        // private List<Button> _videoTracksButtons = new List<Button>();
        // private List<Button> _audioTracksButtons = new List<Button>();
        // private List<Button> _textTracksButtons = new List<Button>();
        private void Awake()
        {
            screen.controller = this;
            // dashPanels.controller = this;
        }

        private void Start()
        {
            screen.Stop();

            Init();
        }

        public void Init()
        {
            if (screen?.feed.Player is null)
            {
                Debug.LogError("VLC Player mediaPlayer not found");
                return;
            }

            var player = screen.feed.Player;

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
        }

        public override void BindUI()
        {
            //Buttons
            rewind10Button.onClick.Rebind(() =>
            {
                Debug.Log("Rewind10Button");
                screen.feed.SeekBack10();
            });
            ffw10Button.onClick.Rebind(() =>
            {
                Debug.Log("FFW10Button");
                screen.feed.SeekForward10();
            });

            var updater = new AspectRatioUpdater(screen);
            updater.SyncAll();

            playButton.onClick.Rebind(() =>
            {
                screen.Play();

                updater.SyncAll();
            });
            pauseButton.onClick.Rebind(() => { screen.Pause(); });
            stopButton.onClick.Rebind(() => { screen.Stop(); });

            fileButton.onClick.Rebind(() => { screen.PromptUserFilePicker(); });

            // TODO: the following drag & drop definition with EventTrigger are too complex
            //   should use onValueChanged only or something similar

            //Seek Bar Events
            {
                var events = seekBar.GetComponent<EventTrigger>();
                events.triggers.Clear();

                var pointerDown = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };
                pointerDown.callback.Rebind((_) => { _isDraggingSeekBar = true; });
                events.triggers.Add(pointerDown);

                var pointerUp = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerUp
                };
                pointerUp.callback.Rebind((_) =>
                {
                    _isDraggingSeekBar = false;
                    screen.feed.SetTime((long)((double)screen.feed.Duration * seekBar.value));
                });
                events.triggers.Add(pointerUp);
            }

            // Aspect Ratio Bar Events
            {
                var events = aspectRatioBar.GetComponent<EventTrigger>();
                events.triggers.Clear();

                // TODO: the following drag & drop with EventTrigger should have a shared class
                var pointerDown = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };
                pointerDown.callback.Rebind((_) => { _isDraggingAspectRatioBar = true; });
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

                    screen.AspectRatio = frac;

                    // var updater = new AspectRatioUpdater(screen);
                    // updater.SyncText();
                }

                pointerUp.callback.Rebind((_) =>
                {
                    if (_isDraggingAspectRatioBar) SyncV(aspectRatioBar.value);
                    _isDraggingAspectRatioBar = false;
                });
                events.triggers.Add(pointerUp);
                aspectRatioBar.onValueChanged.Rebind(SyncV);
            }

            //Volume Bar
            volumeBar.wholeNumbers = true;
            volumeBar.maxValue = maxVolume; //You can go higher than 100 but you risk audio clipping
            volumeBar.value = screen.feed.Volume;
            volumeBar.onValueChanged.Rebind((_) => { screen.feed.SetVolume((int)volumeBar.value); });

            // screen
            screen.BindUI();
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
            // Get the current playback time as a TimeSpan object
            var currentTime = screen.feed.Time;
            var currentTimeSpan = TimeSpan.FromMilliseconds(currentTime);

            // Format the TimeSpan object as a string in the desired format
            var timecode = currentTimeSpan.ToString(@"hh\:mm\:ss");

            currentTimeCode.text = timecode;

            if (!_isDraggingSeekBar)
            {
                var duration = screen.feed.Duration;
                if (duration > 0)
                    seekBar.value = (float)((double)screen.feed.Time / duration);
            }
        }
    }
}