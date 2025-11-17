using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleControllerMouse : MonoBehaviour
{
    public PaddleMouseInput mouseInput; // назначь в инспекторе или оставь пустым — скрипт найдёт автоматически
    public float lerp = 0.9f; // сглаживание движения (0..1)

    public Rigidbody2D rb;
    public Vector2 boundsMinMaxX = new Vector2(-7.5f, 7.5f);


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (mouseInput == null)
            mouseInput = GetComponent<PaddleMouseInput>() ?? FindObjectOfType<PaddleMouseInput>();
        if (mouseInput == null)
            Debug.LogWarning("PaddleControllerMouse: PaddleMouseInput not found. Assign in inspector for best results.");
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Получаем мировую X позиции курсора на плоскости платформы
        float targetX = (mouseInput != null) ? mouseInput.GetMouseWorldX() : transform.position.x;

        // Собираем целевую позицию, оставляя Y и Z прежними
        Vector2 target = new Vector2(targetX, rb.position.y);

        // Перемещаем платформу корректно для kinematic Rigidbody2D
        Vector2 newPos = Vector2.Lerp(rb.position, target, lerp);
        rb.MovePosition(newPos);
    }
}