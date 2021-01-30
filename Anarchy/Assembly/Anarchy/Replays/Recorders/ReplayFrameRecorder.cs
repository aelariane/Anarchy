using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Anarchy.Replays.Recorders
{
    public class ReplayFrameRecorder
    {
        private ReplayWorld replayWorld;
        private List<ReplayFrame> frames = new List<ReplayFrame>();

        public int TotalFramesCount { get; }
        public IEnumerable<ReplayFrame> Frames => frames;

        public ReplayFrameRecorder(ReplayWorld replayWorld)
        {
            this.replayWorld = replayWorld;
        }

        public void RecordNextFrame()
        {
            var nextFrame = new ReplayFrame();

            foreach(var observable in replayWorld.ActiveObjects)
            {
                if (observable.IsObservableObject)
                {
                    nextFrame.RegisterPositionUpdate(new PositionUpdateInformation(
                        observable.Id,
                        observable.Transform.position,
                        observable.Transform.rotation
                    ));
                }
            }

            nextFrame.RegisterCameraPosition(
                Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f),
                IN_GAME_MAIN_CAMERA.BaseT.position,
                IN_GAME_MAIN_CAMERA.BaseT.rotation
            );

            frames.Add(nextFrame);
        }
    }
}
