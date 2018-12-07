/* 
*   NatCorder
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatCorderU.Core.Platforms {

    using UnityEngine;
    using Clocks;

    public interface INatCorder : IClock {

        #region --Properties--
        bool IsRecording { get; }
        #endregion
        
        #region --Operations--
        void StartRecording (Container container, VideoFormat videoFormat, AudioFormat audioFormat, RecordingCallback recordingCallback);
        void StopRecording ();
        RenderTexture AcquireFrame ();
        void CommitFrame (RenderTexture frame, long timestamp);
        void CommitSamples (float[] sampleBuffer, long timestamp);
        #endregion
    }
}