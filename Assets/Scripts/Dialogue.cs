using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] lines;
    public float typingSpeed;
    public GameObject portal; // Portal objesi referans�
    public GameObject enemyPrefabObject;
    public Transform player; // Karakterin referans�
    public Transform portalPosition; // Portal�n pozisyonu
    public float moveSpeed = 2f; // Karakterin hareket h�z�
    private Animator animator;

    private int index;

    // Start is called before the first frame update
    void Start()
    {
        animator = player.GetComponent<Animator>();
        textDisplay.text = string.Empty;
        StartDialogue();
        portal.SetActive(false); // Portal ba�lang��ta gizli
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textDisplay.text == lines[index])
            {
                NextLines();
            }
            else
            {
                StopAllCoroutines();
                textDisplay.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        foreach (char letter in lines[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void NextLines()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textDisplay.text = string.Empty;
            StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = string.Empty;
            portal.SetActive(true); // Portal� aktif hale getir
            StartCoroutine(MoveToPortal()); // Karakteri portala y�nlendir
        }
    }

    IEnumerator MoveToPortal()
    {
        animator.SetBool("isRun", true); // Karakterin ko�ma animasyonunu ba�lat
        player.DOMove(portalPosition.position, moveSpeed); // Karakteri portala do�ru hareket ettir
        yield return new WaitForSeconds(moveSpeed); // Hareket s�resi kadar bekle
        BattleSystem.enemyPrefab = enemyPrefabObject;
        SceneManager.LoadScene(2); // Bir sonraki sahneye ge�
    }
}
