using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CombatState {start, player_turn, enemy_turn, victory, defeat, enemy_dialogue, modifier}

public class Combat_System : MonoBehaviour
{

    public CombatState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerLocation;
    public Transform enemyLocation;

    Unit playerStats;
    Unit enemyStats;

    public TextMeshProUGUI dialogue;

    public CombatHUD playerHUD;
    public CombatHUD enemyHUD;

    public GameObject buttons;

    // Start is called before the first frame update
    void Start()
    {
        buttons.SetActive(false);
        state = CombatState.start;   //immediately launch the intro combat sequence
        //in the future create a Unity timeline here with easy animations of the UI and player sprites sliding in
        
        StartCoroutine(InstantiateCombatants());  //kick off the chain of coroutines for battle
    }

    IEnumerator InstantiateCombatants()
    {

        GameObject player = Instantiate(playerPrefab, playerLocation);   //can always just have the player be used in the serialized field
        GameObject enemy = Instantiate(enemyPrefab, enemyLocation);      //enemy name would be stored as a playerpref once the ecounter began for dynamic enemies

        playerStats = player.GetComponent<Unit>();
        enemyStats = enemy.GetComponent<Unit>();         //get the stats component for each character


        player.transform.position = playerLocation.transform.position;   //had to add these in because of a transform bug in the version of unity used
        enemy.transform.position = enemyLocation.transform.position;

        dialogue.text = "A " + enemyStats.unitName + " challenges you to battle.";

        playerHUD.populateHUD(playerStats);
        enemyHUD.populateHUD(enemyStats);    //pass in the unit scripts to CombatHUD class to set the UI to their values

        yield return new WaitForSeconds(3f);  //wait for a pause in gameplay

        buttons.SetActive(true);

        state = CombatState.player_turn;   //hand off control to the player

        PlayerAction();

    }

    void PlayerAction()
    {
        dialogue.text = "Player Actions: ";
    }

    public void AttackPressed()
    {
        if (state != CombatState.player_turn)
        {
            return;                             //check here to see if player turn
        }

        StartCoroutine(PlayerAttack());
        buttons.SetActive(false);

    }

    public void EscapePressed()
    {
        if (state != CombatState.player_turn)
        {
            return;                             //check here to see if player turn
        }

        StartCoroutine(PlayerEscape());
        buttons.SetActive(false);

    }

    IEnumerator PlayerEscape()
    {
        dialogue.text = "Player escaped from battle!";
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("World");
        
    }

    public void RegenPressed()
    {
        if (state != CombatState.player_turn)
        {
            return;
        }

        StartCoroutine(PlayerHeal());
        buttons.SetActive(false);

    }

    IEnumerator PlayerHeal()
    {
        dialogue.text = "Player focused for this turn and regained strength.";
        yield return new WaitForSeconds(2.5f);
        playerStats.takeDamage(-25);     //negative since the player is healing, subtracts the negative from health
        dialogue.text = "Healed for 25 HP.";
        yield return new WaitForSeconds(2.5f);
        state = CombatState.enemy_turn;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyStats.takeDamage(playerStats.damage); //pass in how much damage the player does to the enemy stats
        
        enemyHUD.setHP(enemyStats.currentHP);    //update enemy HP

        dialogue.text = "Attacked " + enemyStats.unitName + " for " + playerStats.damage + " damage.";
        Debug.Log(enemyStats.currentHP);
        
        yield return new WaitForSeconds(3f);

        if(isDead)
        {
            state = CombatState.victory;
            StartCoroutine(EndBattle());
        }
        else
        {
            state = CombatState.enemy_turn;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EndBattle()
    {
        if (state == CombatState.victory)
        {
            dialogue.text = enemyStats.unitName + " was defeated.";
        }
        else if (state == CombatState.defeat)
        {
            dialogue.text = "You were defeated by " + enemyStats.unitName;
        }

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("World");


    }

    IEnumerator EnemyTurn()
    {
        dialogue.text = enemyStats.unitName + " attacks for " + enemyStats.damage + " damage";
        bool isDead = playerStats.takeDamage(enemyStats.damage);
        playerHUD.setHP(playerStats.currentHP);

        if (isDead)
        {
            state = CombatState.defeat;
            StartCoroutine(EndBattle());
        }
        else
        {
            yield return new WaitForSeconds(2f);
            state = CombatState.player_turn;
            buttons.SetActive(true);
            PlayerAction();
        }

        yield return null;
    }

    

}
