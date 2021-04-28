using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public GameObject bullet;
    public Slider healthBar;
    public GameObject explosion;
    public float damage = 10f;

    void Update()
    {
        float translation = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        transform.Translate(translation, 0, 0);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Instantiate(bullet, this.transform.position, Quaternion.identity);
            GameObject obj = Pool.singleton.Get("bullet");
            if (obj != null)
            {
                obj.transform.position = this.transform.position;
                obj.SetActive(true);
            }
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, -120, 0);
        healthBar.transform.position = screenPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "asteroid")
        {
            healthBar.value -= damage;
            if (healthBar.value <= 0f)
            {
                GameObject explo = Instantiate(explosion, transform.position + new Vector3(0, -1f, 0), Quaternion.identity);
                Destroy(healthBar.gameObject, .1f);
                Destroy(gameObject, .1f);
                Destroy(gameObject, 1f);
            }
        }
    }
}