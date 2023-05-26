using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EnemyInformation //???
{
    public float speed = 1.0f;
    public float rotationSpeed = 9.0f;
    public bool autoKillAfterPathEnd = false;
    public bool stopMovementOnHit = false;
    public bool isAttackTimeSensitive = false;
    public float timeForAttackEvent; //kiedy ma odpalic attack

    public bool isAlive = false;
    public float enemyHeight;


    public bool isIdle = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isTakingDamage = false;
    public bool isHitting = false;
    public bool isDead = false;
}

public class EnemyMovemenet
{
    public float speed = 1.0f;
    public float currentSpeed;
    public float rotationSpeed = 9.0f;

    public EnemyMovemenet ()
    {

    }
}