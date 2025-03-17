using UnityEngine;
using DragonBones;

public class EyeTracking : MonoBehaviour
{
    public UnityArmatureComponent armature;
    public string leftEyeBoneName = "L_eyeball";  // 左眼骨骼名称
    public string rightEyeBoneName = "R_eyeball"; // 右眼骨骼名称
    public string handBoneName = "R_hand"; // 手骨骼名称
    public float trackingSpeed = 5f; // 眼睛跟随速度
    public float maxOffset = 0.25f; // 眼睛最大偏移量（防止移出眼眶）
    public AnimationController animationController;

    private Bone leftEyeBone;
    private Bone rightEyeBone;
    private Camera mainCamera;
    private Vector2 leftEyeInitialPos;
    private Vector2 rightEyeInitialPos;
    private Vector2 eyeInitialPos;
    private Vector3 eyeCenterPos; // 眼睛中心点
    private Vector3 mouseWorldPos; // 鼠标世界坐标
    private Vector3 mouseLocalPos; // 鼠标局部坐标
    private Vector2 offset; // 偏移量
    private DragonBones.AnimationState animationState;

    void Start()
    {
        mainCamera = Camera.main;
        armature = GetComponent<UnityArmatureComponent>();
        animationController = GetComponent<AnimationController>();
        var arm = armature.armature;

        // 获取眼睛骨骼
        leftEyeBone = arm.GetBone(leftEyeBoneName);
        rightEyeBone = arm.GetBone(rightEyeBoneName);

        leftEyeInitialPos = new Vector2(leftEyeBone.offset.x, leftEyeBone.offset.y);
        rightEyeInitialPos = new Vector2(rightEyeBone.offset.x, rightEyeBone.offset.y);

        // 记录眼睛的初始位置（取两只眼睛的平均位置）
        eyeInitialPos = (leftEyeInitialPos + rightEyeInitialPos) / 2;

        animationState = armature.animation.GetState("Idle");
    }

    void Update()
    {
        // animationState = armature.animation.GetState("Idle");

        // if holding left mouse button, move eyes
        if (Input.GetMouseButton(0))
        {
            MoveEyes();
        }
        else
        {
            // reset eye position
            leftEyeBone.offset.x = Mathf.Lerp(leftEyeBone.offset.x, eyeInitialPos.x, Time.deltaTime * trackingSpeed);
            leftEyeBone.offset.y = Mathf.Lerp(leftEyeBone.offset.y, eyeInitialPos.y, Time.deltaTime * trackingSpeed);

            rightEyeBone.offset.x = Mathf.Lerp(rightEyeBone.offset.x, eyeInitialPos.x, Time.deltaTime * trackingSpeed);
            rightEyeBone.offset.y = Mathf.Lerp(rightEyeBone.offset.y, eyeInitialPos.y, Time.deltaTime * trackingSpeed);
        }
        if (Input.GetMouseButton(0) || animationController.isMad)
        {
            if (animationState != null)
            {
                animationState.AddBoneMask(""); // **让所有骨骼受影响**
                animationState.RemoveBoneMask(handBoneName); // **只排除 R_hand**
            }
        }
        else
        {
            if (animationState != null)
            {
                animationState.RemoveAllBoneMask();
            }
        }
    }
    private void MoveEyes()
    {
        if (leftEyeBone == null || rightEyeBone == null)
        {
            Debug.LogWarning("Bone not found.");
            return;
        }

        // 获取鼠标世界坐标
        mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        eyeCenterPos = (armature.transform.TransformPoint(new Vector3(leftEyeBone.global.x, leftEyeBone.global.y, 0)) +
                        armature.transform.TransformPoint(new Vector3(rightEyeBone.global.x, rightEyeBone.global.y, 0))) / 2;

        // 计算偏移量（相对于眼睛中心点）
        float offsetX = Mathf.Clamp((mouseWorldPos.x - eyeCenterPos.x) * 0.5f, -maxOffset, maxOffset);
        float offsetY = Mathf.Clamp((mouseWorldPos.y - eyeCenterPos.y) * 0.5f, -maxOffset, maxOffset);

        // 使用 Lerp 让两只眼睛同步移动
        leftEyeBone.offset.x = Mathf.Lerp(leftEyeBone.offset.x, eyeInitialPos.x + offsetY, Time.deltaTime * trackingSpeed);
        leftEyeBone.offset.y = Mathf.Lerp(leftEyeBone.offset.y, eyeInitialPos.y + offsetX, Time.deltaTime * trackingSpeed);

        rightEyeBone.offset.x = Mathf.Lerp(rightEyeBone.offset.x, eyeInitialPos.x + offsetY, Time.deltaTime * trackingSpeed);
        rightEyeBone.offset.y = Mathf.Lerp(rightEyeBone.offset.y, eyeInitialPos.y + offsetX, Time.deltaTime * trackingSpeed);
    }
    void OnDrawGizmos()
    {
        if (leftEyeBone == null || rightEyeBone == null) return;

        Gizmos.color = Color.green;
        // 绘制眼睛中心点
        Gizmos.DrawSphere(eyeCenterPos, 0.2f);

        Gizmos.color = Color.red;
        // 绘制鼠标目标位置
        Gizmos.DrawSphere(mouseWorldPos, 0.2f);

        Gizmos.color = Color.blue;
        // 绘制偏移量（用短线段表示）
        Gizmos.DrawLine(eyeCenterPos, eyeCenterPos + new Vector3(offset.x, offset.y, 0));
    }
}