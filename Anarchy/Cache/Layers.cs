using UnityEngine;

namespace Cache
{
    /// <summary>
    /// Cached used in game Layers
    /// </summary>
    public static class Layers
    {
        public const int EnemyAABBN = 11;
        public const int EnemyBoxN = 10;
        public const int GroundN = 9;
        public const int NetworkObjectN = 13;
        public const int PlayersN = 8;

        public static readonly LayerMask AABB;
        public static readonly LayerMask EnemyAABB;
        public static readonly LayerMask EnemyAABBGround;
        public static readonly LayerMask EnemyBox;
        public static readonly LayerMask EnemyGround;
        public static readonly LayerMask EnemyGroundNetwork;
        public static readonly LayerMask EnemyGroundPlayerAttack;
        public static readonly LayerMask Ground;
        public static readonly LayerMask NetworkObject;
        public static readonly LayerMask PlayerAttackBox;
        public static readonly LayerMask Players;
        public static readonly LayerMask PlayersEnemyAABGround;

        static Layers()
        {
            AABB = 1 << LayerMask.NameToLayer("AABB");
            EnemyAABB = 1 << LayerMask.NameToLayer("EnemyAABB");
            EnemyBox = 1 << LayerMask.NameToLayer("EnemyBox");
            Ground = 1 << LayerMask.NameToLayer("Ground");
            NetworkObject = 1 << LayerMask.NameToLayer("NetworkObject");
            PlayerAttackBox = 1 << LayerMask.NameToLayer("PlayerAttackBox");
            Players = 1 << LayerMask.NameToLayer("Players");
            EnemyAABBGround = (EnemyAABB | Ground);
            EnemyGround = (EnemyBox | Ground);
            EnemyGroundNetwork = (EnemyBox | Ground | NetworkObject);
            EnemyGroundPlayerAttack = (EnemyBox | Ground | PlayerAttackBox);
            PlayersEnemyAABGround = (Ground | Players | EnemyAABB);
        }
    }
}