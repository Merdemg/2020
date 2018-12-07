/* 
*   NatCorder
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    using UnityEngine;

    public sealed class NatCorderNull : INatCorder {

        #region --Properties--
        public bool IsRecording { get { return false; }}
        public long CurrentTimestamp { get { return 0L; }}
        #endregion


        #region --Operations--

        public NatCorderNull () {
            UnityEngine.Debug.Log("NatCorder: NatCorder 1.3 is not supported on this platform");
        }

        public void StartRecording (Container container, VideoFormat videoFormat, AudioFormat audioFormat, RecordingCallback recordingCallback) {}

        public void StopRecording () {}

        public RenderTexture AcquireFrame () {return null;}

        public void CommitFrame (RenderTexture frame, long timestamp) {}

        public void CommitSamples (float[] sampleBuffer, long timestamp) {}
        #endregion
    }
}