using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trampoline : MonoBehaviour
{
    [Header("Recoil")]
    public float comboRecoilForceX = 100f;
    public float comboRecoilForceY = 100f;

    public float recoilDuration = 0.1f;
    public static bool isRecoiling = false;

    public Transform player;
    private void OnCollisionEnter2D(Collision2D collision) //throw player and rival
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            player = collision.gameObject.transform;
            if (otherRb != null)
            {
                if ((transform.position.x) > (player.transform.position.x))
                    Recoil(Vector2.left, Vector2.up, comboRecoilForceX, comboRecoilForceY, otherRb);

                if ((transform.position.x) < (player.transform.position.x))
                    Recoil(Vector2.right, Vector2.up, comboRecoilForceX, comboRecoilForceY, otherRb);
            }
        }
    }
    private void Recoil(Vector2 xDirection, Vector2 yDirection, float xForce, float yForce, Rigidbody2D otherRb) // HARD RECOIL
    {
        otherRb.AddForce(xDirection * xForce, ForceMode2D.Force);
        otherRb.AddForce(yDirection * yForce, ForceMode2D.Force);

        StartCoroutine(StopRecoil());
    }
    private IEnumerator StopRecoil() //RECOIL END
    {
        isRecoiling = true;
        yield return new WaitForSeconds(recoilDuration);
        isRecoiling = false;
    }
}
