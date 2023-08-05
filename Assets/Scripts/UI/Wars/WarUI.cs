using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class WarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private WarMemberUI _slotPrefab;
    [SerializeField] private WarSideMembersUI _aggressorsSide;
    [SerializeField] private WarSideMembersUI _defendersSide;


    public void RefreshUI(War war)
    {
        _descriptionText.text = war.GetWarName();
        var memebers = war.GetMembers();
        _aggressorsSide.RefreshUI(memebers.FindAll(member => member.MemberType == WarMemberType.Aggressor), _slotPrefab);
        _defendersSide.RefreshUI(memebers.FindAll(member => member.MemberType == WarMemberType.Defender), _slotPrefab);
        war.OnEnd += delegate 
        {
            gameObject.SetActive(false);
        };
    }
}
