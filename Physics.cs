public class Physics : MonoBehaviour
{
    public Collider2D col;

    public const offset = 0.01f;
    public const min_size = 0.1f;
    
    public const gravity = 9.8f;


    public bool is_movible;
    public float mass;

    public float speed_x;
    public float speed_y;


    public virtual void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
    }

    public virtual void Update()
    {
        speed_y -= gravity * mass * Time.deltaTime;

        MoveX(speed_x * Time.deltaTime);
        MoveY(speed_y * Time.deltaTime);

    }

    public void MoveX(float f)
    {
        if (f != 0f)
        {
            Dictionary<Collider2D, float> colliders = new Dictionary<Collider2D, float>();
            float max = MaxMoveX(f, colliders);
            float sign = Mathf.Sign(f);
            speed_x = max * sign < f * sign ? 0 : speed_x;
            foreach (KeyValuePair<Collider2D, float> k in colliders)
            {
                if (max * sign >= k.Value)
                {
                    Physics phy = k.Key.GetComponent<Physics>();
                    phy.MoveX(max - k.Value * sign);
                }
            }
            transform.position += Vector3.right * max;
        }
    }

    public float MaxMoveX(float f, Dictionary<Collider2D, float> colliders)
    {
        if (f != 0f)
        {
            float diference = col.bounds.max.y - col.bounds.min.y - 2*offset;
            float amount = diference / min_size;
            float sign = Mathf.Sign(f);
            float xposition = f > 0 ? col.bounds.max.x + offset : col.bounds.min.x - offset;
            for (float i = 0; i <= amount; i++)
            {
                float yposition = offset + col.bounds.min.y + diference * i / amount;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(xposition, yposition), Vector2.right * sign, Mathf.Abs(f), Layers.Ignore(Layers.triggers));
                if (hit.collider != null)
                {
                    Physics phy = hit.collider.gameObject.GetComponent<Physics>();
                    float g = sign * hit.distance + (phy.is_movible ? phy.MaxMoveX(f - sign * hit.distance, new Dictionary<Collider2D, float>()) : 0);
                    f = g * sign < f * sign ? g : f;
                    colliders[hit.collider] = hit.distance;
                }
            }
        }
        return f;
    }

    public void MoveY(float f)
    {
        if (f != 0)
        {
            Dictionary<Collider2D, float> colliders = new Dictionary<Collider2D, float>();
            float max = MaxMoveY(f, colliders);
            float sign = Mathf.Sign(f);
            grounded = f < max;
            speed_y = max * sign < f * sign ? 0 : speed_y;
            foreach (KeyValuePair<Collider2D, float> k in colliders)
            {
                if (max * sign >= k.Value)
                {
                    Physics phy = k.Key.gameObject.GetComponent<Physics>();
                    phy.MoveY(max - k.Value * sign);
                }
            }
            transform.position += Vector3.up * max;
        }
    }

    public float MaxMoveY(float f, Dictionary<Collider2D, float> colliders)
    {
        if (f != 0f)
        {
            float diference = col.bounds.max.x - col.bounds.min.x - 2*offset;
            float amount = diference / min_size;
            float sign = Mathf.Sign(f);
            float yposition = f > 0 ? col.bounds.max.y + offset : col.bounds.min.y - offset;
            for (float i = 0; i <= amount; i++)
            {
                float xposition = 0.01f + col.bounds.min.x + diference * i / amount;
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(xposition, yposition), Vector2.up * sign, Mathf.Abs(f), Layers.Ignore(Layers.triggers));
                if (hit.collider != null)
                {
                    Physics phy = hit.collider.gameObject.GetComponent<Physics>();
                    float g = sign * hit.distance + (phy.is_movible ? phy.MaxMoveY(f - sign * hit.distance, new Dictionary<Collider2D, float>()) : 0);
                    f = g * sign < f * sign ? g : f;
                    colliders[hit.collider] = hit.distance;
                }
            }
        }
        return f;
    }
}
