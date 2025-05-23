namespace Pngcs.Chunks
{
    /// <summary>
    /// General class for textual chunks.
    /// </summary>
    public abstract class PngChunkTextVar : PngChunkMultiple
    {
        protected internal string _key;
        protected internal string _value;

        protected internal PngChunkTextVar(string id, ImageInfo info)
            : base(id, info)
        {
            _key = string.Empty;
            _value = string.Empty;
        }

        public const string KEY_Title = "Title"; // Short (one line) title or caption for image
        public const string KEY_Author = "Author"; // Name of image's creator
        public const string KEY_Description = "Description"; // Description of image (possibly long)
        public const string KEY_Copyright = "Copyright"; // Copyright notice
        public const string KEY_Creation_Time = "Creation Time"; // Time of original image creation
        public const string KEY_Software = "Software"; // Software used to create the image
        public const string KEY_Disclaimer = "Disclaimer"; // Legal disclaimer
        public const string KEY_Warning = "Warning"; // Warning of nature of content
        public const string KEY_Source = "Source"; // Device used to create the image
        public const string KEY_Comment = "Comment"; // Miscellaneous comment

        public override ChunkOrderingConstraint GetOrderingConstraint()
            => ChunkOrderingConstraint.NONE;

        public string GetKey()
            => _key;

        public string GetValue()
            => _value;

        public void SetKeyValue(string key, string value)
        {
            _key = key;
            _value = value;
        }
    }
}
