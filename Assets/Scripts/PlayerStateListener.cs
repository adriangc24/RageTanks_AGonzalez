using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{
    public float playerWalkSpeed = 3f;
    public GameObject playerRespawnPoint = null;
    public Transform bulletSpawnTransform;
    public GameObject bulletPrefab = null;
    public GameObject bulletPrefab2 = null;

    private Animator playerAnimator = null;
    private PlayerStateController.playerStates previousState;
    private PlayerStateController.playerStates currentState;
    public float playerJumpForceVertical = 1f;
    public float playerJumpForceHorizontal = 1f;
    private bool playerHasLanded = true;
    //Aquest mètode de MonoBehaviour s'executa cada vegada que s'activa l'objecte associat a l'script.
    //L'objecte s'apunta a escoltar l'event onStateChange: afegeix la funcio onStateChange a la llista de
    //handlers (manegadors) de l'event PlayerStateController.onStateChange. Amb aixo, cada vegada que
    //es generi un event PlayerStateController.onStateChange, el sistema passara el control a la funcio
    //onStateChange (i, sequencialment, a totes les funcions que s'hagin afegit a la llista de handlers
    //d'aquest event)
    void OnEnable()
    {
        PlayerStateController.onStateChange += onStateChange;
    }
    //Aquest mètode de MonoBehaviour s'executa cada vegada que es desactiva l'objecte associat a l'script.
    //Es deixa d'escoltar l'event onStateChange
    void OnDisable()
    {
        PlayerStateController.onStateChange -= onStateChange;
    }
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingWeapon] = 0.0f;
        PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingMortar] = 0.0f;

    }
    void LateUpdate()
    {
        onStateCycle();
    }
    // Processar l'estat en cada cicle
    void onStateCycle()
    {
        // Guardar l'actual localScale de l'bjecte (és al component Transform de l'objecte)
        Vector3 localScale = transform.localScale;
        transform.localEulerAngles = Vector3.zero;
        switch (currentState)
        {
            case PlayerStateController.playerStates.idle:
                break;
            case PlayerStateController.playerStates.left:
                //moure cap a l'esquerra modificant la posició
                transform.Translate(new Vector3((playerWalkSpeed * -1.0f) * Time.deltaTime, 0.0f, 0.0f));
                if (localScale.x > 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;
                }
                break;
            case PlayerStateController.playerStates.right:
                //moure cap a la dreta modificant la posició
                transform.Translate(new Vector3(playerWalkSpeed * Time.deltaTime, 0.0f, 0.0f));
                if (localScale.x < 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;
                }
                break;
            case PlayerStateController.playerStates.jump:
                onStateChange(PlayerStateController.playerStates.landing);
                break;
            case PlayerStateController.playerStates.landing:
                break;
            case PlayerStateController.playerStates.falling:
                break;
            case PlayerStateController.playerStates.kill:
                onStateChange(PlayerStateController.playerStates.resurrect);
                break;
            case PlayerStateController.playerStates.resurrect:
                onStateChange(PlayerStateController.playerStates.idle);
                break;
        }
    }
    // onStateChange es crida sempre que canvia l'estat del player

    public void onStateChange(PlayerStateController.playerStates newState)
    {
        // Si l'estat actual i el nou són el mateix, no cal fer res
        if (newState == currentState)
            return;
        // Comprovar que no hi hagi condicions per abortar l'estat
        if (checkIfAbortOnStateCondition(newState))
            return;

// Comprovar que el pas de l'estat actual al nou estat està permès. Si no està, no es continua.
        
    if (!checkForValidStatePair(newState))
            return;

        // Realitzar les accions necessàries en cada cas per canviar l'estat.
        // De moment només es gestionen els estats idle, right i left
        switch (newState)
        {
            case PlayerStateController.playerStates.idle:
                playerAnimator.SetBool("Walking", false);
                break;
            case PlayerStateController.playerStates.left:
                playerAnimator.SetBool("Walking", true);
                break;
            case PlayerStateController.playerStates.right:
                playerAnimator.SetBool("Walking", true);
                break;
            case PlayerStateController.playerStates.jump:
                Debug.Log(newState);
                if (playerHasLanded)
                {
                    // jumpDirection determina si el salt es a la dreta, esquerra o vertical 
                    float jumpDirection = 0.0f;
                    if (currentState == PlayerStateController.playerStates.left)
                        jumpDirection = -1.0f;
                    else if (currentState == PlayerStateController.playerStates.right)
                        jumpDirection = 1.0f;
                    else jumpDirection = 0.0f;
                    // aplicar la força per fer el salt
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));
                    //indicar que el Player esta saltant en l'aire
                    playerHasLanded = false;
                }

                break;
            case PlayerStateController.playerStates.landing:
                playerHasLanded = true;
                break;
            case PlayerStateController.playerStates.falling:
                break;
            case PlayerStateController.playerStates.kill:
                break;
            case PlayerStateController.playerStates.resurrect:
                //posicio: la de PlayerRespawnPoint
                transform.position = playerRespawnPoint.transform.position;
                transform.rotation = Quaternion.identity; //rotacio: cap
                GetComponent<Rigidbody2D>().velocity = Vector2.zero; //velocitat lineal: zero
                break;
            case PlayerStateController.playerStates.firingWeapon:
                // Construir l'objecte bala a partir del Prefab
                GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
                // Establir la posicio inicial de la bala creada
                //(la posicio de BulletSpawnTransform)
                newBullet.transform.position = bulletSpawnTransform.position;

                // Agafar el component PlayerBulletController de la bala
                //que s'ha creat
                // Establir temps a partir del qual es pot tornar a disparar
                PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingWeapon] = Time.time + 0.25f;
                PlayerBulletController bullCon = newBullet.GetComponent<PlayerBulletController>();
                // Assignar a l'atribut playerObject de l'script
                //PlayerBulletController el Player 
                bullCon.playerObject = gameObject;
                // Invocar metode que dispara la bala 
                bullCon.launchBullet();
                // Despres de disparar, tornar a l'estat previ.
                onStateChange(currentState);
                break;
            case PlayerStateController.playerStates.firingMortar:
                // Construir l'objecte bala a partir del Prefab
                GameObject newBullet2 = (GameObject)Instantiate(bulletPrefab2);
                // Establir la posicio inicial de la bala creada
                //(la posicio de BulletSpawnTransform)
                newBullet2.transform.position = bulletSpawnTransform.position;

                // Agafar el component PlayerBulletController de la bala
                //que s'ha creat
                // Establir temps a partir del qual es pot tornar a disparar
                PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingMortar] = Time.time + 0.25f;
                PlayerBulletController2 bullCon2 = newBullet2.GetComponent<PlayerBulletController2>();
                // Assignar a l'atribut playerObject de l'script
                //PlayerBulletController el Player 
                bullCon2.playerObject = gameObject;
                // Invocar metode que dispara la bala 
                bullCon2.launchBullet();
                // Despres de disparar, tornar a l'estat previ.
                onStateChange(currentState);
                break;


        }
        // Guardar estat actual com a estat previ
        previousState = currentState;
        // Assignar el nou estat com a estat actual del player
        currentState = newState;
    }
    // Comprovar si es pot passar al nou estat des de l'actual.
    // Es tracten diversos estats que encara no estan implementats, perquè el
    // codi sigui més ilustratiu
    bool checkForValidStatePair(PlayerStateController.playerStates newState)
    {
        bool returnVal = false;
        // Comparar estat actual amb el candidat a nou estat.
        switch (currentState)
        {
            case PlayerStateController.playerStates.idle:
                // Des de idle es pot passar a qualsevol altre estat
                returnVal = true;
                break;
            case PlayerStateController.playerStates.left:
                // Des de moving left es pot passar a qualsevol altre estat
                returnVal = true;
                break;
            case PlayerStateController.playerStates.right:
                // Des de moving right es pot passar a qualsevol altre estat
                returnVal = true;
                break;
            case PlayerStateController.playerStates.jump:
                // Des de Jump només es pot passar a landing o a kill.
                if (newState == PlayerStateController.playerStates.landing || newState == PlayerStateController.playerStates.kill
                    || newState == PlayerStateController.playerStates.firingWeapon || newState == PlayerStateController.playerStates.firingMortar
)
                {
                    returnVal = true;
                }
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.landing:
                // Des de landing només es pot passar a idle, left o right.
                if (
                newState == PlayerStateController.playerStates.left
                || newState == PlayerStateController.playerStates.right
                || newState == PlayerStateController.playerStates.idle
                || newState == PlayerStateController.playerStates.firingWeapon
                || newState == PlayerStateController.playerStates.firingMortar

        )
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.falling:
                // Des de falling només es pot passar a landing o a kill.
                if (
                newState == PlayerStateController.playerStates.landing
                || newState == PlayerStateController.playerStates.kill
                || newState == PlayerStateController.playerStates.firingWeapon
                || newState == PlayerStateController.playerStates.firingMortar


                )
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.kill:
                // Des de kill només es pot passar a resurrect
                if (newState == PlayerStateController.playerStates.resurrect)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.resurrect:
                // Des de resurrect només es pot passar a Idle
                if (newState == PlayerStateController.playerStates.idle)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.playerStates.firingWeapon:
                returnVal = true;
                break;
            case PlayerStateController.playerStates.firingMortar:
                returnVal = true;
                break;

        }
        return returnVal;
    }
    // Aquesta funció comprova si hi ha algun motiu que impedeixi passar al nou estat.
    // De moment no hi ha cap motiu per cancel·lar cap estat.
    bool checkIfAbortOnStateCondition(PlayerStateController.playerStates newState)
    {
        bool returnVal = false;
        switch (newState)
        {
            case PlayerStateController.playerStates.idle:
                break;
            case PlayerStateController.playerStates.left:
                break;
            case PlayerStateController.playerStates.right:
                break;
            case PlayerStateController.playerStates.jump:
                break;
            case PlayerStateController.playerStates.landing:
                break;
            case PlayerStateController.playerStates.falling:
                break;
            case PlayerStateController.playerStates.kill:
                break;
            case PlayerStateController.playerStates.resurrect:
                break;
            case PlayerStateController.playerStates.firingWeapon:
                // Ignorar si no ha passat prou temps
                if (PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingWeapon] > Time.time)
                {
                    returnVal = true;
                }
                break;
            case PlayerStateController.playerStates.firingMortar:
                // Ignorar si no ha passat prou temps
                if (PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingMortar] > Time.time)
                {
                    returnVal = true;
                }
                break;

        }
        // Retornar True vol dir 'Abort'. Retornar False vol dir 'Continue'.
        return returnVal;
    }

    //Metode cridat en caure de la plataforma i col.lisionar amb DeathTrigger
    public void hitDeathTrigger()
    {
        onStateChange(PlayerStateController.playerStates.kill);
    }
    public void hitByCrusher()
    {
        onStateChange(PlayerStateController.playerStates.kill);
    }
}