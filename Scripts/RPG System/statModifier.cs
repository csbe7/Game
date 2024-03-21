using Godot;
using System;

public enum Mode
{
    Flat,
    PercentageFromBase,
    Percentage,
}
public partial class statModifier : RefCounted
{
	public readonly float value;
    public readonly Mode mode; 
    public readonly int order;
	public statModifier(float v, Mode m, int o)
    {
        value = v;
        mode = m;
        order = o;
    }

    public statModifier(float v, Mode m) : this(v, m, (int)m * 10) { }
	
}
