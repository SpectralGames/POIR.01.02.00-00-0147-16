using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyStatePackage
{
    #region Members
    public Enemy enemy;
    // lista dziwic
    public GameObject player;

    public List<Virgin> virigins;
    #endregion Members

    #region Constructors
    public EnemyStatePackage ()
    {

    }

    public EnemyStatePackage(Enemy enemy, GameObject player, List<Virgin> virg)
    {
        this.enemy = enemy;
        this.player = player;
        virigins = new List<Virgin>(virg);
    }
    #endregion Constructors

}