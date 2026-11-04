namespace ProcessAutomation.Common.DTO.Enum
{
    public enum ValueOptionEnum
    {
        None = 0x0,
        ValueInString = 0x1,
        ValueInLiteralString = 0x2,
        ValueInJson = 0x4,
        ValueInKeyBracket = 0x8,
        ValueInSquareBracket = 0x10,
        ValueInEncapsulated = 0x20
    }
}
