using UnityEngine;
using DragonBones;
using System.Collections.Generic;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    private UnityArmatureComponent armature;
    private List<Slot> L_EyeSlots = new List<Slot>();
    private List<Slot> R_EyeSlots = new List<Slot>();
    private List<Slot> L_EyeMadSlots = new List<Slot>();
    private List<Slot> R_EyeMadSlots = new List<Slot>();
    private List<Slot> NyamuSlots = new List<Slot>();
    private const float TICK_TIME = 60f;
    private float ticker = 0f;
    public bool isMad = false;
    private Slot madSlot;
    public float blinkDuration = 5f;
    public float transitionTime = 0.5f;
    public float idleTime = 5f;

    void Start()
    {
        armature = GetComponent<UnityArmatureComponent>();

        madSlot = armature.armature.GetSlot("Mad");
        madSlot.displayIndex = -1;
        // 播放 Idle 动画（循环）
        armature.animation.Play("Idle");
        L_EyeSlots.Add(armature.armature.GetSlot("L_eyeball"));
        L_EyeSlots.Add(armature.armature.GetSlot("L_eyelid"));
        L_EyeSlots.Add(armature.armature.GetSlot("L_eyebrow"));
        R_EyeSlots.Add(armature.armature.GetSlot("R_eyeball"));
        R_EyeSlots.Add(armature.armature.GetSlot("R_eyelid"));
        R_EyeSlots.Add(armature.armature.GetSlot("R_eyebrow"));
        L_EyeMadSlots.Add(armature.armature.GetSlot("L_eyelid_closed"));
        L_EyeMadSlots.Add(armature.armature.GetSlot("L_eyebrow_mad"));
        R_EyeMadSlots.Add(armature.armature.GetSlot("R_eyelid_closed"));
        R_EyeMadSlots.Add(armature.armature.GetSlot("R_eyebrow_mad"));
        NyamuSlots.Add(armature.armature.GetSlot("Nyamu"));
        NyamuSlots.Add(armature.armature.GetSlot("Nyamu_Tail"));

        // Disable eye slots
        foreach (Slot slot in L_EyeMadSlots)
        {
            slot.displayIndex = -1;
        }
        foreach (Slot slot in R_EyeMadSlots)
        {
            slot.displayIndex = -1;
        }
        foreach (Slot slot in NyamuSlots)
        {
            slot.displayIndex = -1;
        }

        StartCoroutine(PlayNyamuCome());
    }

    void Update()
    {
        // Play Blink animation every blinkDuration seconds
        if (Time.time % blinkDuration < 0.1f)
        {
            // Debug.Log("Blink");
            PlayBlink();
        }

        ticker += Time.deltaTime;
        if (ticker >= TICK_TIME)
        {
            Debug.Log("ticker: " + ticker);
            if (isMad)
            {
                Debug.Log("NyamuGo");
                StartCoroutine(PlayNyamuGo());
            }
            else
            {
                Debug.Log("NyamuCome");
                StartCoroutine(PlayNyamuCome());
            }
            ticker = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Mad");
            PlayMad();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("NyamuCome");
            StartCoroutine(PlayNyamuCome());
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("NyamuGo");
            StartCoroutine(PlayNyamuGo());
        }
    }

    void PlayBlink()
    {
        var animationState = armature.animation.GetState("Blink");
        if (animationState == null || !animationState.isPlaying)
        {
            // 叠加 Blink 动画（Track 1）
            animationState = armature.animation.FadeIn("Blink", 0.1f, 1, 1, "BlendLayer");

            animationState.AddBoneMask("L_eyelid");
            animationState.AddBoneMask("R_eyelid");
            animationState.AddBoneMask("L_eyeball");
            animationState.AddBoneMask("R_eyeball");
        }
    }
    private void PlayMad()
    {
        StartCoroutine(PlayMadSmooth());
        if (!isMad)
        {
            foreach (Slot slot in L_EyeSlots) slot.displayIndex = -1;
            foreach (Slot slot in R_EyeSlots) slot.displayIndex = -1;
            foreach (Slot slot in L_EyeMadSlots) slot.displayIndex = 0;
            foreach (Slot slot in R_EyeMadSlots) slot.displayIndex = 0;
            madSlot.displayIndex = 0;
        }
        else
        {
            foreach (Slot slot in L_EyeMadSlots) slot.displayIndex = -1;
            foreach (Slot slot in R_EyeMadSlots) slot.displayIndex = -1;
            madSlot.displayIndex = -1;
            foreach (Slot slot in L_EyeSlots) slot.displayIndex = 0;
            foreach (Slot slot in R_EyeSlots) slot.displayIndex = 0;
        }

        isMad = !isMad;
    }
    private IEnumerator PlayMadSmooth()
    {
        float elapsedTime = 0f;
        float startAlpha = isMad ? 1f : 0f;
        float endAlpha = isMad ? 0f : 1f;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionTime);
            ColorTransform normalEyeColor = new ColorTransform();
            normalEyeColor.alphaMultiplier = 1 - alpha;
            ColorTransform madEyeColor = new ColorTransform();
            madEyeColor.alphaMultiplier = alpha;

            // 设置普通眼睛渐隐
            foreach (Slot slot in L_EyeSlots) slot._SetColor(normalEyeColor);
            foreach (Slot slot in R_EyeSlots) slot._SetColor(normalEyeColor);

            // 设置愤怒眼睛渐显
            foreach (Slot slot in L_EyeMadSlots) slot._SetColor(madEyeColor);
            foreach (Slot slot in R_EyeMadSlots) slot._SetColor(madEyeColor);
            madSlot._SetColor(madEyeColor);

            yield return null;
        }
    }
    private IEnumerator PlayNyamuCome()
    {
        foreach (Slot slot in NyamuSlots)
        {
            slot.displayIndex = 0;
        }
        var animationState = armature.animation.GetState("NyamuCome");
        if (animationState == null || !animationState.isPlaying)
        {
            // 叠加 NyamuCome 动画（Track 1）
            animationState = armature.animation.FadeIn("NyamuCome", 0f, 1, 0, "BlendLayer");
        }
        yield return new WaitUntil(() => animationState.isPlaying == false);

        PlayMad();
    }
    public IEnumerator PlayNyamuGo()
    {
        var animationState = armature.animation.GetState("NyamuGo");
        if (animationState == null || !animationState.isPlaying)
        {
            // 叠加 NyamuGo 动画（Track 1）
            animationState = armature.animation.FadeIn("NyamuGo", 0.1f, 1, 0, "BlendLayer");
        }

        // 等待动画播放完毕
        yield return new WaitUntil(() => animationState.isPlaying == false);

        foreach (Slot slot in NyamuSlots)
        {
            slot.displayIndex = -1;
        }
        PlayMad();
    }
    public void ResetTicker()
    {
        ticker = 0f;
    }
}