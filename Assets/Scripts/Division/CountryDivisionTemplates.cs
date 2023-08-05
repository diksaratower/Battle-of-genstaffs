using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Country;

public class CountryDivisionTemplates
{
    public List<DivisionTemplate> Templates = new List<DivisionTemplate>();

    public TemplateConstrSerialize GetSerialize()
    {
        return new TemplateConstrSerialize(this);
    }

    public void LoadFromSerialize(TemplateConstrSerialize ser)
    {
        ser.Load(this);
    }

    [Serializable]
    public class TemplateConstrSerialize : SerializeForSave
    {
        public List<DivTamplateSave> Templates = new List<DivTamplateSave>();

        public TemplateConstrSerialize(CountryDivisionTemplates templates)
        {
            foreach (var temp in templates.Templates)
            {
                var dTemp = new DivTamplateSave() { Name = temp.Name };
                dTemp.SetData(temp);
                Templates.Add(dTemp);
            }
        }

        public TemplateConstrSerialize(string jsonSave)
        {
            JsonUtility.FromJsonOverwrite(jsonSave, this);
        }

        public override void Load(object objTarget)
        {
            var constructor = objTarget as CountryDivisionTemplates;
            constructor.Templates.Clear();
            foreach (var saveTemplate in Templates)
            {
                var newTemp = new DivisionTemplate();
                saveTemplate.SetDataToTemplate(newTemp);
                constructor.Templates.Add(newTemp);
            }
        }

        public override string SaveToJson()
        {
            return JsonUtility.ToJson(this);
        }
        [Serializable]
        public class DivTamplateSave
        {
            public string Name;
            public List<string> battalionLines = new List<string>();

            public void SetData(DivisionTemplate template)
            {
                battalionLines = new List<string>();
                //Debug.Log(template);
                foreach (var line in template.DivisionLines)
                {
                    var saveLine = new BattalionLineSave(line);

                    battalionLines.Add(JsonUtility.ToJson(saveLine));
                }
            }

            public void SetDataToTemplate(DivisionTemplate template)
            {
                template.Name = Name;
                template.DivisionLines.Clear();
                foreach (var sline in battalionLines)
                {
                    var saveLine = new BattalionLineSave(sline);
                    var line = new DivisionLine();
                    foreach (var sl in saveLine.Battalions)
                    {
                        //var constructor = DivisionTemplateConstructorUI.GetInstance();
                        line.Battalions.Add(TechnologiesManagerSO.GetInstance().AvailableBattalions.Find(b => b.name == sl.Name));
                    }
                    template.DivisionLines.Add(line);
                }
            }
        }
        [Serializable]
        public class BattalionSave
        {
            public string Name;
        }
        [Serializable]
        public class BattalionLineSave
        {
            public List<BattalionSave> Battalions = new List<BattalionSave>();
            public BattalionLineSave(DivisionLine divisionLine)
            {
                foreach (var bat in divisionLine.Battalions)
                {
                    Battalions.Add(new BattalionSave() { Name = bat.name });
                }
            }
            public BattalionLineSave(string data)
            {
                JsonUtility.FromJsonOverwrite(data, this);
            }
        }
    }
}
