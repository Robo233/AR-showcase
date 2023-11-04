using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Downalods the files needed for the game King Survival and sets up the GameObjects that are used for the game. Many things are created during runtime to save space on the disk
/// </summary>

public class KingSurvivalLoader : GameLoader
{
    GameObject Bullet;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject Laser;
    [SerializeField] GameObject NoInternetContainer;
    [SerializeField] GameObject LoadingText;

    [SerializeField] Transform GroundPlaneStage;

    [SerializeField] Button shootButton;

    [SerializeField] Color[] MaterialColors;

    /*
    The materials are created during runtime to save space on the disk
    The material colors are for the following objects:
    0 EnemySkin
    1 EyeBall
    2 Gold
    3 KingEye
    4 KingMouth
    5 KingSkin
    6 Border
    7 Bullet
    8 CannotShoot
    9 EnemyIdentified
    10 Laser
    */
    
    Material[] Materials;

    [SerializeField] Shader StandardShader;

    [SerializeField] GameStarter gameStarter;
    [SerializeField] EnemyGenerator enemyGenerator;
    [SerializeField] Run run;

    void Start() // Contains the setting up of the GameObjects, that will be created during runtime
    {
        path = Application.persistentDataPath + "/Games/KingSurvival/";
        Directory.CreateDirectory(path);
        
        Materials = new Material[MaterialColors.Length];
        for(int i=0;i<MaterialColors.Length;i++)
        {
            Materials[i] = new Material(StandardShader);
            Materials[i].color = MaterialColors[i];
        }

        Materials[2].SetFloat("_Metallic", 1);

        LoadingText.SetActive(true);
        
        if(!SceneLoader.isVuforiaInitialized)
        {
            StartCoroutine(VuforiaInitializationCoroutine());
            VuforiaApplication.Instance.OnVuforiaInitialized += OnVuforiaInitialized;
        }
        else
        {
            StartGameDownload("King Survival");
            
        }

        Actions = new Action<GameObject>[] // Here are the 3D models that are used in the game, these are downloaded from a server, and below are the lines of code that set up the GameObjects
        {
            Border => 
            {
                Border.name = "Border";
                Border.transform.SetParent(GroundPlaneStage);
                Border.transform.localEulerAngles = new Vector3(-90, 0, 0);
                Border.transform.localScale = new Vector3(120, 120, 40);
                AddComponents(Border, typeof(MeshRenderer));
                Border.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materials[6];
                Border.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
                Border.transform.GetChild(0).tag = "Border";
            },
            Bullet =>
            {
                Bullet.name = "Bullet";
                AddComponents(Bullet, typeof(MeshRenderer), typeof(MeshDestroyer), typeof(BulletProjectile));
                Rigidbody rigidbody = Bullet.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                BoxCollider boxCollider = Bullet.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                Bullet.SetActive(false);
                Bullet.transform.localPosition = new Vector3(-0.02477643f, 0.0802f, 0.0707f);
                Bullet.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                this.Bullet = Bullet;
                Bullet.GetComponent<MeshRenderer>().material = Materials[7];
                Bullet.transform.GetChild(0).GetComponent<MeshRenderer>().material = Materials[7];
                MeshFilter meshFilter = Bullet.AddComponent<MeshFilter>();
                GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                meshFilter.sharedMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
                Destroy(tempSphere);
            },
            Enemy =>
            {
                Enemy.name = "Enemy";
                Enemy.transform.localPosition = new Vector3(-0.14f,0,0);
                Enemy.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Enemy.SetActive(false);
                Enemy.transform.GetChild(0).tag = "Enemy";
                Enemy.transform.GetChild(Enemy.transform.childCount-1).gameObject.AddComponent<MeshCollider>();
                GameObject tempCapsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                Debug.Log("tempCapsule: " + tempCapsule.name);
                MeshFilter meshFilter = Enemy.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = tempCapsule.GetComponent<MeshFilter>().sharedMesh;
                Destroy(tempCapsule);    
                AddComponents(Enemy, typeof(MeshRenderer), typeof(EnemyController), typeof(MeshDestroyer));
                Enemy.GetComponent<MeshRenderer>().material = Materials[0];
                CapsuleCollider capsuleCollider = Enemy.AddComponent<CapsuleCollider>();
                capsuleCollider.height = 2;
                capsuleCollider = Enemy.transform.GetChild(0).gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = 2;
                enemyGenerator.enemyModel = Enemy;
                Enemy.transform.GetChild(Enemy.transform.childCount-1).GetComponent<MeshRenderer>().material = Materials[0];
                Enemy.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Materials[4];
                Enemy.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Materials[1];
                Enemy.transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().material = Materials[3];
                Enemy.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Materials[1];
                Enemy.transform.GetChild(2).GetChild(1).GetComponent<MeshRenderer>().material = Materials[3];
            },
            King =>
            {
                King.name = "King";
                King.transform.SetParent(GroundPlaneStage);
                King.transform.localPosition = new Vector3(0, 0, 0);
                King.transform.localEulerAngles = new Vector3(0, 90, 0);
                King.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                AddComponents(King, typeof(MeshRenderer), typeof(MeshCollider));    
                King.transform.GetChild(King.transform.childCount-1).GetComponent<MeshRenderer>().material = Materials[5];
                King.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Materials[2];    
                King.transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>().material = Materials[4];
                King.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Materials[1];
                King.transform.GetChild(2).GetChild(1).GetComponent<MeshRenderer>().material = Materials[3];
                King.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = Materials[1];
                King.transform.GetChild(3).GetChild(1).GetComponent<MeshRenderer>().material = Materials[3];
                PlayerMovement playerMovement = King.AddComponent<PlayerMovement>();
                CharacterController characterController = King.AddComponent<CharacterController>();
                characterController.stepOffset = 0;
                characterController.skinWidth = 0.0001f;
                characterController.minMoveDistance = 0.001f;
                characterController.radius = 0.51f;
                playerMovement.Bullet = Bullet.transform;
                playerMovement.emptyGunShot = AudioSources[0];
                playerMovement.gunShot = AudioSources[2];
                playerMovement.gameStarter = gameStarter;
                gameStarter.playerMovement = playerMovement;
                run.playerMovement = playerMovement;
                enemyGenerator.player = King;
                shootButton.onClick.AddListener(playerMovement.Shoot);
                playerMovement.shootButtonImage = shootButton.gameObject.GetComponent<UnityEngine.UI.Image>();
                GameObject laser = Instantiate(Laser, King.transform.GetChild(1).gameObject.transform);
                laser.name = "Laser";
                Laser LaserScript = laser.AddComponent<Laser>();
                LaserScript.cannotShootMaterial = Materials[8];
                LaserScript.enemyIdentifiedMaterial = Materials[9];
                LaserScript.laserMaterial = Materials[10];
                LaserScript.playerMovement = playerMovement;

            }
        };
        
    }

    IEnumerator VuforiaInitializationCoroutine()
    {
        yield return null;
        Debug.Log("Loading vuforia");
        VuforiaApplication.Instance.Initialize(); 

    }

    void OnVuforiaInitialized(VuforiaInitError error)
    {
        Debug.Log(error);
        SceneLoader.isVuforiaInitialized = true;
        StartGameDownload("King Survival");
    }

    protected override void GenerationIsOver()
    {
        loadingScreen.SetActive(false);
        PlayerPrefs.SetInt("King Survival", 0);
        if(GameStarter.isRestarted)
        {
            gameStarter.StartModelDetectionOrShowTutorial1();
        }
        else
        {
            mainMenu.SetActive(true);
        }
        
    }

    protected override void ToggleInternetConnectionMenu(bool isInternetWorking)
    {
        NoInternetContainer.SetActive(!isInternetWorking);
        LoadingText.SetActive(isInternetWorking);
    }

}