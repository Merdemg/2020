/* 
*   NatCorder
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatCorderU.Core.Clocks {

    using UnityEngine;
    using Docs;

    /// <summary>
    /// Realtime clock for generating timestamps
    /// </summary>
    [Doc(@"RealtimeClock")]
    public sealed class RealtimeClock : IClock {

        private long? initialTimestamp; // absolute
        private long lastEventTime;     // absolute
        private long pausedTime;        // delta
        private readonly object timeFence = new object();

        /// <summary>
        /// Current timestamp in nanoseconds.
        /// The very first value reported by this property will always be zero.
        /// Do not use this property if the clock is paused as the reported value will be incorrect until the clock is resumed.
        /// </summary>
        [Doc(@"CurrentTimestamp", @"RealtimeClockCurrentTimestampDiscussion")]
        public long CurrentTimestamp {
            get {
                lock (timeFence) {
                    // Sycnhronize the native call to protect `initialTimestamp` offset
                    var now = NatCorder.Implementation.CurrentTimestamp;
                    initialTimestamp = initialTimestamp ?? now;
                    if (IsPaused)
                        Debug.LogWarning("NatCorder Warning: Realtime clock will report wrong time when clock is paused!");
                    return now - (long)initialTimestamp - pausedTime;
                }
            }
        }

        /// <summary>
        /// Is the clock paused?
        /// </summary>
        [Doc(@"RealtimeClockIsPaused")]
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Pause the clock.
        /// This is useful for pausing and resuming recordings
        /// </summary>
        [Doc(@"RealtimeClockPause")]
        public void Pause () {
            lock (timeFence) {
                if (IsPaused) return; // Can't trust anyone :(
                lastEventTime = NatCorder.Implementation.CurrentTimestamp;
                IsPaused = true;
            }
        }

        /// <summary>
        /// Resume the clock
        /// </summary>
        [Doc(@"RealtimeClockResume")]
        public void Resume () {
            lock (timeFence) {
                if (!IsPaused) return;
                pausedTime += NatCorder.Implementation.CurrentTimestamp - lastEventTime;
                IsPaused = false;
            }
        }
    }
}