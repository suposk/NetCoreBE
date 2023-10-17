namespace CommonBE.Helpers;
public static class OtherHelper
{
	public static double ConvertBytesToMegabytesExt(this long bytes) => (bytes / 1024f) / 1024f;	
}
