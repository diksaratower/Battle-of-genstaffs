using System;
using System.Collections.Generic;


public class CountryDiplomacy
{
    public Action<Ultimatum> OnGetUltimatum;
    public Action<WarGoal> OnAddWarGoal;
    public Action<UltimatumAnswerType, Ultimatum> OnGetUltimatumAnser;

    private List<WarGoal> _warGoals = new List<WarGoal>();
    private List<WarGoalJustificationQueueSlot> _justificationQueue = new List<WarGoalJustificationQueueSlot>();
    private List<Ultimatum> _ultimatums = new List<Ultimatum>();
    private const int _justificationTimeDays = 5;
    private Country _country;

    public CountryDiplomacy(Country country)
    {
        _country = country;
        GameTimer.DayEnd += CalculateDiplomacy;
    }

    private void CalculateDiplomacy()
    {
        CalculateWarGoalJustification();
        CalculateUltimatums();
    }

    public void StartJustificationWarGoal(Country target)
    {
        var slot = new WarGoalJustificationQueueSlot(_justificationTimeDays, target);
        _justificationQueue.Add(slot);
    }

    public List<WarGoalJustificationQueueSlot> GetJustificationQueue()
    {
        return new List<WarGoalJustificationQueueSlot>(_justificationQueue);
    }

    public void DeclareWarToCountry(Country target) 
    {
        if (IsHaveWarGoal(target))
        {
            Diplomacy.Instance.DeclareWar(_country, target);
        }
    }

    public void AddWarGoal(Country target)
    {
        var warGoal = new WarGoal(target);
        _warGoals.Add(warGoal);
        OnAddWarGoal?.Invoke(warGoal);
    }

    public void SendUltimatum(Ultimatum ultimatum)
    {
        _ultimatums.Add(ultimatum);
        OnGetUltimatum?.Invoke(ultimatum);
    }

    public bool IsHaveWarGoal(Country countryTarget)
    {
        return _warGoals.Exists(warGoal => warGoal.Target == countryTarget);
    }

    private void CalculateUltimatums()
    {
        var forRemove = new List<Ultimatum>();
        foreach (var ultimatum in _ultimatums)
        {
            if (ultimatum.IsAnsered)
            {
                forRemove.Add(ultimatum);
                continue;
            }
            ultimatum.GetAutoAnserProgressDays++;
            if (ultimatum.GetAutoAnserProgressDays >= ultimatum.GetAutoAnserTimeDays)
            {
                ultimatum.SendAutoAnser();
                forRemove.Add(ultimatum);
            }
        }
        _ultimatums.RemoveAll(slot => forRemove.Contains(slot));
    }

    private void CalculateWarGoalJustification()
    {
        var forRemove = new List<WarGoalJustificationQueueSlot>();
        foreach (var slot in _justificationQueue)
        {
            slot.JustificationProgress++;
            if (slot.JustificationProgress >= slot.JustificationTimeDays)
            {
                forRemove.Add(slot);
            }
        }
        _justificationQueue.RemoveAll(slot => forRemove.Contains(slot));
        forRemove.ForEach(slot => 
        {
            AddWarGoal(slot.Target);
        });
    }
}

public class WarGoalJustificationQueueSlot
{
    public int JustificationTimeDays { get; }
    public Country Target { get; }
    public int JustificationProgress;

    public WarGoalJustificationQueueSlot(int justificationTimeDays, Country target)
    {
        JustificationTimeDays = justificationTimeDays;
        Target = target;
    }
}

public class WarGoal
{
    public Country Target { get; }

    public WarGoal(Country target) 
    {
        Target = target;
    }
}

public abstract class Ultimatum
{
    public int GetAutoAnserProgressDays;
    public bool IsAnsered { get; private set; }
    public Country Sender { get; }
    public Country Target { get; }
    public int GetAutoAnserTimeDays { get; }
    public Action<UltimatumAnswerType> OnAnsered;

    protected UltimatumAnswerType _autoAnser = UltimatumAnswerType.No;

    public Ultimatum(Country sender, Country target)
    {
        Target = target;
        Sender = sender;
        GetAutoAnserTimeDays = 15;
        OnAnsered += (UltimatumAnswerType answerType) => 
        { 
            sender.CountryDiplomacy.OnGetUltimatumAnser?.Invoke(answerType, this); 
        };
    }

    public void SendAutoAnser()
    {
        SendAnser(_autoAnser);
    }

    public void SendAnser(UltimatumAnswerType ultimatumAnswer)
    {
        IsAnsered = true;
        AnserEffect(ultimatumAnswer);
        OnAnsered?.Invoke(ultimatumAnswer);
    }

    protected abstract void AnserEffect(UltimatumAnswerType ultimatumAnswer);
}

public class AnnexCountryUltimatum : Ultimatum
{
    public AnnexCountryUltimatum(Country sender, Country target) : base(sender, target)
    {
    }

    protected override void AnserEffect(UltimatumAnswerType ultimatumAnswer)
    {
        if (ultimatumAnswer == UltimatumAnswerType.Yes)
        {
            Target.AnnexCountry(Sender);
        }
        if (ultimatumAnswer == UltimatumAnswerType.No)
        {
            Sender.CountryDiplomacy.AddWarGoal(Target);
        }
    }
}

public class AnnexRegionUltimatum : Ultimatum
{
    public Region AnnexedRegion { get; }

    public AnnexRegionUltimatum(Country sender, Country target, Region annexedRegion) : base(sender, target)
    {
        AnnexedRegion = annexedRegion;
    }

    protected override void AnserEffect(UltimatumAnswerType ultimatumAnswer)
    {
        if (ultimatumAnswer == UltimatumAnswerType.Yes)
        {
            AnnexedRegion.AnnexRegion(Sender, Target);
        }
        if (ultimatumAnswer == UltimatumAnswerType.No)
        {
            Sender.CountryDiplomacy.AddWarGoal(Target);
        }
    }
}

public enum UltimatumAnswerType
{
    Yes,
    No
}