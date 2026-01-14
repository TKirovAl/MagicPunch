using UnityEngine;

[System.Serializable]
public class MagicType
{
    public string magicName;
    public Color magicColor;
    public float baseDamage;
    public float manaCost;
    // Дополнительные свойства типа магии
}

public class CharacterUnit : MonoBehaviour
{
    // Основные характеристики
    [Header("Основные характеристики")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Защита")]
    [SerializeField] private float armor = 10f;
    
    [Header("Критические атаки")]
    [SerializeField] [Range(0f, 1f)] private float criticalChance = 0.1f;
    [SerializeField] private float criticalDamageMultiplier = 2f;
    
    [Header("Передвижение")]
    [SerializeField] private float movementSpeed = 5f;
    
    [Header("Магия")]
    [SerializeField] private MagicType magicType;
    [SerializeField] private float castDelay = 1f;
    [SerializeField] private float currentCastTimer = 0f;
    
    [Header("Обзор")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 120f;
    
    [Header("Энергия/Мана")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy;
    [SerializeField] private float energyRegenerationRate = 5f;
    
    [Header("Регенерация")]
    [SerializeField] private float healthRegenerationRate = 1f;
    
    [Header("Ближний бой")]
    [SerializeField] private float meleeDamage = 15f;
    
    // Свойства для доступа к приватным полям
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public float MovementSpeed => movementSpeed;
    public MagicType MagicType => magicType;
    public bool CanCast => currentCastTimer <= 0 && currentEnergy >= magicType.manaCost;
    
    // Компоненты
    private Rigidbody rb;
    private Animator animator;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        // Инициализация
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
    }
    
    private void Update()
    {
        UpdateTimers();
        RegenerateResources();
    }
    
    private void UpdateTimers()
    {
        if (currentCastTimer > 0)
        {
            currentCastTimer -= Time.deltaTime;
        }
    }
    
    private void RegenerateResources()
    {
        // Регенерация здоровья
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegenerationRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
        
        // Регенерация энергии
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenerationRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
    }
    
    // Метод получения урона
    public void TakeDamage(float damage)
    {
        // Учет брони
        float damageAfterArmor = damage * (1 - armor / (armor + 100));
        currentHealth -= damageAfterArmor;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // Метод лечения
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    
    // Метод атаки ближнего боя
    public float PerformMeleeAttack()
    {
        // Проверка на критический удар
        bool isCritical = Random.value <= criticalChance;
        float damage = meleeDamage;
        
        if (isCritical)
        {
            damage *= criticalDamageMultiplier;
        }
        
        return damage;
    }
    
    // Метод использования магии
    public bool CastSpell()
    {
        if (!CanCast) return false;
        
        currentEnergy -= magicType.manaCost;
        currentCastTimer = castDelay;
        
        // Здесь логика каста магии
        Debug.Log($"Кастуется {magicType.magicName}!");
        
        return true;
    }
    
    // Метод движения
    public void Move(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            Vector3 moveDirection = direction.normalized * movementSpeed;
            if (rb != null)
            {
                rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
            }
            else
            {
                transform.position += moveDirection * Time.deltaTime;
            }
            
            // Поворот в сторону движения
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    
    // Метод смерти
    private void Die()
    {
        Debug.Log($"{gameObject.name} погиб!");
        
        // Отключение компонентов или уничтожение объекта
        if (animator != null) animator.SetTrigger("Die");
        
        // Дополнительная логика смерти
        Destroy(gameObject, 3f);
    }
    
    // Методы для изменения характеристик
    public void ModifyMovementSpeed(float multiplier)
    {
        movementSpeed *= multiplier;
    }
    
    public void ModifyCriticalChance(float amount)
    {
        criticalChance = Mathf.Clamp(criticalChance + amount, 0f, 1f);
    }
    
    public void ModifyArmor(float amount)
    {
        armor += amount;
        armor = Mathf.Max(armor, 0);
    }
    
    // Визуализация в редакторе (для дебага)
    private void OnDrawGizmosSelected()
    {
        // Отображение радиуса обзора
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // Отображение угла обзора
        Vector3 leftDirection = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftDirection * visionRange);
        Gizmos.DrawRay(transform.position, rightDirection * visionRange);
    }
}