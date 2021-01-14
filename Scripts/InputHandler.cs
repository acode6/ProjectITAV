using GameCreator.Melee;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script is used to handle the player inputs
public class InputHandler : MonoBehaviour
{
    //Inputs
    float vertical;
    float horizontal;
    bool fire1;
    bool fire2;
    bool fire3;
    bool fire4;

    bool tabInput;
    bool useInput;
    bool shiftInput; //shift
    bool lockOnButton;
    bool LCtrl;
    bool Esc;
    bool AdminMenu;

    //Timers
    float shiftTimer;
    float delta;


    //lockon
    public List<GameObject> enemies = new List<GameObject>();
    public Transform lockOnTransform;
    public int enemyIndex;
    float rightaxis;
    public bool islockedOn;

    bool sprinting = false;
    public GamePhase curPhase;

    public Transform t;
    public PlayerCharacter playerCharacter;

    //weapon states 

    #region Init
    private void Start()
    {
        playerCharacter = gameObject.GetComponent<PlayerCharacter>();
        InitInGame();
    }

    public void InitInGame()
    {

    }
    #endregion

    #region FixedUpdate
    void FixedUpdate()
    {
        delta = Time.deltaTime;
        GetInput_FixedUpdate();

        switch (curPhase)
        {
            case GamePhase.inGame:
                InGame_updateStates_FixedUpdate();
                break;
            case GamePhase.inMenu:
                break;
            case GamePhase.inAdminMenu:
                break;
            default:
                break;
        }
    }



    void GetInput_FixedUpdate()
    {
        vertical = Input.GetAxis(StaticStrings.Vertical);
        horizontal = Input.GetAxis(StaticStrings.Horizontal);

       
        fire2 = Input.GetButton(StaticStrings.Fire2);
        fire3 = Input.GetButton(StaticStrings.Fire3);
       
        //fire4 = Input.GetButton(StaticStrings.Fire4);

    }

    void InGame_updateStates_FixedUpdate()
    {

        playerCharacter.playerInputs.vertical = vertical;
        playerCharacter.playerInputs.horizontal = horizontal;



    }
    #endregion

    #region Update
    void Update()
    {
        delta = Time.deltaTime;
        GetInput_Update();


        //Game States -- idea is to enable different scripts during different gamephases ingame/menu

        switch (curPhase)
        {
            case GamePhase.inGame:

                InGame_updateStates_Update();
                break;
            case GamePhase.inMenu:
                InMenu_updateStates_Update();
                break;
            case GamePhase.inAdminMenu:
                AdminMenu_updateStates_Update();
                break;
            default:
                break;
        }
    }

    //Gets input during update
    void GetInput_Update()
    {
        //getting Inputs from input manager
        fire1 = Input.GetButtonUp(StaticStrings.Fire1);
        shiftInput = Input.GetButton(StaticStrings.ShiftInput);
        lockOnButton = Input.GetButtonUp(StaticStrings.TabInput);
        LCtrl = Input.GetButtonUp(StaticStrings.LCtrl);
        //useInput = Input.GetButtonUp(StaticStrings.UseInput);
        Esc = Input.GetButtonUp(StaticStrings.Escape);
        AdminMenu = Input.GetButtonUp(StaticStrings.AdminMenu);

        rightaxis = Input.GetAxis(StaticStrings.mouseX);

        //Switching game states
        if (Esc && curPhase != GamePhase.inMenu && curPhase != GamePhase.inAdminMenu )
        {
            VariablesManager.SetGlobal("MainMenu", true);

            curPhase = GamePhase.inMenu;
            playerCharacter.playerCharacterGC.characterLocomotion.SetIsControllable(false);
            Esc = false;
        }
       


        if (AdminMenu && curPhase != GamePhase.inAdminMenu)
        {
            VariablesManager.SetGlobal("AdminMenu", true);        
            curPhase = GamePhase.inAdminMenu;
            playerCharacter.playerCharacterGC.characterLocomotion.SetIsControllable(false);
            AdminMenu = false;
        }
        




        if (shiftInput)
            shiftTimer += delta;

        //Lock On Logic
        if (lockOnButton)
        {

            islockedOn = !islockedOn;


            if (islockedOn)
            {

                VariablesManager.SetGlobal("LockOn", islockedOn);
                //check list here
                enemies.Clear();
                foreach (GameCreator.Variables.Variable enemy in playerCharacter.lockOnTargets.variables)
                {

                    enemies.Add((GameObject)enemy.Get());
                }
                if (enemies.Count == 0)
                {

                    islockedOn = false;
                    VariablesManager.SetGlobal("LockOn", islockedOn);

                    playerCharacter.PlayerMeleeComponenent.ReleaseTargetFocus();


                }
                else
                {

                    enemyIndex++;
                    if (enemyIndex > enemies.Count - 1)
                    {
                        enemyIndex = 1;
                    }
                    Debug.Log("Enemies " + enemies[enemyIndex]);
                    lockOnTransform = enemies[enemyIndex].transform;

                    playerCharacter.PlayerMeleeComponenent.SetTargetFocus(enemies[enemyIndex].GetComponent<TargetMelee>());

                }

            }
            else
            {

                lockOnTransform = null;
                playerCharacter.PlayerMeleeComponenent.ReleaseTargetFocus();
            }



        }

        if (islockedOn)
        {
            if (enemies.Count == 0)
            {
                islockedOn = false;
                VariablesManager.SetGlobal("LockOn", islockedOn);
              
                    playerCharacter.PlayerMeleeComponenent.ReleaseTargetFocus();

                
            }
            else
            {
                //TODO:refine logic for switching enemies when list is full
                //instead maybe refresh list and set it for closet when right axis is moved
                //if (rightaxis < -.8f)
                //{
                //    enemyIndex--;
                //    if (enemyIndex <= 0)
                //    {
                //        enemyIndex = 1;
                //    }

                //    lockOnTransform = enemies[enemyIndex].transform;
                //    playerCharacter.PlayerMeleeComponenent.SetTargetFocus(enemies[enemyIndex].GetComponent<TargetMelee>());
                //}

                //if (rightaxis > .8f)
                //{
                //    enemyIndex++;
                //    if (enemyIndex > enemies.Count - 1)
                //    {
                //        enemyIndex = 1;
                //    }

                //    lockOnTransform = enemies[enemyIndex].transform;
                //    playerCharacter.PlayerMeleeComponenent.SetTargetFocus(enemies[enemyIndex].GetComponent<TargetMelee>());

                //}
            }

            if (lockOnTransform == null)
            {
              
                islockedOn = false;

                playerCharacter.PlayerMeleeComponenent.ReleaseTargetFocus();


            }
            else
            {
                float v = Vector3.Distance(playerCharacter.mTransform.position, lockOnTransform.position);
                if (v > 20)
                {
                    lockOnTransform = null;
                    islockedOn = false;
                    playerCharacter.PlayerMeleeComponenent.ReleaseTargetFocus();
                }
            }

        }

       
    }

    void InGame_updateStates_Update()
    {
        //updating the states input variables by assigning them to the input here
        playerCharacter.playerInputs.fire1 = fire1;
        playerCharacter.playerInputs.fire2 = fire2;
        playerCharacter.playerInputs.fire3 = fire3;
        // playerCharacter.playerInputs.fire4 = fire4;

        //Handling rolling and sprinting
        if (shiftInput && playerCharacter.states.onGround && playerCharacter.playerStamina > 5)
        {
            shiftTimer += delta;
            if (shiftTimer > 0.5f && !playerCharacter.states.isBlocking )
            {
                if (sprinting == false)
                {
                    sprinting = true;
                    VariablesManager.SetGlobal("Sprinting", true);
                    if (playerCharacter.states.isLockedOn)
                    {
                        if (playerCharacter.PlayerMeleeComponenent.HasFocusTarget)
                        {
                            playerCharacter.PlayerMeleeComponenent.ReleaseTargetFocus();
                        }
                        islockedOn = false;

                    }
                }
                
                playerCharacter.states.isRunning = true;
            }
        }
        else
        {
            if (shiftTimer > 0.05f && shiftTimer < 0.5f && playerCharacter.states.onGround && !playerCharacter.states.isDodging && playerCharacter.playerStamina >= (playerCharacter.AttackDrainRate /2) )
            {
                
                playerCharacter.playerStats.SetAttrValue("stamina", playerCharacter.playerStamina - playerCharacter.dodgeCost, true);
                VariablesManager.SetGlobal("PlayerDodging", true);
               
            }

            shiftTimer = 0;
            if (sprinting == true)
            {
                sprinting = false;
                VariablesManager.SetGlobal("Sprinting", false);
            }


            playerCharacter.states.isRunning = false;
        }

        //Handle Crouching
        if (LCtrl && playerCharacter.states.onGround && !playerCharacter.states.isRunning)
        {
            if (!playerCharacter.states.isCrouching)
            {
                playerCharacter.states.isCrouching = true;
                VariablesManager.SetGlobal("PlayerCrouching", true);
            }
            else
            {
                playerCharacter.states.isCrouching = false;
                VariablesManager.SetGlobal("PlayerCrouching", false);
            }

        }







        playerCharacter.states.isLockedOn = islockedOn;

    }

    void InMenu_updateStates_Update()
    {
       // Debug.Log("In Menu");

        if (Esc)
        {
            Debug.Log("Escape" + Esc);
            VariablesManager.SetGlobal("MainMenu", false);
            curPhase = GamePhase.inGame;
            playerCharacter.playerCharacterGC.characterLocomotion.SetIsControllable(true);
        }








    }
    void AdminMenu_updateStates_Update()
    {

        //Debug.Log("In admin Menu");
        if (AdminMenu)
        {
            VariablesManager.SetGlobal("AdminMenu", false);
            curPhase = GamePhase.inGame;
            playerCharacter.playerCharacterGC.characterLocomotion.SetIsControllable(true);
        }
    }

    #endregion

}

public enum InputType
{
    fire1, fire2, fire3, fire4
}
public enum GamePhase
{
    inGame, inMenu, inAdminMenu
}
