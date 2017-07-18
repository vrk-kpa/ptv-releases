namespace PTV.Framework.Enums
{
    public enum AccessRightEnum
    {
        UiAppRead,
        UiAppWrite,
        OpenApiRead,
        OpenApiWrite
    }

    public class AccessRightWrapper
    {
        public AccessRightEnum AccessRight { get; set; }

        public AccessRightWrapper(AccessRightEnum accessRight)
        {
            this.AccessRight = accessRight;
        }
    }
}