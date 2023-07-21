namespace Cleanception.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SupportDeepSearch : Attribute
{

}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnSupportDeepSearch : Attribute
{

}