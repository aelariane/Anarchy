﻿namespace Anarchy
{
    internal enum InputCodes : int
    {
        //Defaults
        Forward,

        Backward,
        Left,
        Right,
        Gas,
        Dodge,
        LeftHook,
        RightHook,
        BothHooks,
        Lock,
        Attack,
        SpecialAttack,
        Salute,
        CameraType,
        Reset,
        Pause,
        HideCursor,
        FullScreen,
        Reload,
        FlareGreen,
        FlareRed,
        FlareBlack,
        DefaultsCount,

        //Rebinds
        ReelIn,

        ReelOut,
        GasBurst,

        //Cannon
        CannonUp,

        CannongDown,
        CannonLeft,
        CannonRight,
        CannonFire,
        CannonMount,
        CannonSlow,

        //Titan
        TitanForward,

        TitanBackward,
        TitanLeft,
        TitanRight,
        TitanJump,
        TitanKill,
        TitanWalk,
        TitanPunch,
        TitanSlam,
        TitanGrabFront,
        TitanGrabBack,
        TitanGrabNape,
        TitanSlap,
        TitanCoverNape,

        //Horse
        HorseForward,

        HorseBackWard,
        HorseLeft,
        HorseRight,
        HorseWalk,
        HorseJump,
        HorseMount,

        //Mod
        ModCleanChat,

        ModCleanConsole,
        ModDebugPanel,
        ModStatsPanel
    }
}