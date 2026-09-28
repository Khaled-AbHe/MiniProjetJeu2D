/// <summary>
/// Anything the player's attacks can damage (Enemy, Boss, ...).
/// CombatUtils looks for this instead of a concrete Enemy type.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int damageAmount);
}
