public class PlayerHealth : Health
{
    UIHud hud;
    void Awake() {
        hud = FindObjectOfType<UIHud>();
    }
    void Start() {
        hud.UpdateHealth(HP, MaxHP);
    }

    public override void Damage(float delta) {
        base.Damage(delta);
        hud.UpdateHealth(HP, MaxHP);
    }
    public override void Heal(float delta) {
        base.Heal(delta);
        hud.UpdateHealth(HP, MaxHP);
    }
}
