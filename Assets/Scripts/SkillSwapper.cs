using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillSwapper : MonoBehaviour
{
    public RectTransform primarySlot;
    public Image primaryImage;

    public RectTransform[] secondarySlots;
    public Image[] secondaryImages;

    public float swapDuration = 0.4f;

    private bool isSwapping = false;

    public void OnSecondarySkillClicked(int index)
    {
        if (isSwapping || index < 0 || index >= secondarySlots.Length) return;
        StartCoroutine(SwapSkills(index));
    }

    private IEnumerator SwapSkills(int index)
    {
        isSwapping = true;

        RectTransform selectedSlot = secondarySlots[index];
        Image selectedImage = secondaryImages[index];

        // Get world positions
        Vector3 startA = primarySlot.position;
        Vector3 startB = selectedSlot.position;

        // Center point for circular path
        Vector3 center = (startA + startB) / 2f;
        float radius = Vector3.Distance(startA, center);

        // Direction vector from center to each orb
        Vector3 dirA = (startA - center).normalized;
        Vector3 dirB = (startB - center).normalized;

        // Store original sprites
        Sprite spriteA = primaryImage.sprite;
        Sprite spriteB = selectedImage.sprite;

        // Create temp visuals
        GameObject tempA = CreateTempOrb(primaryImage, startA);
        GameObject tempB = CreateTempOrb(selectedImage, startB);

        // Hide originals
        primaryImage.enabled = false;
        selectedImage.enabled = false;

        float duration = swapDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float angle = Mathf.Lerp(0, Mathf.PI, t); // Rotate 180 degrees

            // Rotate both around center in opposite directions
            Vector3 posA = center + new Vector3(
                Mathf.Cos(angle) * radius * dirA.x - Mathf.Sin(angle) * radius * dirA.y,
                Mathf.Sin(angle) * radius * dirA.x + Mathf.Cos(angle) * radius * dirA.y,
                0f);

            Vector3 posB = center + new Vector3(
                Mathf.Cos(angle) * radius * dirB.x - Mathf.Sin(angle) * radius * dirB.y,
                Mathf.Sin(angle) * radius * dirB.x + Mathf.Cos(angle) * radius * dirB.y,
                0f);

            tempA.transform.position = posA;
            tempB.transform.position = posB;

            yield return null;
        }

        // Cleanup
        Destroy(tempA);
        Destroy(tempB);

        // Swap sprites
        primaryImage.sprite = spriteB;
        selectedImage.sprite = spriteA;

        // Show updated originals
        primaryImage.enabled = true;
        selectedImage.enabled = true;

        isSwapping = false;
    }

    private GameObject CreateTempOrb(Image sourceImage, Vector3 startPosition)
    {
        GameObject temp = new GameObject("TempOrb", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        temp.transform.SetParent(transform.root, false);
        temp.transform.position = startPosition;

        Image img = temp.GetComponent<Image>();
        img.sprite = sourceImage.sprite;
        img.raycastTarget = false;
        img.SetNativeSize();

        RectTransform rt = temp.GetComponent<RectTransform>();
        rt.sizeDelta = sourceImage.rectTransform.sizeDelta;
        rt.localScale = sourceImage.rectTransform.lossyScale;

        return temp;
    }
}
