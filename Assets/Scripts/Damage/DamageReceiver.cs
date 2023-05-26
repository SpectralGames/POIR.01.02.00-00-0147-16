using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Enemy enemy = null;
    //[SerializeField] private bool stopTargetAfterAttack = false;
    [SerializeField] private float damagaModyfier = 1f;

    private List<DamageReceiver> damageReceivers = new List<DamageReceiver>();
    private bool ignoreDamage;

    [Space]
    /// For debug only. 
    private float counter = 0;
    private Collider receverCollider = null;
    [SerializeField] private float hitDisplayTime = 3f;

    public static Transform Projectile { get; set; }

    private void Awake()
    {
        damageReceivers.AddRange(enemy.GetComponentsInChildren<DamageReceiver>());
        damageReceivers.Remove(this);
        receverCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        ignoreDamage = false;    
    }

    public void TakeDamage(float damage, bool stopTargetAfterAttack, EAttackSideEffect attackSideEffect, string attackTypeName, int attackLevel, bool critted)
    {
        if(ignoreDamage)
        {
            ignoreDamage = false;
        }
        else
        {
            foreach (var item in damageReceivers)
                item.ignoreDamage = true;
            var closestDamateReciver = this;
            if(Projectile != null)
            {
                var currentDistance = Vector3.Distance(closestDamateReciver.transform.position, Projectile.position);
                foreach (var item in damageReceivers)
                {
                    var distance = Vector3.Distance(item.transform.position, Projectile.position);
                    if(distance < currentDistance)
                    {
                        currentDistance = distance;
                        closestDamateReciver = item;
                    }
                }
            }

            closestDamateReciver.ApplyDamage(damage, stopTargetAfterAttack, attackSideEffect, attackTypeName, attackLevel, critted);
        }
    }

    private void ApplyDamage(float damage, bool stopTargetAfterAttack, EAttackSideEffect attackSideEffect, string attackTypeName, int attackLevel, bool critted)
    {
        int damageType = 0;
        if (damagaModyfier < 0)
            damageType = -1;
        if (damagaModyfier > 1)
            damageType = 1;
        enemy.TakeDamage(damage * damagaModyfier, stopTargetAfterAttack, EAttackType.WoodenBolt, attackSideEffect, attackTypeName, attackLevel, critted, damageType);
        counter = hitDisplayTime;
        Debug.Log(string.Format("{0} named {1} recived {2} damage.", GetType().Name, name, damage), this);
    }

    private void Reset()
    {
        enemy = transform.root.GetComponentInChildren<Enemy>();
    }

    private void OnDrawGizmos()
    {
        if((counter -= Time.deltaTime) > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            if(receverCollider is BoxCollider)
            {
                var box = receverCollider as BoxCollider;
                Gizmos.DrawCube(Vector3.zero, box.size);
                return;
            }

            if(receverCollider is SphereCollider)
            {
                var sphere = receverCollider as SphereCollider;
                Gizmos.DrawSphere(Vector3.zero, sphere.radius);
            }
        }
    }

}
