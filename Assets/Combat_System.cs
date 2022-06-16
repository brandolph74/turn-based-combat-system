using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
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
            return;
        }

        StartCoroutine(PlayerAttack());

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
            //EndBattle();
        }
        else
        {
            state = CombatState.enemy_turn;
        }

    }

}
