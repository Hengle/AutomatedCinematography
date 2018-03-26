using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomVariables;

public interface IIncludable
{
	void IncludeActor (string actor);

}

public interface IIntensify
{
	void ZoomIn (float val);
}
