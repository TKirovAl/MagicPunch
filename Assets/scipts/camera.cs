using UnityEngine;

public class MagicSurvivalCamera : MonoBehaviour
{
    [Header("Цель для слежения")]
    [SerializeField] private Transform target; // Персонаж
    
    [Header("Настройки камеры")]
    [SerializeField] private float smoothSpeed = 5f; // Плавность следования
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // Смещение камеры
    
    [Header("Параметры персонажа")]
    [SerializeField] private CharacterUnit characterUnit; // Ссылка на персонажа
    
    [Header("Зависимость от обзора персонажа")]
    [SerializeField] private float baseOrthographicSize = 5f; // Базовый размер камеры
    [SerializeField] private float visionMultiplier = 0.5f; // Множитель для параметра обзора
    
    [Header("Эффекты")]
    [SerializeField] private bool enableScreenShake = true;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;
    
    private Camera cam;
    private float currentShakeDuration = 0f;
    private float currentShakeIntensity = 0f;
    private Vector3 initialPosition;
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // Автоматически находим цель, если не задана
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // Автоматически находим компонент персонажа
        if (characterUnit == null && target != null)
        {
            characterUnit = target.GetComponent<CharacterUnit>();
        }
        
        initialPosition = transform.position;
    }
    
    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Камера: цель не назначена!");
            return;
        }
        
        // Обновляем размер камеры в зависимости от обзора персонажа
        UpdateCameraSize();
        
        // Следим за целью с плавностью
        FollowTarget();
        
        // Обрабатываем тряску экрана
        HandleScreenShake();
    }
    
    private void FollowTarget()
    {
        // Целевая позиция камеры
        Vector3 desiredPosition = target.position + offset;
        
        // Плавное перемещение
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Применяем позицию
        transform.position = smoothedPosition;
    }
    
    private void UpdateCameraSize()
    {
        if (characterUnit != null)
        {
            // Получаем доступ к параметру обзора персонажа
            // Если у вас есть публичное свойство или метод для получения visionRange
            float visionRange = GetVisionRangeFromCharacter();
            
            // Рассчитываем размер ортографической камеры на основе обзора
            float targetSize = baseOrthographicSize + (visionRange * visionMultiplier);
            
            // Плавно меняем размер камеры
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothSpeed * Time.deltaTime);
        }
    }
    
    private float GetVisionRangeFromCharacter()
    {
        // Метод для получения параметра обзора из персонажа
        // Вот несколько вариантов:
        
        // 1. Если у CharacterUnit есть публичное поле visionRange:
        // return characterUnit.visionRange;
        
        // 2. Если нужно получить через свойство:
        // return characterUnit.VisionRange;
        
        // 3. Через рефлексию (менее предпочтительно):
        // var field = characterUnit.GetType().GetField("visionRange");
        // if (field != null) return (float)field.GetValue(characterUnit);
        
        // Временно возвращаем базовое значение
        // Вам нужно реализовать получение реального значения из вашего класса CharacterUnit
        return 5f; // Замените на реальное получение значения
    }
    
    private void HandleScreenShake()
    {
        if (currentShakeDuration > 0)
        {
            // Генерируем случайное смещение
            Vector3 shakeOffset = Random.insideUnitSphere * currentShakeIntensity;
            shakeOffset.z = 0; // Не меняем глубину в 2D
            
            // Применяем тряску
            transform.position += shakeOffset;
            
            // Уменьшаем длительность тряски
            currentShakeDuration -= Time.deltaTime;
            
            // Плавно уменьшаем интенсивность
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, Time.deltaTime * 5f);
        }
    }
    
    // Метод для запуска тряски экрана
    public void TriggerScreenShake(float intensity = -1f, float duration = -1f)
    {
        if (!enableScreenShake) return;
        
        currentShakeIntensity = intensity > 0 ? intensity : shakeIntensity;
        currentShakeDuration = duration > 0 ? duration : shakeDuration;
    }
    
    // Метод для установки новой цели
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        if (newTarget != null)
        {
            characterUnit = newTarget.GetComponent<CharacterUnit>();
        }
    }
    
    // Метод для плавного изменения размера камеры
    public void SetCameraSize(float newSize, float duration = 1f)
    {
        StartCoroutine(ChangeCameraSizeSmoothly(newSize, duration));
    }
    
    private System.Collections.IEnumerator ChangeCameraSizeSmoothly(float targetSize, float duration)
    {
        float startSize = cam.orthographicSize;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        cam.orthographicSize = targetSize;
    }
}