using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.UI;
using Antis;
using ExitGames.Client.Photon;
using Optimization.Caching;
using RC;
using UnityEngine;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
// ReSharper disable once CheckNamespace
public partial class HERO
{
    [RPC]
    private void backToHumanRPC()
    {
        titanForm = false;
        eren = null;
        baseG.GetComponent<SmoothSyncMovement>().Disabled = false;
    }

    [RPC]
    private void killObject(PhotonMessageInfo info)
    {
        Log.AddLineRaw($"HERO.killObjectRPC by ID {info.Sender.ID}", MsgType.Warning);
    }

    [RPC]
    private void net3DMGSMOKE(bool ifON)
    {
        if (smoke3Dmg != null) smoke3Dmg.enableEmission = ifON;
    }

    [RPC]
    private void netContinueAnimation()
    {
        foreach (var obj in baseA)
        {
            var animationState = (AnimationState) obj;
            if (animationState.speed == 1f) return;
            animationState.speed = 1f;
        }

        PlayAnimation(CurrentPlayingClipName());
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        currentAnimation = aniName;
        if (baseA != null) baseA.CrossFade(aniName, time);
    }

    [RPC]
    private void netDie2(int viewID = -1, string titanName = "", PhotonMessageInfo info = null)
    {
        if (BasePV.IsMine)
        {
            if (myBomb != null) myBomb.destroyMe();
            if (myCannon != null) PhotonNetwork.Destroy(myCannon);
            PhotonNetwork.RemoveRPCs(BasePV);
            if (titanForm && eren != null) eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            if (skillCD != null) skillCD.transform.localPosition = Vectors.up * 5000f;
        }

        meatDie.Play();
        if (bulletLeft != null) bulletLeft.RemoveMe();
        if (bulletRight != null) bulletRight.RemoveMe();
        var audioTf = baseT.Find("audio_die");
        if (audioTf != null)
        {
            audioTf.parent = null;
            audioTf.GetComponent<AudioSource>().Play();
        }

        if (BasePV.IsMine)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            FengGameManagerMKII.FGM.logic.MyRespawnTime = 0f;
        }

        FalseAttack();
        IsDead = true;
        baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            PhotonNetwork.RemoveRPCs(BasePV);
            PhotonNetwork.player.Dead = true;
            PhotonNetwork.player.Deaths++;
            if (viewID != -1)
            {
                var photonView = PhotonView.Find(viewID);
                if (photonView != null)
                {
                    FengGameManagerMKII.FGM.SendKillInfo(true, User.DeathFormat(info.Sender.ID, info.Sender.UIName),
                        false, User.DeathName);
                    photonView.owner.Kills++;
                }
            }
            else
            {
                FengGameManagerMKII.FGM.SendKillInfo(true, User.DeathFormat(info.Sender.ID, titanName), false,
                    User.DeathName);
            }

            AnarchyManager.Feed.Kill(titanName, User.DeathName, 0);
            FengGameManagerMKII.FGM.BasePV.RPC("someOneIsDead", PhotonTargets.MasterClient,
                titanName != string.Empty ? 1 : 0);
        }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            Pool.NetworkEnable("hitMeat2", baseT.position, Quaternion.Euler(270f, 0f, 0f));
        else
            Pool.Enable("hitMeat2", baseT.position, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            OnDeathEvent(viewID, true);
            if (RCManager.heroHash.ContainsKey(viewID)) RCManager.heroHash.Remove(viewID);
        }

        if (BasePV.IsMine) PhotonNetwork.Destroy(BasePV);
    }

    [RPC]
    private void netGrabbed(int id, bool leftHand)
    {
        titanWhoGrabMeID = id;
        Grabbed(PhotonView.Find(id).gameObject, leftHand);
    }

    [RPC]
    private void netlaughAttack()
    {
        var array = GameObject.FindGameObjectsWithTag("titan");
        foreach (var gameObject in array)
            if (Vector3.Distance(gameObject.transform.position, baseT.position) < 50f &&
                Vector3.Angle(gameObject.transform.Forward(), baseT.position - gameObject.transform.position) < 90f &&
                gameObject.GetComponent<TITAN>())
                gameObject.GetComponent<TITAN>().BeLaughAttacked();
    }

    [RPC]
    private void netPauseAnimation()
    {
        foreach (var obj in baseA)
        {
            var animationState = (AnimationState) obj;
            animationState.speed = 0f;
        }
    }

    [RPC]
    private void netPlayAnimation(string aniName, PhotonMessageInfo info = null)
    {
        if (!Protection.HeroAnimationCheck.Check(aniName))
        {
            if (info != null) AntisManager.Response(info.Sender.ID, true, "Invalid HERO anim: " + aniName);
            return;
        }

        currentAnimation = aniName;
        baseA.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info = null)
    {
        if (!Protection.HeroAnimationCheck.Check(aniName))
        {
            if (info != null) AntisManager.Response(info.Sender.ID, true, "Invalid HERO anim: " + aniName);
            return;
        }

        currentAnimation = aniName;
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void netSetIsGrabbedFalse()
    {
        State = HeroState.Idle;
    }

    [RPC]
    private void netTauntAttack(float tauntTime, float distance = 100f)
    {
        foreach (var tit in FengGameManagerMKII.Titans)
            if (tit != null && !tit.hasDie && Vector3.Distance(tit.baseGT.position, baseT.position) < distance)
                tit.BeTauntedBy(baseG, tauntTime);
    }

    [RPC]
    private void netUngrabbed()
    {
        Ungrabbed();
        netPlayAnimation(standAnimation);
        FalseAttack();
    }

    [RPC]
    private void RPCHookedByHuman(int hooker, Vector3 hookPosition)
    {
        hookBySomeOne = true;
        badGuy = PhotonView.Find(hooker).gameObject;
        if (Vector3.Distance(hookPosition, baseT.position) < 15f)
        {
            launchForce = PhotonView.Find(hooker).gameObject.transform.position - baseT.position;
            baseR.AddForce(-baseR.velocity * 0.9f, ForceMode.VelocityChange);
            var d = Mathf.Pow(launchForce.magnitude, 0.1f);
            if (grounded) baseR.AddForce(Vectors.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
            baseR.AddForce(launchForce * d * 0.1f, ForceMode.Impulse);
            if (State != HeroState.Grab)
            {
                dashTime = 1f;
                CrossFade("dash", 0.05f);
                baseA["dash"].time = 0.1f;
                State = HeroState.AirDodge;
                FalseAttack();
                facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
                var quaternion = Quaternion.Euler(0f, facingDirection, 0f);
                baseGT.rotation = quaternion;
                baseR.rotation = quaternion;
                targetRotation = quaternion;
            }
        }
        else
        {
            hookBySomeOne = false;
            badGuy = null;
            PhotonView.Find(hooker).RPC("hookFail", PhotonView.Find(hooker).owner);
        }
    }

    [RPC]
    private void setMyTeam(int val)
    {
        myTeam = val;
        if (IsLocal)
        {
            wLeft.myTeam = val;
            wRight.myTeam = val;
        }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
        {
            if (GameModes.FriendlyMode.Enabled)
            {
                if (val != 1) BasePV.RPC("setMyTeam", PhotonTargets.AllBuffered, 1);
            }
            else
            {
                if (GameModes.BladePvp.Enabled)
                {
                    var team = 0;
                    switch (GameModes.BladePvp.Selection)
                    {
                        case 1:
                            team = BasePV.owner.RCteam;
                            break;

                        case 2:
                            team = BasePV.owner.ID;
                            break;
                    }

                    if (val == team) return;
                    BasePV.RPC("setMyTeam", PhotonTargets.AllBuffered, team);
                }
            }
        }
    }

    [RPC]
    private void showHitDamage()
    {
        var go = CacheGameObject.Find("LabelScore");
        if (!go) return;
        speed = Mathf.Max(10f, speed);
        go.GetComponent<UILabel>().text = speed.ToString(CultureInfo.CurrentCulture); //maybe Invariant
        go.transform.localScale = Vectors.zero;
        speed = (int) (speed * 0.1f);
        speed = Mathf.Clamp(speed, 40f, 150f);
        iTween.Stop(go);
        iTween.ScaleTo(go,
            iTween.Hash("x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
        iTween.ScaleTo(go,
            iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f));
    }

    [RPC]
    private void whoIsMyErenTitan(int id)
    {
        eren = PhotonView.Find(id).gameObject;
        titanForm = true;
    }

    [RPC]
    public void badGuyReleaseMe()
    {
        hookBySomeOne = false;
        badGuy = null;
    }

    [RPC]
    public void blowAway(Vector3 force)
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && !BasePV.IsMine) return;
        baseR.AddForce(force, ForceMode.Impulse);
        baseT.LookAt(baseT.position);
    }

    [RPC]
    public void hookFail()
    {
        hookTarget = null;
        hookSomeOne = false;
    }

    [RPC]
    public void loadskinRPC(int horse, string urls, PhotonMessageInfo info = null)
    {
        if (SkinSettings.HumanSkins.Value == 0) return;
        if (info != null && BasePV != null && (BasePV.owner.ID != info.Sender.ID ||
                                               !Anarchy.Network.Antis.IsValidSkinURL(ref urls, 13, info.Sender.ID)))
            return;
        if (SkinSettings.HumanSkins.Value > 1 && info != null && !info.Sender.IsLocal) return;
        StartCoroutine(loadskinRPCE(horse, urls, info));
    }

    [RPC]
    public void netDie(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true,
        PhotonMessageInfo info = null)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            OnDeathEvent(viewID, killByTitan);
            if (RCManager.heroHash.ContainsKey(viewID)) RCManager.heroHash.Remove(viewID);
        }

        if (BasePV.IsMine && titanForm && eren != null) eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (bulletLeft) bulletLeft.RemoveMe();
        if (bulletRight) bulletRight.RemoveMe();
        meatDie.Play();
        if (!Gunner && (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }

        FalseAttack();
        BreakApart(v, isBite);
        if (BasePV.IsMine)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(false);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            FengGameManagerMKII.FGM.logic.MyRespawnTime = 0f;
        }

        IsDead = true;
        var audioTransform = baseT.Find("audio_die");
        audioTransform.parent = null;
        audioTransform.GetComponent<AudioSource>().Play();
        baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        if (BasePV.IsMine)
        {
            if (myBomb != null) myBomb.destroyMe();
            if (myCannon != null) PhotonNetwork.Destroy(myCannon);
            PhotonNetwork.RemoveRPCs(BasePV);
            PhotonNetwork.player.Dead = true;
            PhotonNetwork.player.Deaths++;
            FengGameManagerMKII.FGM.BasePV.RPC("someOneIsDead", PhotonTargets.MasterClient,
                titanName == string.Empty ? 0 : 1);
            if (viewID != -1)
            {
                var photonView = PhotonView.Find(viewID);
                if (photonView != null)
                {
                    FengGameManagerMKII.FGM.SendKillInfo(killByTitan,
                        User.DeathFormat(info.Sender.ID, info.Sender.UIName), false, User.DeathName);
                    photonView.owner.Kills++;
                }
            }
            else
            {
                FengGameManagerMKII.FGM.SendKillInfo(titanName != string.Empty,
                    User.DeathFormat(info.Sender.ID, titanName), false, User.DeathName);
            }

            AnarchyManager.Feed.Kill(titanName, User.DeathName, 0);
        }

        if (BasePV.IsMine) PhotonNetwork.Destroy(BasePV);
    }

    [RPC]
    public void moveToRPC(float x, float y, float z, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient) baseT.position = new Vector3(x, y, z);
    }

    [RPC]
    public void ReturnFromCannon(PhotonMessageInfo info)
    {
        if (info.Sender == BasePV.owner)
        {
            isCannon = false;
            gameObject.GetComponent<SmoothSyncMovement>().Disabled = false;
        }
    }

    [RPC]
    public void SetMyCannon(int viewid, PhotonMessageInfo info)
    {
        if (info.Sender == BasePV.owner)
        {
            var photonView = PhotonView.Find(viewid);
            if (photonView != null)
            {
                Log.AddLineRaw("viewID: " + viewid);
                Component[] comps = photonView.gameObject.GetComponentsInChildren<MonoBehaviour>();
                foreach (var com in comps) Log.AddLineRaw(com.GetType().Name);
            }

            if (photonView != null && photonView.gameObject.GetComponent<Cannon>() != null)
            {
                myCannon = photonView.gameObject;
                if (myCannon != null)
                {
                    myCannonBase = myCannon.transform;
                    myCannonPlayer = myCannonBase.Find("PlayerPoint");
                    isCannon = true;
                }
            }
        }
    }

    [RPC]
    public void SetMyPhotonCamera(float offset, PhotonMessageInfo info)
    {
        if (BasePV.owner == info.Sender) //TODO: Maybe change to equals ?
        {
            CameraMultiplier = offset;
            GetComponent<SmoothSyncMovement>().PhotonCamera = true;
            IsPhotonCamera = true;
        }
    }

    [RPC]
    public void SpawnCannonRPC(string settings, PhotonMessageInfo info)
    {
        var flag = info.Sender.IsMasterClient && IsLocal && myCannon == null;
        if (flag)
        {
            var flag2 = myHorse != null && isMounted;
            if (flag2) GetOffHorse();
            idle();
            var flag3 = bulletLeft != null;
            if (flag3) bulletLeft.RemoveMe();
            var flag4 = bulletRight != null;
            if (flag4) bulletRight.RemoveMe();
            var flag5 = smoke3Dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && IsLocal;
            if (flag5)
            {
                object[] parameters =
                {
                    false
                };
                BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, parameters);
            }

            smoke3Dmg.enableEmission = false;
            rigidbody.velocity = Vector3.zero;
            var array = settings.Split(',');
            var flag6 = array.Length > 15;
            if (flag6)
                myCannon = Pool.NetworkEnable("RCAsset/" + array[1],
                    new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])),
                    new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]),
                        Convert.ToSingle(array[17]), Convert.ToSingle(array[18])));
            else
                myCannon = Pool.NetworkEnable("RCAsset/" + array[1],
                    new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])),
                    new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]),
                        Convert.ToSingle(array[8])));
            myCannonBase = myCannon.transform;
            myCannonPlayer = myCannon.transform.Find("PlayerPoint");
            isCannon = true;
            myCannon.GetComponent<Cannon>().myHero = this;
            myCannonRegion = null;
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(myCannon.transform.Find("Barrel").Find("FiringPoint")
                .gameObject);
            IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView = 55f;
            BasePV.RPC("SetMyCannon", PhotonTargets.OthersBuffered, myCannon.GetPhotonView().viewID);
            skillCDLastCannon = skillCDLast;
            skillCDLast = 3.5f;
            skillCDDuration = 3.5f;
        }
    }
}