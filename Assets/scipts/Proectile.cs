using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Настройки")]
    public float speed = 15f;
    public GameObject explosionEffect;
    
    private Vector3 targetPosition;
    private float damage;
    private float explosionRadius;
    private CharacterUnit caster;
    
    public void Initialize(CharacterUnit caster, Vector3 target, float dmg, float radius)
    {
        this.caster = caster;
        this.targetPosition = target;
        this.damage = dmg;
        this.explosionRadius = radius;
        
        // Поворачиваем в сторону цели
        transform.LookAt(targetPosition);
        
        // Автоуничтожение через время
        Destroy(gameObject, 5f);
    }
    
    void Update()
    {
        // Движение к цели
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        
        // Проверка достижения цели
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Explode();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Ground"))
        {
            Explode();
        }
    }
    
    void Explode()
    {
        // Создаем эффект взрыва
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        // Наносим урон врагам в радиусе
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Расчет урона в зависимости от расстояния
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    float distanceMultiplier = 1f - (distance / explosionRadius);
                    float finalDamage = damage * Mathf.Clamp(distanceMultiplier, 0.3f, 1f);
                    
                    enemy.TakeDamage(finalDamage);
                    
                    // Эффект горения
                    enemy.ApplyBurn(finalDamage * 0.1f, 3f);
                }
            }
        }
        
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}