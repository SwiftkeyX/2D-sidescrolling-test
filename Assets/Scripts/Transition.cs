using System;
using UnityEngine;

namespace MagicSchool.Combat.Heroes.States
{
    // share transition condition for statemachine
    internal class Transition
    {
        private readonly Player _me;

        public Transition(Player me)
        {
            _me = me;
        }

        // ======================================== transition condition ========================================
        // if enemy is in attack range, not dead, transition to attack 
        public bool CanAttack(ICombatant nearestEnemy)
        {
            return nearestEnemy != null && nearestEnemy.IsAlive && _me.CurrentHex.IsWithinRange(nearestEnemy.CurrentHex(), _me.Range);
        }

        public bool CanWalk(ICombatant nearestEnemy)
        {
            // If there is ANY enemy that'll walk into my neighbors (adjacent), stop moving, and wait for him instead
            if (IsEnemyArrivingNextToMe()) return false;

            // Find next hex that could lead this hero toward nearest enemy
            Hex targetHex = HexPathfinder.FindValidHexToTarget(_me.CurrentHex, nearestEnemy.CurrentHex(), _isHexBlocked);
            if (targetHex == null) return false;

            // Do I wait for the blocker to move? (Read function's comment)
            if (ShouldWaitForBlocker(nearestEnemy, targetHex)) return false;

            // finally, walk, reset the hold
            targetHex.OnUnitReserved(_me);
            _holdSince = -1f;
            return true;
        }

        // ======================================== private ========================================
        // ...
    }
}
