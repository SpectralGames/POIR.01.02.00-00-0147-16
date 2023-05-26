using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HitInformation
{
    public float Damage { get; set; }
    //public float DamagePerSecond { get; set; } // ilosc dmg zadawane co sekunde
    public bool Stun { get; set; }
    public float Time { get; set; } // czas trwania obrazenia, stunu czegokolwiek

    public HitInformation()
    {

    }

    public HitInformation(float damage, float damagePerSecond, bool stun, float time)
    {
        Damage = damage;
        //DamagePerSecond = damagePerSecond;
        Stun = stun;
        Time = time;
    }
}