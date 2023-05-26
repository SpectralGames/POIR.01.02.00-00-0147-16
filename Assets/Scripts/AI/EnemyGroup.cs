using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnemyGroup
{
    private List<Enemy> enemies; // poprostu lista enemies
   // private StrategyTactics strategy;
    private Vector3 center;

    private Vector3 forcePos;

    #region Constructors
    public EnemyGroup ()
    {
        enemies = new List<Enemy>();
    }

    public EnemyGroup (List<Enemy> newEnemies)
    {
        enemies = new List<Enemy>(newEnemies);
       // SetStrategy(new SquareTactic(enemies));
    }

    public EnemyGroup (params Enemy[] newEnemies)
    {
        enemies = new List<Enemy>();
        enemies = newEnemies.ToList();
    }

    //public EnemyGroup (List<Enemy> newEnemies)
    //{
    //    enemies = new List<Enemy>(newEnemies);
    //}

    public EnemyGroup (List<Enemy> newEnemies, Vector3 centerPos)
    {
        enemies = new List<Enemy>(newEnemies);
        forcePos = centerPos;
        Debug.Log("Group center pos: " + centerPos);
        //ForceToCenterPosition();
    }
    #endregion Constructors


    private void ForceToCenterPosition ()
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.transform.position = forcePos;
        }
    }



    //private StrategyTactics GetStrategyType (List<Enemy> enemies, EStrategyType strategy)
    //{
    //    switch (strategy)
    //    {
    //        case EStrategyType.Base:
    //        return new BasicTactic(enemies);
    //        case EStrategyType.Square:
    //        return new SquareTactic(enemies);
    //        case EStrategyType.Triangle:
    //        return new TriangleTactic(enemies);
    //        case EStrategyType.Circle:
    //        return new CircleTactic(enemies);
    //        case EStrategyType.ReverseTriangle:
    //        return new ReverseTriangleTactic(enemies);
    //        default:
    //        return new BasicTactic(enemies);
    //    }
    //}

    //public void SetStrategy (StrategyTactics strategy)
    //{
    //    this.strategy = strategy;
    //}

    /// <summary>
    /// powinna byc wywolywana co klatke
    /// </summary>
    public void HoldTactic ()
    {
        //if (strategy != null)
        //{
        //    // Debug.Log("HoldTactic");
        //    strategy.HoldTactic();
        //}
    }

    /// <summary>
    /// wywolywana tylko na poczatku
    /// </summary>
    public void StartHoldTactic ()
    {
        //if (strategy != null)
        //{
        //    //  Debug.Log("StartHoldTactic");
        //    strategy.StartHoldTactic();
        //}
    }
}