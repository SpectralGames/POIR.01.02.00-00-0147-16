using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singleton;

[CreateAssetMenu(fileName = "MagicTypeDamageMap", menuName = "Magic/MagicTypeDamageMap")]
public class MagicTypeDamageMap : ScriptableObjectSingleton<MagicTypeDamageMap>
{
    [Serializable]
    public class TypeMaping
    {
        [SerializeField] private MagicType attacker = null;
        public MagicType Attacker { get { return attacker; } }

        [SerializeField] private MagicType receiver = null;
        public MagicType Receiver { get { return receiver; } }

        [Range(-1, 1), SerializeField] private float modifier = 0;
        public float Modifier { get { return modifier; } }
    }

    [SerializeField] private List<TypeMaping> typeMapings = new List<TypeMaping>();

    public float GetModyfier(MagicType attacker, MagicType receiver)
    {
        foreach (var item in typeMapings)
            if (item.Attacker == attacker && item.Receiver == receiver)
                return item.Modifier;

        return 0;
    }
}