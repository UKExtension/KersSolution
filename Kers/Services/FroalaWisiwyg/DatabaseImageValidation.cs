using System;

namespace Kers.Services.FroalaWisiwyg
{
    /// <summary>
    /// Image Validation.
    /// </summary>
    public class DatabaseImageValidation:FileValidation
    {
        /// <summary>
        /// Allowed image validation default extensions.
        /// </summary>
        public static string[] AllowedImageExtsDefault = new string[] { "gif", "jpeg", "jpg", "png", "svg", "blob" };

        /// <summary>
        /// Allowed image validation default mimetypes.
        /// </summary>
        public static string[] AllowedImageMimetypesDefault = new string[] { "image/gif", "image/jpeg", "image/pjpeg", "image/x-png", "image/png", "image/svg+xml" };

        /// <summary>
        /// Init default image validation settings.
        /// </summary>
        protected override void initDefault()
        {
            AllowedExts = AllowedImageExtsDefault;
            AllowedMimeTypes = AllowedImageMimetypesDefault;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseImageValidation(): base()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="allowedExts">Allowed validation image extensions.</param>
        /// <param name="allowedMimeTypes">Allowed validation image mimetypes.</param>
        public DatabaseImageValidation(string[] allowedExts, string[] allowedMimeTypes): base(allowedExts, allowedMimeTypes)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="function">Custom function used for validation.</param>
        public DatabaseImageValidation(Func<string, string, bool> function): base(function)
        {
        }
    }
}
