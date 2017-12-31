

namespace Kers.Services.FroalaWisiwyg
{
    /// <summary>
    /// Image Options used for uploading.
    /// </summary>
    public class DatabaseImageOptions:DatabaseFileOptions
    {
        /// <summary>
        /// Parameter for resizing an image.
        /// </summary>

        /// <summary>
        /// Init default image upload settings.
        /// </summary>
        protected override void initDefault()
        {
            Validation = new ImageValidation();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseImageOptions(): base() { }
    }
}
