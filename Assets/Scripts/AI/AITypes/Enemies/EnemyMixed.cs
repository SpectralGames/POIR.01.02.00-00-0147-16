using UnityEngine;


public class EnemyMixed : Enemy
{
    #region Actions
    #endregion Actions

    #region Members
    #endregion

    #region Inits
    private void Start ()
    {
        //Init(null);
    }
    public override void Init (Pathpoint startPathPoint, BezierPath startBezierPath, NavigationType navigationType)
    {
        base.Init(startPathPoint, startBezierPath, navigationType);
        //CreateFSM();
    }
    #endregion Inits

    #region Logic
    private void Update ()
    {
        if (!isInit)
            return;

        NavigationController?.Tick(); // new
        //aiController?.Tick();
        statusBar?.Tick();
        finiteStateMachine.CurrentState?.Reason();
        finiteStateMachine.CurrentState?.Act();
    }

    public override void ForceOnLookAt (Transform target = null)
    {
        StopCoroutine("OnLookAt");
        //if (target == null)
        //    target = aiController.EnemyFixer.currentTargetInformation; // zmienic  nie bedzie enemyfixer
        StartCoroutine(OnLookAt(target));
    }

	/*
    protected override void CreateFSM ()
    {
        // TODO: stworz mozliwe stany // podac stringa, moze dictionary
        EnemyMovementState movement = CreateMovementState();
        EnemyTakeDamageState takeDamage = CreateTakeDamageState();
        EnemyAvoidingSpellState avoid = CreateAvoidState();

        EnemyCarryVirginState virgin = new EnemyCarryVirginState(package);
        EnemyDeathState death = new EnemyDeathState(package);

        finiteStateMachine = new FSMSystem();
        finiteStateMachine.AddState(movement);
        //finiteStateMachine.AddState(takeDamage);
        finiteStateMachine.AddState(avoid);
        finiteStateMachine.AddState(death);
        finiteStateMachine.AddState(virgin);
        //finiteStateMachine.AddState(attack);
    }

    private EnemyAvoidingSpellState CreateAvoidState ()
    {
        EnemyAvoidingSpellState avoid = new EnemyAvoidingSpellState(package);
        avoid.AddTransition(Transition.LostPlayer, StateID.Movement);
        avoid.AddTransition(Transition.Hit, StateID.TakingDamage);
        avoid.AddTransition(Transition.DeadlyHit, StateID.Death);
        return avoid;
    }

    private EnemyTakeDamageState CreateTakeDamageState ()
    {
        EnemyTakeDamageState takeDamage = new EnemyTakeDamageState(package, animator.GetAnimations("Hit"));
        takeDamage.AddTransition(Transition.LostPlayer, StateID.Movement); //?? // transisiton = nie jest juz w polu razenia
        takeDamage.AddTransition(Transition.Hit, StateID.TakingDamage);
        takeDamage.AddTransition(Transition.DeadlyHit, StateID.Death);
        takeDamage.AddTransition(Transition.SpellThrew, StateID.AvoidingSpell);
        return takeDamage;
    }

    private EnemyMovementState CreateMovementState ()
    {
        EnemyMovementState movement = new EnemyMovementState(package, animator.GetAnimations("Walk"), animator.GetAnimations("Run"));
        movement.AddTransition(Transition.SawPlayer, StateID.AttackingPlayer);
        movement.AddTransition(Transition.Hit, StateID.TakingDamage);
        movement.AddTransition(Transition.DeadlyHit, StateID.Death);
        movement.AddTransition(Transition.SpellThrew, StateID.AvoidingSpell);
        movement.AddTransition(Transition.VirginIsClose, StateID.CarryVirign);
        return movement;
    }
	*/

    #endregion Logic
}