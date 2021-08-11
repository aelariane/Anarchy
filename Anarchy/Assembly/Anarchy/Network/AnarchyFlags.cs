namespace Anarchy.Network
{
    /// <summary>
    /// Anarchy functions flags
    /// </summary>
    /// <remarks>Used fo netwiork detection of functions used by players</remarks>
    public enum AnarchyFlags
    {
        DisableBodyLean = 0b1,
        LegacyBurst = 0b10,
        NewTPSCamera = 0b100,
        DisableBurstCooldown = 0b1000
    }
}
