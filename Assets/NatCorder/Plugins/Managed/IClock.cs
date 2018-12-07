/* 
*   NatCorder
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatCorderU.Core.Clocks {

    /// <summary>
    /// Clock for generating recording timestamps.
    /// The spacing between timestamps determines the actual framerate of the recording.
    /// Clocks are also important for synchronizing audio and video tracks when recording with audio.
    /// Clocks are thread-safe, so they can be used on multiple threads concurrently.
    /// </summary>
    public interface IClock {
        /// <summary>
        /// Current timestamp in nanoseconds.
        /// The very first value reported by this property will always be zero.
        /// </summary>
        long CurrentTimestamp { get; }
    }
}