using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public static GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    private Animator playerAnimator;
    private Animator enemyAnimator;

    public Text skillButtonText;
    public GameObject skillButton;

    public AudioClip[] audioclips;
    public AudioClip defaultClip;
    public static AudioClip enemyClip;

    public static int winIndex;
    Unit playerUnit;
    Unit enemyUnit;

    public static BattleState state;

    private void Start()
    {
        enemyClip = enemyUnit.GetComponent<AudioSource>().clip;
        switch (NewEnemySetup.choosedSkill)
        {
            case 1:
                skillButtonText.text = "BotWheel";
                
                skillButton.SetActive(true);
                break;
            case 2:
                skillButtonText.text = "NightBorne";
                skillButton.SetActive(true);
                break;
            case 3:
                skillButtonText.text = "Wizard";
                skillButton.SetActive(true);
                break;
            case 4:
                skillButtonText.text = "Slime";
                skillButton.SetActive(true);
                break;
            case 5:
                skillButtonText.text = "Demon";
                skillButton.SetActive(true);
                break;
            case 6:
                skillButtonText.text = "Necromancer";
                skillButton.SetActive(true);
                break;
            case 7:
                skillButtonText.text = "Patron";
                skillButton.SetActive(true);
                break;
            default:
                skillButtonText.text = "LOCKED";
                skillButton.SetActive(false);
                break;
        }
    }

    void Awake()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        playerGo.transform.position = new Vector2(playerBattleStation.position.x - 1.6f, playerBattleStation.position.y);
        playerUnit = playerGo.GetComponent<Unit>();
        if (playerUnit == null)
        {
            Debug.LogError("Player Unit component is missing on playerPrefab.");
            yield break;
        }

        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyGo.transform.position = new Vector2(enemyBattleStation.position.x + 1.6f, enemyBattleStation.position.y);
        enemyUnit = enemyGo.GetComponent<Unit>();
        enemyUnit.isEnemy = true;
        if (enemyUnit == null)
        {
            Debug.LogError("Enemy Unit component is missing on enemyPrefab.");
            yield break;
        }

        playerAnimator = playerUnit.GetComponent<Animator>();
        enemyAnimator = enemyUnit.GetComponent<Animator>();

        yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;
    }

    IEnumerator PlayerAttack()
    {
        if (state == BattleState.PLAYERTURN)
        {
            state = BattleState.ENEMYTURN;
            yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "Attack", defaultClip));
            yield return new WaitForSeconds(0.3f);
            if (enemyUnit.currentHP <= 0)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                StartCoroutine(EnemyTurn());
            }
        }
    }

    IEnumerator EnemyTurn()
    {
        if (state == BattleState.ENEMYTURN)
        {
            yield return StartCoroutine(enemyUnit.Attack(playerBattleStation, enemyBattleStation, playerAnimator, playerUnit, "Attack", enemyClip));
            yield return new WaitForSeconds(0.3f);
            if (playerUnit.currentHP <= 0)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
            }
        }
    }

    public void EndBattle()
    {
        if (state == BattleState.WON)
        {
            winIndex++;
            SceneManager.LoadScene(3);
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("You were defeated.");
        }
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        playerUnit.isMove = true;
        StartCoroutine(PlayerAttack());
    }

    public void OnSkillButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(skillChoose());
    }

    public IEnumerator skillChoose()
    {
        if (state == BattleState.PLAYERTURN)
        {
            state = BattleState.ENEMYTURN;
            switch (NewEnemySetup.choosedSkill)
            {
                case 1:
                    playerUnit.isMove = false;
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "BotWheel", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                case 2:
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "NightBorne", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                case 3:
                    playerUnit.isMove = false;
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "Wizard", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                case 4:
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "Slime", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                case 5:
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "Demon", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                case 6:
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "Necromancer", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                case 7:
                    yield return StartCoroutine(playerUnit.Attack(enemyBattleStation, playerBattleStation, enemyAnimator, enemyUnit, "Patron", audioclips[NewEnemySetup.choosedSkill - 1]));
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(1f);
        }
            if (enemyUnit.currentHP <= 0)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                StartCoroutine(EnemyTurn());
            }
    }
}
