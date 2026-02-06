using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClass : MonoBehaviour
{
    PlayerInput input;
    Rigidbody rb;
	Collider col;

	public int PV;

	public static PlayerClass instance;

	#region stuff

	[Header("Parametre des mouvement")]
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float runMult = 2f;
	[SerializeField] private float runRampUpTime = 0.25f;
	[SerializeField] private float runRampDownTime = 0.20f;
	private float _runMultCurrent = 1f;
	private Coroutine _runLerpRoutine;
	[SerializeField] private float midAirMoveSpeed = 3f;
	private float inputDeadzone = 0.2f;
	[HideInInspector] public bool isMenuOn;

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
		if (PV - Damage <= 0) { Death(); }
		PV -= Damage;
	}

	public void Death()
	{

	}
	private void Awake()
    {
		instance = this;
        rb = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		col = GetComponent<Collider>();
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
		transform.rotation = Quaternion.LookRotation(new Vector3(raw.x,0,raw.y));

	}


	void OnSwordPressed(InputAction.CallbackContext ctx)
	{
		StartCoroutine(SwordSwipe());
	}

	private IEnumerator SwordSwipe()
	{
		Vector3 SpawnOffset = rb.linearVelocity.normalized * swordSpawnDistance;
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
