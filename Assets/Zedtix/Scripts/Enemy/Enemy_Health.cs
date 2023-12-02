using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Enemy_Health : MonoBehaviour
{
   // public Slider slider;
    public Vector3 offect;
    public float maxhealth = 10;
    private float health;
    public int killvalue = 1;
    [SerializeField]
    public GameObject BloodEffect;
    public GameObject Coins;
    public GameObject DamageText;


  
    // ******************** Flash Stuff*********************
    public Material flashMaterial;  
    public float duration; 
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;

        health = maxhealth;

    }

    void Update()
    {


        if (health <= 0)
        {          
                if(Coins!=null)
                Instantiate(Coins, transform.position, Quaternion.identity);
            GameManager.Instance.NumberOfKills++;
            Destroy(gameObject);                    
        }

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(GetComponent<Enemy_Movement>()!=null)
        GetComponent<Enemy_Movement>().NockBackTime = .2f;

        AudioManager.instance.PlaySound("Enemy_Hurt");
        if (DamageText != null)
        {
            GameObject Text = Instantiate(DamageText, transform.position, Quaternion.identity);

            Text.GetComponent<TMP_Text>().text = damage.ToString();
        }
        if(flashMaterial!=null)
        Flash();
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
      
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }
}
