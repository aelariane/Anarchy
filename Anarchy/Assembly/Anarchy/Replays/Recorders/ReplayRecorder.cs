using Anarchy.Configuration;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.Replays.Recorders
{
    public class ReplayRecorder : MonoBehaviour
    {
        private bool dropped = false;
        private Replay lastReplay;
        private int nextReplayFps;
        private float timer;
        private bool wasPlaying = false;
        private bool isRecordingEnabled;

        public static readonly BoolSetting RecordReplays = new BoolSetting(nameof(RecordReplays), false);
        public static readonly IntSetting ReplayFPS = new IntSetting(nameof(ReplayFPS), 60);
        public static readonly IntSetting ReplayTimeLimitSeconds = new IntSetting(nameof(ReplayTimeLimitSeconds), 1200);

        public ReplayAnimationRecorder AnimationRecorder { get; private set; }
        public ReplayWorld World { get; private set; }
        public ReplayFrameRecorder FrameRecorder { get; private set; }
        public static ReplayRecorder Instance { get; private set; }
        public ReplayObjectOperationRecorder OperationRecorder { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        private void OnLevelWasLoaded(int level)
        {
            if (level == 0)
            {
                if (wasPlaying)
                {
                    lastReplay = ComposeReplay();
                }
                wasPlaying = false;
                return;
            }

            if (!dropped && wasPlaying)
            {
                lastReplay = ComposeReplay();
            }
            else
            {
                lastReplay = null;
            }

            wasPlaying = true;

            World = new ReplayWorld();
            FrameRecorder = new ReplayFrameRecorder(World);
            AnimationRecorder = new ReplayAnimationRecorder(World);
            OperationRecorder = new ReplayObjectOperationRecorder(World);

            isRecordingEnabled = RecordReplays.Value;
            nextReplayFps = ReplayFPS.Value;
            timer = 1000f / (float)nextReplayFps;
            dropped = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (lastReplay != null)
                {
                    //lastReplay.SaveAsFile();
                }
            }

            if(Application.loadedLevel == 0)
            {
                return;
            }

            if (FengGameManagerMKII.FGM.logic.RoundTime >= ReplayTimeLimitSeconds.Value)
            {
                dropped = true;
            }

            if (isRecordingEnabled && !dropped)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    FrameRecorder.RecordNextFrame();
                    timer = 1000f / (float)nextReplayFps;
                }
            }
        }

        public Replay ComposeReplay()
        {
            return null;
        }

        public void OnAnimation(GameObject go, AnimationType type, string animation, float time = 0f)
        {
            ReplayGameObject resObject = World.ActiveObjects.FirstOrDefault(x => x.SourceObject == go);
            if(resObject == null)
            {
                return;
            }

            AnimationRecorder.RecordAnimationCall(resObject, type, animation, time);
        }
    }
}