using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Fusion;

public class Damage_Player : NetworkBehaviour
{
    //Has Interacted varibales
    private bool GotHit;
    private float ImmortalDurationAfterHit = 2f;
    public bool IsImmortal => _timer.ExpiredOrNotRunning(Runner) == false;

    //Hits
    [Networked]
    private int HitCount { get; set; }
    private int VisibleHitCount;
    [Networked]
    private Vector2 HitDirection { get; set; }

    [Header("Components")]
    private Vector3 originalPosition; // Start pos
    private Quaternion originalRotation; // Start rot
    private GameObject player2;
    private GameObject player2Reference;

    [Header("Collider Tags")]

    [Header("Cam Shake Settings")]
    public Transform trampoline;
    public Transform cameraTransform; // Cam transform[Header("Camera Shake")]
    public float shakeDuration = 0.3f; // shake Duration
    public float shakestrength = 1f; // shake strength

    [Header("Hard Camera Shake")]
    public float shakeComboStrength = 1f; // Combo shake Strength
    public float shakeComboDuration = 1f; // Combo shake Duration

    [Header("Recoil Settings")]
    public float recoilForce = 1000f; // recoil force
    public float recoilDuration = 0.1f; // recoil duration

    public float comboRecoilForceX = 400f;
    public float comboRecoilForceY = 100f;

    [Header("Damages")]
    public float normalDamage, ultimateDamage, laserDamage, smokeDamage,comboDamage;
    public GameObject smoke_end;
    public static bool isRecoiling = false;

    [Networked]
    public NetworkBool canUltimate { get; set; } = false;
    private Collider2D _hitCollider;
    private Collider2D _collider;
    [SerializeField]private GameObject weapon;

    private PVP_PlayerMovement _playerMovement = null;
    private Rigidbody2D rb = null;
    private Health_Playerr health = null;
    [Networked] private TickTimer _timer { get; set; }

    public override void Spawned()
    {
        _playerMovement = GetComponent<PVP_PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health_Playerr>();
        //hasInteracted = false;
        _collider = weapon.GetComponent<BoxCollider2D>();

        if (HasStateAuthority)
        {
            _timer = TickTimer.CreateFromSeconds(Runner, ImmortalDurationAfterHit);
        }
        VisibleHitCount = HitCount;
    }

    public override void FixedUpdateNetwork()
    {

        //DetectCollisions();
        //if (hasInteracted && _timer.ExpiredOrNotRunning(Runner))
        //{
        //    ResetInteraction();
        //}
    }

    public void Damage(string referencetag)
    {
        if (referencetag == "bullet")// && !hasInteracted)
        {
            Audio_Manager.instance.PlaySound(Audio_Manager.instance.ultiDmg);
            DeductHealth(ultimateDamage);
            //Recoil Combo
        }

        else if (referencetag == "smoke")// && !hasInteracted)
        {
            DeductHealth(smokeDamage);
            //Audio_Manager.instance.PlaySound(Audio_Manager.instance.ultiDmg);

            //Instantiate(smoke_end, transform.position, Quaternion.identity); //Smoke end spawn
            //RecoilCombo 
        }

        else if (referencetag == "laser")// && !hasInteracted)
        {
            DeductHealth(laserDamage);
            //Audio_Manager.instance.PlaySound(Audio_Manager.instance.ultiDmg);
            //RecoilCombo
        }
    }

    public void WeaponCollisions(float damageValue, Vector2 direction, bool comboAttack = false)
    {
        /*if (collision.gameObject.CompareTag(tag) && collision.transform.root != transform.root && !hasInteracted) //If he touched the gun and hasInteracted=false ise
        {
            if (_playerMovement.isBlocking) //If the player blocks
            {
                Dodge();
                cameraTransform.DOShakePosition(shakeDuration, shakestrength) //Cam shake
                  .OnComplete(() => ResetCameraPosition());
            }
            else
            {
                //player2Reference.GetComponent<Player2_Movements>().attackAmount += 1; //Player attack amount //Take normaldmg
                DeductHealth(normalDamage);

                //cameraTransform.DOShakePosition(shakeDuration, shakestrength) //Cam shake
                //    .OnComplete(() => ResetCameraPosition());

                //if (transform.position.x < player2Reference.transform.position.x && !isRecoiling)
                //    RecoilObject(Vector2.left);
                //else if (transform.position.x > player2Reference.transform.position.x && !isRecoiling)
                //    RecoilObject(Vector2.right); //Right recoil
            }

            //if(player2Reference.GetComponent<Player2_Movements>().comboDamage) //COMBO DAMAGE
            //{
            //    Audio_Manager.instance.PlaySound(Audio_Manager.instance.comboDmg);

            //    player2Reference.GetComponent<Player2_Movements>().ComboReset(); // Rival resets her combo after incurring COMBO DAMAGE
            //    player2Reference.GetComponent<Player2_Movements>().comboText.text = ("COMBO");

            //    cameraTransform.DOShakePosition(shakeComboDuration, shakeComboStrength).OnComplete(() => ResetCameraPosition());

            //    if (transform.position.x < player2Reference.transform.position.x && !isRecoiling)
            //    {
            //        Debug.Log("RecoilCombo to the left");
            //        RecoilCombo(Vector2.left, Vector2.up);
            //    }
            //    else if (transform.position.x > player2Reference.transform.position.x && !isRecoiling)
            //    {
            //        Debug.Log("RecoilCombo to the right");
            //        RecoilCombo(Vector2.right, Vector2.up);
            //    }
            //}
            hasInteracted = true;
            StartCoroutine(ResetInteractionCooldown());
        }
        _hitCollider = Runner.GetPhysicsScene2D().OverlapBox(weapon.transform.position, _collider.bounds.size * 0.5f, 0, LayerMask.GetMask("MultPlayer"));
        if (_hitCollider != default)
        { 
            if (_hitCollider.tag.Equals("Player") && _hitCollider.transform.root != transform.root && !hasInteracted && _playerMovement.GetIsAttacking())
            {
                Debug.Log("Hit collider is Enemy");
                if (_playerMovement.isBlocking) //If the player blocks
                {
                    Dodge();
                    cameraTransform.DOShakePosition(shakeDuration, shakestrength) //Cam shake
                      .OnComplete(() => ResetCameraPosition());
                }else
                {
                    //Increasing the attack amount of the other player
                    _hitCollider.GetComponent<PVP_PlayerMovement>().attackAmount += 1;
                    Debug.Log("Player hit" + _hitCollider.gameObject.name);
                    if (_hitCollider.GetComponent<PVP_PlayerMovement>().comboDamage)
                        DeductHealth(comboDamage);
                    else
                        DeductHealth(normalDamage);
                }
                hasInteracted = true;
                _timer = TickTimer.CreateFromSeconds(Runner, _resetCollision);
            }
        }*/

        if (_playerMovement.GetIsBlocking()) //If the player blocks
        {
            Dodge();
            //cameraTransform.DOShakePosition(shakeDuration, shakestrength) //Cam shake
            //  .OnComplete(() => ResetCameraPosition());
        }
        else
        {
            if (IsImmortal)
                return;
            DeductHealth(damageValue);
            if (comboAttack)
                HitCount += 2;
            else
                HitCount++;
            HitDirection = -direction;
            //Recoil
        }
    }

    private void DeductHealth(float damage)
    {
        if (Object.HasStateAuthority == false) return;
        _timer = TickTimer.CreateFromSeconds(Runner, ImmortalDurationAfterHit);
        health.TakeDamage(damage); //take ulti damage
    }

    public override void Render()
    {
        if (VisibleHitCount < HitCount)
        {
            if(VisibleHitCount + 2 == HitCount)
                RecoilCombo();
            // Network hit counter changed in FUN, play damage effect.
            PlayDamageEffect();
        }

        // Sync network hit counter with local.
        VisibleHitCount = HitCount;

    }

    private void PlayDamageEffect()
    {
        rb.AddForce(HitDirection * recoilForce, ForceMode2D.Force);
    }
    private void ResetCameraPosition() //Cam shake end
    {
        cameraTransform.localPosition = originalPosition;
        cameraTransform.localRotation = originalRotation;
    }
    private void RecoilObject(Vector2 direction)  // NORMAL RECOIL
    {
        rb.AddForce(HitDirection * recoilForce, ForceMode2D.Force);
        StartCoroutine(StopRecoil());
    }
    private void RecoilCombo() // COMBO RECOIL
    {
        rb.AddForce(HitDirection * comboRecoilForceX, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * comboRecoilForceY, ForceMode2D.Impulse);

        //StartCoroutine(ResetComboRecoil());
    }

    private IEnumerator StopRecoil() //RECOIL END
    {
        isRecoiling = true;
        yield return new WaitForSeconds(recoilDuration);
        isRecoiling = false;
    }
    public void Dodge() //Blocking attacks with block 
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.dodge);

        if (Object.HasStateAuthority == false) return;

        if (health.currentDodge < health.maxDodge)
        {
            health.AddToDodge(2.0f);
        }
        if(health.currentDodge == health.maxDodge)
        {
            // Add a print statement for can use ultimate attack
            canUltimate = true;
        }
    }
}
