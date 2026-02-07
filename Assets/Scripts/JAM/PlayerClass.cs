using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerClass : MonoBehaviour
{
    PlayerInput input;
    Rigidbody rb;
	Animator anim;
	Collider col;

	public int PV;

	public static PlayerClass instance;

	#region stuff

	[Header("Parametre des mouvement")]
	[SerializeField] private float moveSpeed = 5f;

	[Header("Parametre mode Revert")]
	[SerializeField] private float recordWindowSeconds = 10f;

	[Header("parametre etat")]
	[SerializeField] private float knockbackUntil;

	[SerializeField] private bool inputLocked;

	[Header("Input action")]
	[SerializeField] private string actionMapName = "Gameplay";
	[SerializeField] private string moveActionName = "Move";
	[SerializeField] private string punchActionName = "Punch";
	[SerializeField] private string shootActionName = "Shoot";

	[SerializeField] private string swordActionName = "Sword";

	[Header("Punch")]
	[SerializeField] private float CoolDown = 0.5f;
	[SerializeField] private float punchLogInterval = 0.2f;
	private bool isPunching = false;
	private float punchTimer = 0f;
	private float punchLogTimer = 0f;
	[SerializeField] private float PunchDistance = 3f;
	[SerializeField] private float punchEaseStart = 0.75f;
	private float _punchLastS;
	private Coroutine punchRoutine;

	[Header("Sword")]
	[SerializeField] private GameObject swordPrefab;
	[SerializeField] private float swordLockSeconds = 0.5f;
	[SerializeField] private float swordSpawnDistance = 1.0f;
	[SerializeField] private float swordKnockbackForce = 8f;
	[SerializeField] private float swordKnockbackMult = 1f;
	[SerializeField] private float swordCooldown = 0.35f;

	private InputAction swordAction;


	private PlayerInput playerInput;
	private InputAction moveAction;
	private InputAction punchAction;
	private InputAction shootAction;

	#endregion

	public void TakeDamage(int Damage)
	{
		if (PV - Damage <= 0) { StartCoroutine(Death()); }
		PV -= Damage;
		//StartCoroutine(InvincibilityFrames());
	}

	public IEnumerator Death()
	{
		BlackScreen.Fade(1f);
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
	}
	private void Awake()
    {
		instance = this;
        rb = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		col = GetComponent<Collider>();
		anim = GetComponentInChildren<Animator>();
	}
	private void OnEnable()
	{
		if (rb != null) rb.useGravity = false;

		var actions = playerInput.actions;
		if (!string.IsNullOrEmpty(actionMapName))
			actions.FindActionMap(actionMapName, throwIfNotFound: true);

		moveAction = actions[moveActionName];
		punchAction = actions[punchActionName];
		shootAction = actions[shootActionName];
		swordAction = actions[swordActionName];

		swordAction.performed += OnSwordPressed;
		swordAction.Enable();


		punchAction.performed += OnPunchPressed;
		shootAction.performed += OnShootPressed;

		moveAction.Enable();
		punchAction.Enable();
		shootAction.Enable();
	}

	private void OnDisable()
	{


		if (punchAction != null)
		{
			punchAction.performed -= OnPunchPressed;
		}

		if (shootAction != null)
		{
			shootAction.performed -= OnShootPressed;
		}

		if (swordAction != null)
			swordAction.performed -= OnSwordPressed;


	}



    private void Update()
	{

		if (isPunching)
		{
			punchTimer -= Time.deltaTime;
			punchLogTimer += Time.deltaTime;

			if (punchLogTimer >= punchLogInterval) { punchLogTimer = 0f; Debug.Log($"[PlayerMovement] Punch ACTIVE ({Mathf.Max(0f, punchTimer):F2}s left)"); }
			if (punchTimer > 0f) return;

			isPunching = false;
			Debug.Log("[PlayerMovement] Punch END");
		}

		if (inputLocked) return;


		Vector2 raw = moveAction.ReadValue<Vector2>();
		float mag2 = raw.sqrMagnitude;

		float dt = Time.deltaTime;
		float speed = moveSpeed ;

		rb.AddForce(raw.x*speed, 0,raw.y*speed);
		if (raw.magnitude > 0.1f) anim.SetBool("Moving",true);
		else anim.SetBool("Moving", false);
		transform.rotation = Quaternion.LookRotation(new Vector3(raw.x, 0, raw.y));

	}


	void OnSwordPressed(InputAction.CallbackContext ctx)
	{
		anim.SetTrigger("Attacking");
		StartCoroutine(SwordSwipe());
	}

	private IEnumerator SwordSwipe()
	{
		Vector3 SpawnOffset = transform.forward * swordSpawnDistance;
		GameObject obj = Instantiate(swordPrefab, transform.position + SpawnOffset, Quaternion.identity, transform);
		obj.transform.rotation = transform.rotation;
		var hitbox = obj.GetComponent<SwordHitbox>();
		if (hitbox != null)
			hitbox.ServerInit(obj.transform.forward, 20f, 1);
		yield return new WaitForSeconds(0.3f);
		Destroy(obj);
	}

	private IEnumerator InvincibilityFrames()
	{
		col.enabled = false;
		yield return new WaitForSeconds(1f);
		col.enabled = true;

	}
	void OnPunchPressed(InputAction.CallbackContext ctx)
	{
		rb.AddForce(transform.forward.normalized * 150f,ForceMode.Impulse);
	}

	void OnShootPressed(InputAction.CallbackContext ctx)
	{

	}

    private void OnDestroy()
    {
        
    }
}
