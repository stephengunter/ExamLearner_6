namespace ApplicationCore.Helpers;
public static class PhotoesHelpers
{
	public static ImageResizeType ToImageResizeType(this string val)
	{
		try
		{
			var type = val.ToEnum<ImageResizeType>();
			return type;
		}
		catch (Exception)
		{
			return ImageResizeType.Unknown;
		}
	}
}
