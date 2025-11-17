using UnityEngine;

public class PaddleMouseInput : MonoBehaviour
{
    [Tooltip("Если не указана — будет использована Camera.main")]
    public Camera cam;

    // Z-плоскость, на которой лежит платформа. Если = float.NaN — будет использован transform.position.z
    public float planeZ = float.NaN;

    void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
                Debug.LogWarning("PaddleMouseInput: Main Camera not found. Please assign a Camera in the inspector or tag your camera as MainCamera.");
        }
    }

    // Возвращает X в мировых координатах для позиции мыши на той же Z-плоскости, где находится paddle
    public float GetMouseWorldX()
    {
        // Проверки на камеру
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
                return transform.position.x;
        }

        // Определяем Z-координату плоскости (где находится платформа)
        float paddleZ = float.IsNaN(planeZ) ? transform.position.z : planeZ;

        // Берём позицию курсора
        Vector3 mousePos = Input.mousePosition;

        // Защита от NaN/Inf в координатах мыши
        if (!IsFinite(mousePos.x) || !IsFinite(mousePos.y))
        {
            // Попробуем использовать центр экрана как fallback
            mousePos.x = Screen.width * 0.5f;
            mousePos.y = Screen.height * 0.5f;
        }

        // Расстояние от камеры до плоскости, обязательно положительное
        float distance = Mathf.Abs(cam.transform.position.z - paddleZ);
        if (!IsFinite(distance) || distance <= 0f) distance = 1f;

        mousePos.z = distance;

        // Если курсор находится за пределами rect камеры (multi-camera/canvas) — можно ограничить
        // но основная защита — корректный z, поэтому делаем ScreenToWorldPoint
        Vector3 world = cam.ScreenToWorldPoint(mousePos);

        // Защита на случай, если world.x вдруг Inf/NaN
        if (!IsFinite(world.x)) return transform.position.x;

        return world.x;
    }

    static bool IsFinite(float v) => !(float.IsNaN(v) || float.IsInfinity(v));
}