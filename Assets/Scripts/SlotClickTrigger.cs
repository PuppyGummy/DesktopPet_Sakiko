using UnityEngine;
using DragonBones;

public class SlotClickTrigger : MonoBehaviour
{
    public UnityArmatureComponent armatureComp;
    public AnimationController animationController;

    void Start()
    {
        armatureComp = GetComponent<UnityArmatureComponent>();
        animationController = GetComponent<AnimationController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localPosition = armatureComp.transform.InverseTransformPoint(worldPosition);

            CheckBoundingBoxClick(localPosition);
        }
    }

    void CheckBoundingBoxClick(Vector3 localPosition)
    {
        var slot = armatureComp.armature.ContainsPoint(localPosition.x, localPosition.y);
        if (slot != null)
        {
            Debug.Log("Slot clicked!");
            StartCoroutine(animationController.PlayNyamuGo());
            animationController.ResetTicker();
        }
    }
}
