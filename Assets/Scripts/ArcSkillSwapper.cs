using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ArcSkillSwapper : MonoBehaviour
{
    [System.Serializable]
    public class SkillOrb
    {
        public string skillName;
        public Button button;
        public Image image;
        public RectTransform rectTransform => image.rectTransform;
    }

    public List<Transform> slotPositions;      // Slot0, Slot1, PrimarySlot, Slot3, Slot4
    public List<SkillOrb> skillOrbs;           // Skills currently assigned
    public Transform rotationCenter;           // Set this to the true center of the circular arc
    public float moveDuration = 0.5f;

    private const int primarySlotIndex = 2;
    private bool isRotating = false;

    private void Start()
    {
        foreach (var orb in skillOrbs)
        {
            orb.button.onClick.RemoveAllListeners();
            orb.button.onClick.AddListener(() => OnSkillClicked(orb));
        }

        for (int i = 0; i < skillOrbs.Count; i++)
            skillOrbs[i].rectTransform.position = slotPositions[i].position;
    }

    public void OnSkillClicked(SkillOrb clickedOrb)
    {
        if (isRotating) return;

        int clickedIndex = skillOrbs.IndexOf(clickedOrb);
        if (clickedIndex == -1) return;

        int steps = (primarySlotIndex - clickedIndex + skillOrbs.Count) % skillOrbs.Count;
        if (steps == 0) return;

        StartCoroutine(RotateOrbsCircularly(steps));
    }

    private IEnumerator RotateOrbsCircularly(int steps)
    {
        isRotating = true;

        Vector3 center = rotationCenter.position;

        List<float> startAngles = new List<float>();
        List<float> endAngles = new List<float>();
        List<float> radii = new List<float>();

        for (int i = 0; i < skillOrbs.Count; i++)
        {
            Vector3 fromCenter = skillOrbs[i].rectTransform.position - center;
            float radius = fromCenter.magnitude;
            float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;

            startAngles.Add(angle);
            radii.Add(radius);

            int newIndex = (i + steps) % skillOrbs.Count;
            Vector3 toCenter = slotPositions[newIndex].position - center;
            float targetAngle = Mathf.Atan2(toCenter.y, toCenter.x) * Mathf.Rad2Deg;
            endAngles.Add(targetAngle);
        }

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration);

            for (int i = 0; i < skillOrbs.Count; i++)
            {
                float start = startAngles[i];
                float end = endAngles[i];

                while (end <= start)
                    end += 360f;

                float angle = Mathf.Lerp(start, end, t) % 360f;
                float rad = radii[i];

                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * rad;
                skillOrbs[i].rectTransform.position = center + offset;
            }

            yield return null;
        }

        // Snap to final position
        for (int i = 0; i < skillOrbs.Count; i++)
            skillOrbs[i].rectTransform.position = slotPositions[(i + steps) % slotPositions.Count].position;

        RotateListLeft(skillOrbs, steps);

        isRotating = false;
    }

    private void RotateListLeft<T>(List<T> list, int steps)
    {
        int count = list.Count;
        List<T> rotated = new List<T>(count);

        for (int i = 0; i < count; i++)
            rotated.Add(list[(i + steps) % count]);

        for (int i = 0; i < count; i++)
            list[i] = rotated[i];
    }
}