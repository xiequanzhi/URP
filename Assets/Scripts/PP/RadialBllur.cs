using System;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Option to control motion blur Mode.
    /// </summary>
    public enum RadialBlurMode
    {
        /// <summary>
        /// Use this if you don't need object motion blur.
        /// </summary>
        CameraOnly,

        /// <summary>
        /// Use this if you need object motion blur.
        /// </summary>
        CameraAndObjects
    }

    /// <summary>
    /// Options to control the quality the motion blur effect.
    /// </summary>
    public enum RadialBlurQuality
    {
        /// <summary>
        /// Use this to select low motion blur quality.
        /// </summary>
        Low,

        /// <summary>
        /// Use this to select medium motion blur quality.
        /// </summary>
        Medium,

        /// <summary>
        /// Use this to select high motion blur quality.
        /// </summary>
        High
    }

    /// <summary>
    /// A volume component that holds settings for the motion blur effect.
    /// </summary>
    [Serializable, VolumeComponentMenuForRenderPipeline("Post-processing/Radial Blur", typeof(UniversalRenderPipeline))]
    // [URPHelpURL("Post-Processing-Motion-Blur")]
    public sealed class RadialBlur : VolumeComponent, IPostProcessComponent
    {
        /// <summary>
        /// The motion blur technique to use. If you don't need object motion blur, CameraOnly will result in better performance.
        /// </summary>
        [Tooltip("The motion blur technique to use. If you don't need object motion blur, CameraOnly will result in better performance.")]
        public RadialBlurModeParameter mode = new RadialBlurModeParameter(RadialBlurMode.CameraOnly);

        /// <summary>
        /// The quality of the effect. Lower presets will result in better performance at the expense of visual quality.
        /// </summary>
        [Tooltip("The quality of the effect. Lower presets will result in better performance at the expense of visual quality.")]
        public RadialBlurQualityParameter quality = new RadialBlurQualityParameter(RadialBlurQuality.Low);

        /// <summary>
        /// Sets the intensity of the motion blur effect. Acts as a multiplier for velocities.
        /// </summary>
        [Tooltip("The strength of the motion blur filter. Acts as a multiplier for velocities.")]
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        /// <summary>
        /// Sets the maximum length, as a fraction of the screen's full resolution, that the velocity resulting from Camera rotation can have.
        /// Lower values will improve performance.
        /// </summary>
        [Tooltip("Sets the maximum length, as a fraction of the screen's full resolution, that the velocity resulting from Camera rotation can have. Lower values will improve performance.")]
        public ClampedFloatParameter clamp = new ClampedFloatParameter(0.05f, 0f, 0.2f);

        /// <inheritdoc/>
        public bool IsActive() => intensity.value > 0f && mode.value == RadialBlurMode.CameraOnly;

        /// <inheritdoc/>
        public bool IsTileCompatible() => false;
    }

    /// <summary>
    /// A <see cref="VolumeParameter"/> that holds a <see cref="RadialBlurMode"/> value.
    /// </summary>
    [Serializable]
    public class RadialBlurModeParameter : VolumeParameter<RadialBlurMode>
    {
        /// <summary>
        /// Creates a new <see cref="RadialBlurModeParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public RadialBlurModeParameter(RadialBlurMode value, bool overrideState = false) : base(value, overrideState) { }
    }

    /// <summary>
    /// A <see cref="VolumeParameter"/> that holds a <see cref="RadialBlurQuality"/> value.
    /// </summary>
    [Serializable]
    public class RadialBlurQualityParameter : VolumeParameter<RadialBlurQuality>
    {
        /// <summary>
        /// Creates a new <see cref="RadialBlurQualityParameter"/> instance.
        /// </summary>
        /// <param name="value">The initial value to store in the parameter.</param>
        /// <param name="overrideState">The initial override state for the parameter.</param>
        public RadialBlurQualityParameter(RadialBlurQuality value, bool overrideState = false) : base(value, overrideState) { }
    }
}
