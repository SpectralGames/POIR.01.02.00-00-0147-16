using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISpellTypeProvider
{
    event Action<EAttackType> OnSetSpellType;  
}

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private AttacksController attacksController = null;
    

    [SerializeField] private EAttackType _attackType = EAttackType.VioletBall;
    public EAttackType AttackType
    {
        get { return _attackType; }
        set { _attackType = value; }
    }

    public readonly HashSet<EAttackType> maidens = new HashSet<EAttackType>(new[] { EAttackType.FireMaiden, EAttackType.IceMaiden, EAttackType.FireMaiden, EAttackType.ThunderMaiden } );
    public static event Action OnSpellCast = delegate {  };
    
    public void GetSpellTypeProviders(GameObject gameObject)
    {
        var SpellTypeProviders = gameObject.GetComponentsInChildren<ISpellTypeProvider>();
        foreach (var item in SpellTypeProviders)
            item.OnSetSpellType += SetAttackType;
    }
    

    public void Cast()
    {

            
    }

    public void SetAttackType(EAttackType attackType)
    {
        _attackType = attackType;
    }
}
