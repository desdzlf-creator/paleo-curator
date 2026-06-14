using UnityEngine;

// Script penanda — attach ke setiap GameObject slot di prefab level lo
// HintManager akan otomatis nemuin semua slot yang punya script ini
public class SlotTulang : MonoBehaviour
{
    // Kosong — fungsinya cuma sebagai penanda/tag via script
    // biar HintManager bisa GetComponentsInChildren<SlotTulang>()
}