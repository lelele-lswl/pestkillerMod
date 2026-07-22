using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace CharMod.CharModCode.Scenes;

/// <summary>
/// 战斗角色形象。死亡时切换到死亡图片。
/// </summary>
public partial class CustomCreatureVisuals : NCreatureVisuals
{
    public const float DeathAnimLength = 0.8f;

    private Sprite2D? _sprite;
    private Texture2D? _aliveTexture;
    private Texture2D? _deathTexture;
    private bool _isDead;

    public override void _Ready()
    {
        base._Ready();

        var visualsNode = GetNode<Node2D>("%Visuals");
        _sprite = visualsNode?.GetNode<Sprite2D>("CharSprite");

        if (_sprite != null)
        {
            _aliveTexture = _sprite.Texture;
        }

        string deathPath = "res://pic/pestkiller_death.png";
        if (ResourceLoader.Exists(deathPath))
        {
            _deathTexture = GD.Load<Texture2D>(deathPath);
        }
    }

    public void PlayAnimation(string trigger)
    {
        // "Dead" 后除非 "Revive" 否则不响应
        if (_isDead && trigger != "Revive") return;

        switch (trigger)
        {
            case "Idle":
            case "Revive":
                if (_sprite != null && _aliveTexture != null)
                    _sprite.Texture = _aliveTexture;
                _isDead = false;
                break;
            case "Dead":
                if (_isDead) return;
                _isDead = true;
                if (_sprite != null && _deathTexture != null)
                {
                    _sprite.Texture = _deathTexture;
                }
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sprite = null;
            _aliveTexture = null;
            _deathTexture = null;
        }
        base.Dispose(disposing);
    }
}
