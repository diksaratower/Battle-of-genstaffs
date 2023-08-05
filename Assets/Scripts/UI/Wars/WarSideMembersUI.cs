using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WarSideMembersUI : MonoBehaviour
{
    [SerializeField] private Transform _membersUIParent;
    
    private List<WarMemberUI> _slots = new List<WarMemberUI>();


    public void RefreshUI(List<WarMember> warSideMembers, WarMemberUI memberUIPrefab)
    {
        _slots.ForEach(slot => Destroy(slot.gameObject));
        _slots.Clear();
        foreach (var warMember in warSideMembers)
        {
            SpawnSlot(warMember, _membersUIParent, memberUIPrefab);
        }
    }

    private void SpawnSlot(WarMember country, Transform parent, WarMemberUI slotPrefab)
    {
        var slot = Instantiate(slotPrefab, parent);
        slot.RefreshUI(country);
        _slots.Add(slot);
    }
}
