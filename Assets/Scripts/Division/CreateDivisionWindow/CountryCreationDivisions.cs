using System;
using System.Collections.Generic;
using UnityEngine;


public class CountryCreationDivisions
{
    public Action OnAddedSlotToQueue;
    public Action OnRemovedSlotFromQueue;
    public List<CreationDivisionsQueueSlot> CreationQueue = new List<CreationDivisionsQueueSlot>();
    public readonly int MaxQueueSlots = 20;

    private readonly Country _country;

    public CountryCreationDivisions(Country country) 
    {
        _country = country;
        GameTimer.DayEnd += CalculateDivisionCreation;
    }

    public void AddDivisionCreation(DivisionTemplate divisionTemplate, Province divisionProvince, string divisionName)
    {
        if (CreationQueue.Count > MaxQueueSlots)
        {
            throw new TryToAddMoreMaxDivisionsToTheCreationException();
        }
        var slot = new CreationDivisionsQueueSlot(divisionTemplate, divisionProvince, divisionName);
        CreationQueue.Add(slot);
        OnAddedSlotToQueue?.Invoke();
    }

    public void RemoveDivisionCreation(CreationDivisionsQueueSlot divisionsQueueSlot)
    {
        CreationQueue.Remove(divisionsQueueSlot);
        OnRemovedSlotFromQueue?.Invoke();
    }

    private void CalculateDivisionCreation()
    {
        var divisionsForCreate = new List<CreationDivisionsQueueSlot>();

        for (int i = 0; i < 10; i++)
        {
            if (i == CreationQueue.Count || i > CreationQueue.Count)
            {
                break;
            }
            CreationQueue[i].CreationProgressDays += 1;
            if (CreationQueue[i].CreationProgressDays >= CreationQueue[i].CretionTimeDays || Cheats.InstantTraingDivision)
            {
                divisionsForCreate.Add(CreationQueue[i]);
            }
        }

        foreach (var slot in divisionsForCreate)
        {
            var division = UnitsManager.Instance.AddDivision(slot.DivisionProvince, slot.DivisionTemplate, _country);
            if (slot.DivisionTemplate.Battalions.Count > 0)
            {
                division.SetTemplate(slot.DivisionTemplate);
                division.Name = slot.DivisionName;
            }
            RemoveDivisionCreation(slot);
        }
    }

    public class TryToAddMoreMaxDivisionsToTheCreationException : Exception
    {

    }
}

public class CreationDivisionsQueueSlot
{
    public int CreationProgressDays = 0;
    public DivisionTemplate DivisionTemplate { get; }
    public Province DivisionProvince { get; }
    public string DivisionName { get; }
    public readonly int CretionTimeDays = 20;

    public CreationDivisionsQueueSlot(DivisionTemplate divisionTemplate, Province divisionProvince, string divisionName)
    {
        DivisionTemplate = divisionTemplate;
        DivisionProvince = divisionProvince;
        DivisionName = divisionName;
    }
}