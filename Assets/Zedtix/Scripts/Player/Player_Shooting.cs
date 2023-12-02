using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shooting : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    public GameObject bulletPrefab;
    public Transform FirePostion;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring,BulletDamage=5;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
       
     //   transform.localScale = new Vector2(transform.parent.GetComponent<Player_Controller>().Right, 1);
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            AudioManager.instance.PlaySound("Shooting");
            GameObject Bullett = Instantiate(bulletPrefab, FirePostion.position, Quaternion.identity);
            Bullett.GetComponent<Player_Bullet>().Damage = BulletDamage;
        }
    }
}
