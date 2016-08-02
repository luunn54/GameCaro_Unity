using System;

public enum StartupType {
	NORMAL,
	PREFAB
}

public class StartupAttribute : Attribute
{
	public StartupType Type { get; set; }
	public Type ParentType { get; set; }

	public StartupAttribute (StartupType type = StartupType.NORMAL)
	{
		Type = type;
		ParentType = typeof(GM);
	}

	public StartupAttribute (StartupType type, Type parentType)
	{
		Type = type;
		ParentType = parentType;
	}
}