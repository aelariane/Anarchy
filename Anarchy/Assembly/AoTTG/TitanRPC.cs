using System;
using System.Diagnostics.CodeAnalysis;
using Anarchy;
using Anarchy.Configuration;
using Antis;
using ExitGames.Client.Photon;
using Optimization;
using Optimization.Caching;
using UnityEngine;

// ReSharper disable UnusedMember.Local
[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once CheckNamespace
public partial class TITAN
{
    [RPC]
    private void dieBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            var magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f) DieBlowFunc(attacker, hitPauseTime);
        }
    }

    [RPC]
    private void dieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            var magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f) DieHeadBlowFunc(attacker, hitPauseTime);
        }
    }

    [RPC]
    private void hitLRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            var magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f) Hit("hit_eren_L", attacker, hitPauseTime);
        }
    }

    [RPC]
    private void hitRRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            if (hasDie) return;

            var magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f) Hit("hit_eren_R", attacker, hitPauseTime);
        }
    }

    [RPC]
    private void laugh(float time = 0f)
    {
        if (state == TitanState.Idle || state == TitanState.Turn || state == TitanState.Chase)
        {
            sbtime = time;
            state = TitanState.Laugh;
            CrossFade("laugh", 0.2f);
        }
    }

    [RPC]
    public void labelRPC(int health, int healthMax)
    {
        if (health < 0)
        {
            if (healthLabel != null) Destroy(healthLabel);
        }
        else
        {
            if (healthLabel == null)
            {
                healthLabel = (GameObject) Instantiate(CacheResources.Load("UI/LabelNameOverHead"));
                healthLabel.name = "LabelNameOverHead";
                healthLabel.transform.parent = baseT;
                healthLabel.transform.localPosition = new Vector3(0f, 20f + 1f / myLevel, 0f);
                var num = 1f;
                if (myLevel < 1f) num = 1f / myLevel;

                healthLabel.transform.localScale = new Vector3(num, num, num);
                healthLabel.GetComponent<UILabel>().text = string.Empty;
                var txt = healthLabel.GetComponent<TextMesh>();
                if (txt == null) txt = healthLabel.AddComponent<TextMesh>();

                var render = healthLabel.GetComponent<MeshRenderer>();
                if (render == null) render = healthLabel.AddComponent<MeshRenderer>();

                render.material = Labels.Font.material;
                txt.font = Labels.Font;
                txt.fontSize = 20;
                txt.anchor = TextAnchor.MiddleCenter;
                txt.alignment = TextAlignment.Center;
                txt.color = Colors.white;
                txt.text = healthLabel.GetComponent<UILabel>().text;
                txt.richText = true;
                txt.gameObject.layer = 5;
                if (abnormalType == AbnormalType.Crawler)
                    healthLabel.transform.localPosition = new Vector3(0f, 10f + 1f / myLevel, 0f);

                healthLabelEnabled = true;
            }

            var str = "[7FFF00]";
            var num2 = health / (float) healthMax;
            if (num2 < 0.75f && num2 >= 0.5f)
                str = "[f2b50f]";
            else if (num2 < 0.5f && num2 >= 0.25f)
                str = "[ff8100]";
            else if (num2 < 0.25f) str = "[ff3333]";

            healthLabel.GetComponent<TextMesh>().text = (str + Convert.ToString(health)).ToHTMLFormat();
        }
    }

    [RPC]
    private void loadskinRPC(string body, string eye, PhotonMessageInfo info = null)
    {
        if (SkinSettings.TitanSkins != 1) return;

        if (Skin != null)
        {
            Debug.Log(
                $"Trying to change TITAN.Skin for viewID {BasePV.viewID} by ID {(info == null ? "-1" : info.Sender.ID.ToString())}");
            return;
        }

        //Put antis there
        StartCoroutine(loadskinRPCE(body, eye));
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        baseA.CrossFade(aniName, time);
    }

    [RPC]
    private void netDie()
    {
        asClientLookTarget = false;
        if (hasDie) return;

        hasDie = true;
        if (nonAI)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            PhotonNetwork.player.Dead = true;
            PhotonNetwork.player.Deaths++;
        }

        DieAnimation();
    }

    [RPC]
    private void netPlayAnimation(string aniName, PhotonMessageInfo info = null)
    {
        if (!Protection.TitanAnimationCheck.Check(aniName))
        {
            if (info != null) AntisManager.Response(info.Sender.ID, true, "Invalid TITAN anim: " + aniName);

            return;
        }

        baseA.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info = null)
    {
        if (!Protection.TitanAnimationCheck.Check(aniName))
        {
            if (info != null) AntisManager.Response(info.Sender.ID, true, "Invalid TITAN anim: " + aniName);

            return;
        }

        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void netSetAbnormalType(int type)
    {
        if (type == 0)
        {
            abnormalType = AbnormalType.Normal;
            runAnimation = "run_walk";
            GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 1)
        {
            abnormalType = AbnormalType.Aberrant;
            runAnimation = "run_abnormal";
            GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 2)
        {
            abnormalType = AbnormalType.Jumper;
            runAnimation = "run_abnormal";
            GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 3)
        {
            abnormalType = AbnormalType.Crawler;
            runAnimation = "crawler_run";
            GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 4)
        {
            abnormalType = AbnormalType.Punk;
            runAnimation = "run_abnormal_1";
            GetComponent<TITAN_SETUP>().setPunkHair();
        }

        name = titanNames[(int) abnormalType];
        ShowName = User.TitanNames[(int) abnormalType].PickRandomString();
        if (abnormalType == AbnormalType.Aberrant || abnormalType == AbnormalType.Jumper ||
            abnormalType == AbnormalType.Punk)
        {
            speed = 18f;
            if (myLevel > 1f) speed *= Mathf.Sqrt(myLevel);

            if (myDifficulty == 1) speed *= 1.4f;

            if (myDifficulty == 2) speed *= 1.6f;

            baseA["turnaround1"].speed = 2f;
            baseA["turnaround2"].speed = 2f;
        }

        if (abnormalType == AbnormalType.Crawler)
        {
            chaseDistance += 50f;
            speed = 25f;
            if (myLevel > 1f) speed *= Mathf.Sqrt(myLevel);

            if (myDifficulty == 1) speed *= 2f;

            if (myDifficulty == 2) speed *= 2.2f;

            AABB.GetComponent<CapsuleCollider>().height = 10f;
            AABB.GetComponent<CapsuleCollider>().radius = 5f;
            AABB.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
        }

        if (nonAI)
        {
            if (abnormalType == AbnormalType.Crawler)
                speed = Mathf.Min(70f, speed);
            else
                speed = Mathf.Min(60f, speed);

            baseA["attack_jumper_0"].speed = 7f;
            baseA["attack_crawler_jump_0"].speed = 4f;
        }

        baseA["attack_combo_1"].speed = 1f;
        baseA["attack_combo_2"].speed = 1f;
        baseA["attack_combo_3"].speed = 1f;
        baseA["attack_quick_turn_l"].speed = 1f;
        baseA["attack_quick_turn_r"].speed = 1f;
        baseA["attack_anti_AE_l"].speed = 1.1f;
        baseA["attack_anti_AE_low_l"].speed = 1.1f;
        baseA["attack_anti_AE_r"].speed = 1.1f;
        baseA["attack_anti_AE_low_r"].speed = 1.1f;
        Idle();
    }

    [RPC]
    private void netSetLevel(float level, int AI, int skinColor)
    {
        SetLevel(level, AI, skinColor);
    }

    [RPC]
    private void playsoundRPC(string soundName)
    {
        var transformSound = baseT.Find(soundName);
        transformSound.GetComponent<AudioSource>().Play();
    }

    [RPC]
    private void setIfLookTarget(bool bo)
    {
        asClientLookTarget = bo;
    }

    [RPC]
    private void setMyTarget(int ID)
    {
        if (ID == -1) myHero = null;

        var photonView = PhotonView.Find(ID);
        if (photonView != null) myHero = photonView.gameObject;
    }

    [RPC]
    public void DieByCannon(int viewID)
    {
        var view = PhotonView.Find(viewID);
        if (view != null)
        {
            var damage = 0;
            if (PhotonNetwork.IsMasterClient) OnTitanDie(view);

            if (nonAI)
                FengGameManagerMKII.FGM.TitanGetKill(view.owner, damage, PhotonNetwork.player.UIName);
            else
                FengGameManagerMKII.FGM.TitanGetKill(view.owner, damage, ShowName);
        }
        else
        {
            FengGameManagerMKII.FGM.BasePV.RPC("netShowDamage", view.owner, speed);
        }
    }

    [RPC]
    public void grabbedTargetEscape()
    {
        grabbedTarget = null;
    }

    [RPC]
    public void grabToLeft()
    {
        grabTF.transform.parent = Hand_L_001;
        grabTF.transform.position = Hand_L_001SphereT.position;
        grabTF.transform.rotation = Hand_L_001SphereT.rotation;
        grabTF.transform.localPosition -= Vectors.right * Hand_L_001Sphere.radius * 0.3f;
        grabTF.transform.localPosition -= Vectors.up * Hand_L_001Sphere.radius * 0.51f;
        grabTF.transform.localPosition -= Vectors.forward * Hand_L_001Sphere.radius * 0.3f;
        grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x,
            grabTF.transform.localRotation.eulerAngles.y + 180f,
            grabTF.transform.localRotation.eulerAngles.z + 180f);
    }

    [RPC]
    public void grabToRight()
    {
        grabTF.transform.parent = Hand_R_001;
        grabTF.transform.position = Hand_R_001SphereT.position;
        grabTF.transform.rotation = Hand_R_001SphereT.rotation;
        grabTF.transform.localPosition -= Vectors.right * Hand_R_001Sphere.radius * 0.3f;
        grabTF.transform.localPosition += Vectors.up * Hand_R_001Sphere.radius * 0.51f;
        grabTF.transform.localPosition -= Vectors.forward * Hand_R_001Sphere.radius * 0.3f;
        grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x,
            grabTF.transform.localRotation.eulerAngles.y + 180f,
            grabTF.transform.localRotation.eulerAngles.z);
    }

    [RPC]
    public void hitAnkleRPC(int viewID)
    {
        if (hasDie) return;

        if (state == TitanState.Down) return;

        var photonView = PhotonView.Find(viewID);
        if (photonView == null) return;

        var magnitude = (photonView.gameObject.transform.position - baseT.position).magnitude;
        if (magnitude < 20f)
        {
            if (BasePV.IsMine && grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);

            GetDown();
        }
    }

    [RPC]
    public void hitEyeRPC(int viewID)
    {
        if (hasDie) return;

        var magnitude = (PhotonView.Find(viewID).gameObject.transform.position - Neck.position).magnitude;
        if (magnitude < 20f)
        {
            if (BasePV.IsMine && grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);

            if (!hasDie) JustHitEye();
        }
    }

    [RPC]
    public void moveToRPC(float x, float y, float z, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient) baseT.position = new Vector3(x, y, z);
    }

    [RPC]
    public void titanGetHit(int viewID, int hitSpeed)
    {
        var photonView = PhotonView.Find(viewID);
        if (photonView == null) return;

        var magnitude = (photonView.gameObject.transform.position - Neck.position).magnitude;
        if (magnitude < lagMax && !hasDie && Time.time - healthTime > 0.2f)
        {
            healthTime = Time.time;
            if (GameModes.DamageMode.Enabled && hitSpeed < GameModes.DamageMode.GetInt(0))
            {
                FengGameManagerMKII.FGM.BasePV.RPC("netShowDamage", photonView.owner, hitSpeed);
                if (maxHealth > 0) BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, currentHealth, maxHealth);

                return;
            }

            currentHealth -= hitSpeed;
            if (maxHealth > 0) BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                OnTitanDie(photonView);
                BasePV.RPC("netDie", PhotonTargets.OthersBuffered);
                if (grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);

                netDie();
                if (nonAI)
                    FengGameManagerMKII.FGM.TitanGetKill(photonView.owner, hitSpeed,
                        (string) PhotonNetwork.player.Properties[PhotonPlayerProperty.name]);
                else
                    FengGameManagerMKII.FGM.TitanGetKill(photonView.owner, hitSpeed, ShowName);
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("netShowDamage", photonView.owner, hitSpeed);
            }
        }
    }
}