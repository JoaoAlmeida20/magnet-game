using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public float fieldStrength;
    public bool isRed;
    public bool isSquare;
    public Transform squarePoint1, squarePoint2;

    SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isRed) {
            spriteRenderer.color = new Color(0.85f, 0.2f, 0.1f, 0.25f);
        }
    }

    void OnTriggerStay2D(Collider2D collider) {
        MagneticObject colliderMagneticObject;
        if (collider.gameObject.TryGetComponent<MagneticObject>(out colliderMagneticObject)) {
            if (colliderMagneticObject.isOn) {
                bool isAttraction = isRed ^ colliderMagneticObject.isRed;
                colliderMagneticObject.isSuck = true;

                Rigidbody2D colliderRigidbody2d = collider.GetComponent<Rigidbody2D>();
                colliderRigidbody2d.gravityScale = 0;

                Vector2 suckDirection = isSquare ? (Vector2) (squarePoint1.position - squarePoint2.position) : (Vector2) (transform.position - collider.transform.position);
                Vector2 suckForce = suckDirection.normalized * fieldStrength * colliderMagneticObject.magnetStrength;
                if (colliderMagneticObject.isPlayer) {
                    suckForce += new Vector2(colliderMagneticObject.player.horizontal, colliderMagneticObject.player.vertical) * colliderMagneticObject.diStrength;
                }

                float angle = Mathf.Atan2(suckDirection.x, suckDirection.y) * Mathf.Rad2Deg;
                angle = Mathf.DeltaAngle(angle, 0);

                if (!isAttraction) {
                    suckForce *= -1;
                }

                colliderRigidbody2d.AddForce(suckForce);
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                collider.transform.rotation = Quaternion.Slerp(collider.transform.rotation, rotation, colliderMagneticObject.rotationSpeed * Time.deltaTime);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        MagneticObject colliderMagneticObject;
        if (collider.gameObject.TryGetComponent<MagneticObject>(out colliderMagneticObject)) {
            colliderMagneticObject.isSuck = false;
        }
    }
}
