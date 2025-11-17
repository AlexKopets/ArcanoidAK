using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    public float speed = 8f;
    public float horizontalImpulseFactor = 4f;   // увеличь чтобы сильнее отдавать в сторону
    public float extraVertical = 1.2f;          // добавочная вертикальная составляющая
    public float maxBounceAngleDeg = 75f;       // максимальный угол от вертикали

    Rigidbody2D rb;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    public void Launch(Vector2 direction)
    {
        direction = direction.normalized;
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D col)
{
    if (rb == null) return;

    if (col.collider.CompareTag("Paddle"))
    {
        // берём нормаль первого контакта, с защитой на случай отсутствия контактов
        Vector2 normal = (col.contacts != null && col.contacts.Length > 0) ? col.contacts[0].normal : Vector2.up;
        if (normal.sqrMagnitude < 0.0001f) normal = Vector2.up;

        // базовое отражение от нормали
        Vector2 incoming = rb.velocity.sqrMagnitude > 0.0001f ? rb.velocity.normalized : Vector2.down;
        Vector2 reflected = Vector2.Reflect(incoming, normal).normalized;

        // добавляем офсет по горизонтали в зависимости от точки контакта
        Vector2 contactPoint = (col.contacts != null && col.contacts.Length > 0) ? col.contacts[0].point : (Vector2)transform.position;
        float offset = (contactPoint.x - col.collider.bounds.center.x) / col.collider.bounds.extents.x;
        offset = Mathf.Clamp(offset, -1f, 1f);

        // горизонтальный вклад — уменьшай/увеличивай factor по вкусу
        float horizontalFactor = horizontalImpulseFactor; // объявленное поле
        reflected.x += offset * horizontalFactor * 0.2f; // небольшой контролируемый вклад
        reflected = reflected.normalized;

        // ограничиваем угол (чтобы мяч не ушёл в почти горизонталь)
        float maxAngleRad = maxBounceAngleDeg * Mathf.Deg2Rad;
        float angle = Mathf.Atan2(reflected.x, reflected.y);
        angle = Mathf.Clamp(angle, -maxAngleRad, maxAngleRad);
        Vector2 finalDir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized;

        // назначаем скорость и слегка двигаем мяч вдоль нормали, чтобы он не застрял
        rb.velocity = finalDir * Mathf.Max(0.001f, speed); // гарантируем ненулевую скорость
        transform.position += (Vector3)normal * 0.02f;
        return;
    }

    // прочие столкновения — просто поддерживаем скорость
    rb.velocity = rb.velocity.normalized * speed;
}
}
