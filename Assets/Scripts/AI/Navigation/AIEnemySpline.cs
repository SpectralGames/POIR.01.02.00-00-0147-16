using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AIEnemySpline : AISplinePath
{
    private Enemy enemy;
	public event Action OnSplineEndAction;

    public AIEnemySpline (Enemy ai, BezierPath start) : base(ai, start)
    {
        this.enemy = ai;
        Init(ai, start);
    }

    public override void Init (AI ai, BezierPath start)
    {
        base.Init(ai, start);
    }

    public override void Tick ()
    {
        if (enemy.BlockMovement) return;
        base.Tick();
        // sprwdz koniec sciezki
    }
		

    protected override void OnSplineEnd (BezierPath path, int headedPoint = 0)
    {
        if (isGoingForward)
        {
            
            if (path.controlPoint.isTriggerEndPoint)
            {
                CheckCarryVirgins();
            }
            
            isGoingForward = false;
            this.SetPath(path.controlPoint.nextPoints[headedPoint]); //wypusc go z powrotem
        }
        else // jesli sie wraca i doszedl do celu
        {
            if(CheckCarryVirgins() == false)
            {
                isGoingForward = true;
            }
        }

		if(OnSplineEndAction != null)
			OnSplineEndAction();
    }

    protected override void ReachedPoint(BezierPath point) // check is point has is end path trgger value, then check if is with virgin.
    {
        base.ReachedPoint(point);
        if (point == null)
            return;

        if (point.controlPoint.isTriggerEndPoint)
        {
            CheckCarryVirgins();
        }
    }

    private bool CheckCarryVirgins()
    {
        if (enemy.carryVirginList.Count > 0)
        {
            foreach (var virgin in enemy.carryVirginList)
            {
                virgin.OnTeleport_Invoke();
                enemy.OnTeleport_Invoke();
            }
            enemy.carryVirginList.Clear();
            return true;
        }
        return false;
    }
}