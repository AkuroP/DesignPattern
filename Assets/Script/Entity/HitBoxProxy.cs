using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HitBoxProxy : MonoBehaviour
    {
        [SerializeField, Required] Entity _target;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            _target.DamageHitBox(collider, this.gameObject.name);
        }
    }
}
