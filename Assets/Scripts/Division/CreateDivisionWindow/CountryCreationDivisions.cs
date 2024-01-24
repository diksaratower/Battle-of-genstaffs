using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


public class CountryCreationDivisions
{
    public Action OnAddedSlotToQueue;
    public Action OnRemovedSlotFromQueue;
    public ReadOnlyCollection<CreationDivisionsQueueSlot> CreationQueue => _creationQueue.AsReadOnly();
    public readonly int MaxQueueSlots = 20;

    private List<CreationDivisionsQueueSlot> _creationQueue = new List<CreationDivisionsQueueSlot>();
    private Country _country;


    public CountryCreationDivisions(Country country) 
    {
        _country = country;
        GameTimer.DayEnd += CalculateDivisionCreation;
        _country.OnAnnexed += delegate 
        {
            ClearQueue();
        };
        _country.OnCapitulated += delegate 
        {
            ClearQueue();
        };
    }

    public void AddDivisionCreation(DivisionTemplate divisionTemplate, Province divisionProvince, string divisionName)
    {
        if (_creationQueue.Count > MaxQueueSlots)
        {
            throw new TryToAddMoreMaxDivisionsToTheCreationException();
        }
        var slot = new CreationDivisionsQueueSlot(divisionTemplate, divisionProvince, divisionName);
        _creationQueue.Add(slot);
        OnAddedSlotToQueue?.Invoke();
    }

    public void RemoveDivisionCreation(CreationDivisionsQueueSlot divisionsQueueSlot)
    {
        _creationQueue.Remove(divisionsQueueSlot);
        OnRemovedSlotFromQueue?.Invoke();
    }

    private void ClearQueue()
    {
        var queue = new List<CreationDivisionsQueueSlot>(_creationQueue);
        foreach (var slot in queue)
        {
            RemoveDivisionCreation(slot);
        }
    }

    private void CalculateDivisionCreation()
    {
        var divisionsForCreate = new List<CreationDivisionsQueueSlot>();

        for (int i = 0; i < 10; i++)
        {
            if (i == _creationQueue.Count || i > _creationQueue.Count)
            {
                break;
            }
            _creationQueue[i].CreationProgressDays += 1;
            if (_creationQueue[i].CreationProgressDays >= _creationQueue[i].CretionTimeDays || Cheats.InstantTraingDivision)
            {
                divisionsForCreate.Add(_creationQueue[i]);
            }
        }

        foreach (var slot in divisionsForCreate)
        {
            if (slot.DivisionProvince.Owner != _country)
            {
                throw new Exception("Try spawn divisions in stranger province.");
            }
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