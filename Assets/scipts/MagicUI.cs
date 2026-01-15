using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellUI : MonoBehaviour
{
    [System.Serializable]
    public class SpellUIElement
    {
        public Image icon;
        public Text keyText;
        public Text cooldownText;
        public Image cooldownOverlay;
        public Text manaCostText;
    }
    
    public SpellUIElement[] spellUIElements = new SpellUIElement[4];
    public Text energyText;
    
    private MagicManager magicManager;
    private CharacterUnit character;
    
    void Start()
    {
        character = FindObjectOfType<CharacterUnit>();
        magicManager = character?.GetComponent<MagicManager>();
        
        InitializeUI();
    }
    
    void InitializeUI()
    {
        if (magicManager == null) return;
        
        for (int i = 0; i < spellUIElements.Length; i++)
        {
            if (i < magicManager.spellSlots.Length)
            {
                Spell spell = magicManager.GetSpellInSlot(i);
                if (spell != null)
                {
                    UpdateSpellUI(i, spell);
                }
            }
        }
    }
    
    void Update()
    {
        if (magicManager == null || character == null) return;
        
        // Обновляем кд заклинаний
        for (int i = 0; i < spellUIElements.Length; i++)
        {
            if (i < magicManager.spellSlots.Length)
            {
                MagicManager.SpellSlot slot = magicManager.spellSlots[i];
                if (slot.spell != null)
                {
                    // Обновляем кд
                    if (spellUIElements[i].cooldownText != null)
                    {
                        if (slot.spell.currentCooldown > 0)
                        {
                            spellUIElements[i].cooldownText.text = slot.spell.currentCooldown.ToString("F1");
                            spellUIElements[i].cooldownText.gameObject.SetActive(true);
                        }
                        else
                        {
                            spellUIElements[i].cooldownText.gameObject.SetActive(false);
                        }
                    }
                    
                    // Обновляем оверлей кд
                    if (spellUIElements[i].cooldownOverlay != null)
                    {
                        float cooldownPercent = slot.spell.currentCooldown / slot.spell.cooldown;
                        spellUIElements[i].cooldownOverlay.fillAmount = cooldownPercent;
                    }
                }
            }
        }
        
        // Обновляем энергию
        if (energyText != null)
        {
            energyText.text = $"Мана: {character.CurrentEnergy:F0}/{character.MaxEnergy:F0}";
        }
    }
    
    void UpdateSpellUI(int slotIndex, Spell spell)
    {
        if (slotIndex >= spellUIElements.Length) return;
        
        SpellUIElement uiElement = spellUIElements[slotIndex];
        
        // Устанавливаем иконку (если есть)
        if (uiElement.icon != null && spell.icon != null)
        {
            uiElement.icon.sprite = spell.icon;
            uiElement.icon.color = GetElementColor(spell.element);
        }
        
        // Устанавливаем текст клавиши
        if (uiElement.keyText != null)
        {
            uiElement.keyText.text = GetKeyCodeText(magicManager.spellSlots[slotIndex].key);
        }
        
        // Устанавливаем стоимость маны
        if (uiElement.manaCostText != null)
        {
            uiElement.manaCostText.text = spell.manaCost.ToString("F0");
        }
    }
    
    string GetKeyCodeText(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Q: return "Q";
            case KeyCode.W: return "W";
            case KeyCode.E: return "E";
            case KeyCode.R: return "R";
            case KeyCode.T: return "T";
            case KeyCode.Y: return "Y";
            case KeyCode.F: return "F";
            default: return key.ToString();
        }
    }
    
    Color GetElementColor(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return new Color(1f, 0.3f, 0f, 1f); // Оранжево-красный
            case ElementType.Air: return new Color(0.7f, 1f, 1f, 1f); // Голубой
            case ElementType.Water: return new Color(0f, 0.5f, 1f, 1f); // Синий
            case ElementType.Ice: return new Color(0.5f, 0.8f, 1f, 1f); // Светло-синий
            default: return Color.white;
        }
    }
}