using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropTulang : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Target Snap System")]
    public Transform targetSlot; 
    public float jarakSnapMaksimal = 0.5f; 

    private Vector3 posisiAwalDiTray;
    private bool sudahTerkunci = false;

    //tulang mulai disentuh
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (sudahTerkunci) return;
        posisiAwalDiTray = transform.localPosition; 
    }

    //tulang digeser
    public void OnDrag(PointerEventData eventData)
    {
        if (sudahTerkunci) return;
        transform.position = Input.mousePosition; 
    }

    //tulang dilepas
    public void OnEndDrag(PointerEventData eventData)
    {
        if (sudahTerkunci) return;

        
        float jarakDunia = Vector3.Distance(transform.position, targetSlot.position);

        
        Debug.Log("Jarak Dunia saat dilepas: " + jarakDunia);

        if (jarakDunia <= jarakSnapMaksimal)
        {
           
            transform.position = targetSlot.position;
            sudahTerkunci = true;

            PuzzleManager pm = FindFirstObjectByType<PuzzleManager>();
            if (pm != null)
            {
                pm.TulangBerhasilDisusun();
            }
        }
        else
        {
            transform.localPosition = posisiAwalDiTray;
        }
    }
}