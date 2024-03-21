using Godot;
using System;

public partial class AttackInfo : RefCounted
{
    [Export] public float damage;
    [Export]public Vector3 knockback;
    [Export]public float hitstunDuration;
    public CharacterSheet attacker;
    [Export]public Vector3 direction;

    public AttackInfo(float dmg, Vector3 kb, Vector3 dir, float hsD, CharacterSheet att)
    {
        damage = dmg;
        knockback = kb;
        attacker = att;
        hitstunDuration = hsD;
        direction = dir;
    }

    public AttackInfo(float dmg, Vector3 kb, float hsD, CharacterSheet att) : this(dmg, kb, kb.Normalized(), hsD, att) { }

    
}
