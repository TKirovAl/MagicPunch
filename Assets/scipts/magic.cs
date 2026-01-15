using UnityEngine;
using System.Collections.Generic;

public class MagicManager : MonoBehaviour
{
    [System.Serializable]
    public class SpellSlot
    {
        public KeyCode key;
        public Spell spell;
        public float cooldownDisplay;
    }
    
    [Header("Слоты заклинаний")]
    public SpellSlot[] spellSlots = new SpellSlot[4];
    
    [Header("Настройки по умолчанию")]
    public GameObject fireballPrefab;
    public GameObject healEffectPrefab;
    
    private CharacterUnit character;
    private Camera mainCamera;
    
    void Start()
    {
        character = GetComponent<CharacterUnit>();
        mainCamera = Camera.main;
        
        InitializeDefaultSpells();
        SetupKeyBindings();
    }
    
    void InitializeDefaultSpells()
    {
        // Создаем стандартные заклинания
        if (spellSlots[0].spell == null)
        {
            spellSlots[0].spell = new FireballSpell()
            {
                spellName = "Огненный шар",
                element = ElementType.Fire,
                manaCost = 20f,
                cooldown = 2f,
                castTime = 0.5f,
                damage = 35f,
                explosionRadius = 3f,
                projectilePrefab = fireballPrefab
            };
        }
        
        if (spellSlots[1].spell == null)
        {
            spellSlots[1].spell = new WindBlastSpell()
            {
                spellName = "Порыв ветра",
                element = ElementType.Air,
                manaCost = 15f,
                cooldown = 4f,
                castTime = 0.3f,
                force = 20f,
                range = 10f,
                duration = 3f
            };
        }
        
        if (spellSlots[2].spell == null)
        {
            spellSlots[2].spell = new WaterHealSpell()
            {
                spellName = "Целебная волна",
                element = ElementType.Water,
                manaCost = 30f,
                cooldown = 8f,
                castTime = 1f,
                healAmount = 40f,
                healRadius = 6f
            };
        }
        
        if (spellSlots[3].spell == null)
        {
            spellSlots[3].spell = new IceNovaSpell()
            {
                spellName = "Ледяная волна",
                element = ElementType.Ice,
                manaCost = 25f,
                cooldown = 6f,
                castTime = 0.8f,
                freezeDuration = 1.5f,
                slowDuration = 4f,
                novaRadius = 8f
            };
        }
    }
    
    void SetupKeyBindings()
    {
        // QWER по умолчанию
        spellSlots[0].key = KeyCode.Q;
        spellSlots[1].key = KeyCode.W;
        spellSlots[2].key = KeyCode.E;
        spellSlots[3].key = KeyCode.R;
    }
    
    void Update()
    {
        UpdateCooldowns();
        CheckSpellInput();
        UpdateUI();
    }
    
    void UpdateCooldowns()
    {
        float deltaTime = Time.deltaTime;
        
        foreach (var slot in spellSlots)
        {
            if (slot.spell != null)
            {
                slot.spell.UpdateCooldown(deltaTime);
                slot.cooldownDisplay = slot.spell.currentCooldown;
            }
        }
    }
    
    void CheckSpellInput()
    {
        if (character == null || character.IsDead) return;
        
        // Проверяем каждую клавишу
        for (int i = 0; i < spellSlots.Length; i++)
        {
            if (Input.GetKeyDown(spellSlots[i].key))
            {
                TryCastSpell(i);
            }
        }
        
        // Дополнительные клавиши для будущих заклинаний
        if (Input.GetKeyDown(KeyCode.T))
        {
            // T - для 5-го заклинания
            Debug.Log("Слот T - зарезервирован для будущих заклинаний");
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // Y - для 6-го заклинания
            Debug.Log("Слот Y - зарезервирован для будущих заклинаний");
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            // F - для ультимейта или особого заклинания
            Debug.Log("Слот F - ультимейт");
        }
    }
    
    void TryCastSpell(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= spellSlots.Length) return;
        
        SpellSlot slot = spellSlots[slotIndex];
        
        if (slot.spell == null)
        {
            Debug.Log($"Слот {slot.key} пуст!");
            return;
        }
        
        // Проверяем условия каста
        if (!slot.spell.IsReady)
        {
            Debug.Log($"{slot.spell.spellName} перезаряжается!");
            return;
        }
        
        if (character.CurrentEnergy < slot.spell.manaCost)
        {
            Debug.Log("Недостаточно маны!");
            return;
        }
        
        // Получаем позицию каста (курсор мыши в мире)
        Vector3 targetPosition = GetTargetPosition();
        
        // Расходуем ману
        character.ConsumeEnergy(slot.spell.manaCost);
        
        // Кастуем заклинание
        slot.spell.Cast(character, targetPosition);
        
        // Запускаем анимацию каста
        character.TriggerCastAnimation(slot.spell.castTime);
    }
    
    Vector3 GetTargetPosition()
    {
        if (mainCamera == null) return transform.position + transform.forward * 5f;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f))
        {
            return hit.point;
        }
        
        // Если луч никуда не попал, используем направление от персонажа
        return transform.position + transform.forward * 10f;
    }
    
    void UpdateUI()
    {
        // Здесь обновляем UI с кд и маной
        // Можно использовать Unity UI или другие системы
    }
    
    // Методы для управления заклинаниями
    public void LearnSpell(Spell newSpell, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < spellSlots.Length)
        {
            spellSlots[slotIndex].spell = newSpell;
            Debug.Log($"Выучил новое заклинание: {newSpell.spellName} в слот {slotIndex}");
        }
    }
    
    public void SwapSpells(int slotA, int slotB)
    {
        if (slotA >= 0 && slotA < spellSlots.Length && 
            slotB >= 0 && slotB < spellSlots.Length)
        {
            Spell temp = spellSlots[slotA].spell;
            spellSlots[slotA].spell = spellSlots[slotB].spell;
            spellSlots[slotB].spell = temp;
        }
    }
    
    public Spell GetSpellInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < spellSlots.Length)
        {
            return spellSlots[slotIndex].spell;
        }
        return null;
    }
    
    public bool HasSpellOfElement(ElementType element)
    {
        foreach (var slot in spellSlots)
        {
            if (slot.spell != null && slot.spell.element == element)
            {
                return true;
            }
        }
        return false;
    }
    
    // Метод для сброса всех заклинаний
    public void ResetAllCooldowns()
    {
        foreach (var slot in spellSlots)
        {
            if (slot.spell != null)
            {
                slot.spell.currentCooldown = 0;
            }
        }
    }
}