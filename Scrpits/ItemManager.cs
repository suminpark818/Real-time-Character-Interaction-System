using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ItemManager : MonoBehaviour
{
    public Transform attachPoint;
    public TMP_Text equippedItemText; // 장착된 아이템 이름을 표시할 텍스트 (UI에서 연결)

    private Dictionary<string, GameObject> equippedItems = new Dictionary<string, GameObject>();

    // 아이템 장착
    public void EquipItem(string itemName)
    {
        // 기존 아이템 제거
        foreach (Transform child in attachPoint)
        {
            Destroy(child.gameObject);
        }

        // 프리팹 로드 및 장착
        GameObject prefab = Resources.Load<GameObject>("Items/" + itemName);
        if (prefab != null)
        {
            GameObject item = Instantiate(prefab, attachPoint);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            equippedItems[itemName] = item;

            // 텍스트 업데이트
            if (equippedItemText != null)
            {
                equippedItemText.text = "Equipped: " + itemName;
                equippedItemText.ForceMeshUpdate(); // 텍스트 즉시 반영
            }
            else
            {
                Debug.LogWarning("equippedItemText is NULL on EquipItem");
            }
        }
    }

    // 특정 아이템 제거
    public void RemoveItem(string itemName)
    {
        if (equippedItems.ContainsKey(itemName))
        {
            Destroy(equippedItems[itemName]);
            equippedItems.Remove(itemName);

            if (equippedItemText != null)
            {
                equippedItemText.text = "Equipped : None";
                equippedItemText.ForceMeshUpdate();
            }
        }
    }

    // 전체 제거 (Clear 버튼 연결용)
    public void RemoveAll()
    {
        foreach (var item in equippedItems.Values)
        {
            Destroy(item);
        }
        equippedItems.Clear();

        if (equippedItemText != null)
        {
            equippedItemText.text = "Equipped : None";
            equippedItemText.ForceMeshUpdate(); // 강제 렌더링
        }
        else
        {
            Debug.LogWarning("equippedItemText is NULL on RemoveAll");
        }
    }
}
