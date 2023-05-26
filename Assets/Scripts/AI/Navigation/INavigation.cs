using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum NavigationType
{
    NavMesh,
    Splines,
    None
}

public interface INavigation : IPath
{
    void Enable ();
    void Disable ();
    float WalkSpeed { get; set; }
    void Tick ();
    void GetBackPosition ();
	void UpdateSpeedFactor ();
	bool IsMovingEnabled();
}