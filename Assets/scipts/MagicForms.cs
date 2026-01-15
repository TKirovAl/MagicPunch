using UnityEngine;
using System.Collections.Generic;

// Типы элементов
public enum ElementType
{
    Fire,       // Огонь - горение, взрывы
    Air,        // Воздух - скорость, контроль
    Water,      // Вода - лечение, контроль
    Ice         // Лёд - замедление, контроль
}

// Базовый класс для всех заклинаний
[System.Serializable]
public abstract class Spell
{
    public string spellName;
    public ElementType element;
    public Sprite icon;
    public float manaCost;
    public float cooldown;
    public float castTime;
    
    protected float currentCooldown = 0f;
    
    public bool IsReady => currentCooldown <= 0f;
    
    public virtual void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= deltaTime;
        }
    }
    
    public abstract void Cast(CharacterUnit caster, Vector3 targetPosition);
    
    public virtual string GetDescription()
    {
        return $"{spellName}\nЭлемент: {element}\nМана: {manaCost}\nКД: {cooldown}с";
    }
}

// Конкретные заклинания для каждого элемента
[System.Serializable]
public class FireballSpell : Spell
{
    public float damage = 30f;
    public float explosionRadius = 3f;
    public GameObject projectilePrefab;
    
    public override void Cast(CharacterUnit caster, Vector3 targetPosition)
    {
        currentCooldown = cooldown;
        
        // Создание файрбола
        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = caster.transform.position + caster.transform.forward;
            GameObject fireball = GameObject.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            
            // Настройка файрбола
            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(caster, targetPosition, damage, explosionRadius);
            }
        }
        
        Debug.Log($"Кастуется {spellName}!");
    }
}

[System.Serializable]
public class WindBlastSpell : Spell
{
    public float force = 15f;
    public float range = 8f;
    public float duration = 3f;
    
    public override void Cast(CharacterUnit caster, Vector3 targetPosition)
    {
        currentCooldown = cooldown;
        
        // Создание ветряного импульса
        Vector3 direction = (targetPosition - caster.transform.position).normalized;
        
        // Поиск врагов в конусе
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, range);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direction * force, ForceMode.Impulse);
                    
                    // Можно добавить дебафф замедления после отталкивания
                    Enemy enemy = collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.ApplySlow(0.5f, duration);
                    }
                }
            }
        }
        
        Debug.Log($"Кастуется {spellName}!");
    }
}

[System.Serializable]
public class WaterHealSpell : Spell
{
    public float healAmount = 25f;
    public float healRadius = 5f;
    
    public override void Cast(CharacterUnit caster, Vector3 targetPosition)
    {
        currentCooldown = cooldown;
        
        // Лечение себя и союзников в радиусе
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, healRadius);
        foreach (var collider in colliders)
        {
            CharacterUnit unit = collider.GetComponent<CharacterUnit>();
            if (unit != null && unit.CompareTag("Player"))
            {
                unit.Heal(healAmount);
                
                // Визуальный эффект
                GameObject healEffect = new GameObject("HealEffect");
                healEffect.transform.position = unit.transform.position;
                // Здесь можно добавить частицы
            }
        }
        
        Debug.Log($"Кастуется {spellName}!");
    }
}

[System.Serializable]
public class IceNovaSpell : Spell
{
    public float freezeDuration = 2f;
    public float slowDuration = 5f;
    public float novaRadius = 7f;
    
    public override void Cast(CharacterUnit caster, Vector3 targetPosition)
    {
        currentCooldown = cooldown;
        
        // Ледяная волна от персонажа
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, novaRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Проверка расстояния для полного заморозки
                    float distance = Vector3.Distance(caster.transform.position, enemy.transform.position);
                    if (distance < novaRadius * 0.5f)
                    {
                        enemy.Freeze(freezeDuration);
                    }
                    else
                    {
                        enemy.ApplySlow(0.3f, slowDuration);
                    }
                }
            }
        }
        
        Debug.Log($"Кастуется {spellName}!");
    }
}