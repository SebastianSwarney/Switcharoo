using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    void ActiveState(bool p_active);
    void ResetMe();
}
