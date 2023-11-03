namespace CommonCleanArch.Helpers;
public static class OtherHelper
{
    public static bool IsNotNullValidIdExt(this EntityBase dto)
        => dto == null || string.IsNullOrWhiteSpace(dto.Id);
}
