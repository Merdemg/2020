/* 
*   NatCorder
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatCorderU.Core.Recorders {

    using UnityEngine;
    using System;
    using Clocks;
    using Docs;

    /// <summary>
    /// Recorder for recording game audio from an audio listener, audio source, or both
    /// </summary>
    [Doc(@"AudioRecorder"), AddComponentMenu(""), DisallowMultipleComponent]
    public sealed class AudioRecorder : MonoBehaviour, IRecorder {
        
        #region --Op vars--
        private bool mute = false;
        private IClock clock;
        #endregion


        #region --Client API--

        /// <summary>
        /// Create an audio recorder for a scene's AudioListener
        /// </summary>
        /// <param name="sceneAudio">Audio listener for the current scene</param>
        /// <param name="clock">Optional. Clock for generating timestamps</param>
        [Doc(@"AudioRecorderCreateListener")]
        public static AudioRecorder Create (AudioListener sceneAudio, IClock clock = null) {
            // Null checking
            if (!sceneAudio) {
                Debug.LogError("NatCorder Error: Cannot create audio recorder for null AudioListener");
                return null;
            }
            var recorder = sceneAudio.gameObject.AddComponent<AudioRecorder>();
            recorder.clock = clock ?? new RealtimeClock();
            return recorder;
        }

        /// <summary>
        /// Create an audio recorder for an audio source
        /// </summary>
        /// <param name="audioSource">Audio source to record</param>
        /// <param name="mute">Optional. Mute audio source after recording so that it is not heard in scene</param>
        /// <param name="clock">Optional. Clock for generating timestamps</param>
        [Doc(@"AudioRecorderCreateSource")]
        public static AudioRecorder Create (AudioSource audioSource, bool mute = false, IClock clock = null) {
            // Null checking
            if (!audioSource) {
                Debug.LogError("NatCorder Error: Cannot create audio recorder for null AudioSource");
                return null;
            }
            var recorder = audioSource.gameObject.AddComponent<AudioRecorder>();
            recorder.mute = mute;
            recorder.clock = clock ?? new RealtimeClock();
            return recorder;
        }

        /// <summary>
        /// Stop recording and teardown resources
        /// </summary>
        [Doc(@"AudioRecorderDispose")]
        public void Dispose () {
            AudioRecorder.Destroy(this);
        }
        #endregion


        #region --Operations--

        private AudioRecorder () {}

        private void OnAudioFilterRead (float[] data, int channels) {
            NatCorder.CommitSamples(data, clock.CurrentTimestamp);
            if (mute) Array.Clear(data, 0, data.Length);
        }
        #endregion
    }
}