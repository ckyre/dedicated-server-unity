using UnityEngine;

public class Player : MonoBehaviour
{
    public float health;
    public float speed;
    public float damages;

    Rigidbody2D rb;
    Vector2 inputs;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Movements
        inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.velocity = inputs * speed;

        //Rotation
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        LayerMask mask = LayerMask.GetMask("NetworkPlayer");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, mask);
        if (hit.transform != null && hit.transform.GetComponent<NetworkIdentifier>() != null)
        {
            DataSender.SendTakeDamages(hit.transform.GetComponent<NetworkIdentifier>().id, damages);
        }
    }

    public void TakeDamages (float count, int fromID)
    {
        health -= count;

        if (health <= 0f)
        {
            Die(fromID);
        }
    }

    public void Die (int lastDamageFromID)
    {
        DataSender.SendDie(lastDamageFromID);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        Destroy(gameObject);
    }

}
