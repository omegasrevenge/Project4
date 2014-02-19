using System.Collections.Generic;
using System.Linq;

public class FarmResult 
{
    public class Driod
    {
        public int Element;
        public int Level;
        public int Count = 0;
    }

    private List<Driod> _driods = new List<Driod>();

    public void AddResult(int element, int level, int count)
    {
        Driod driod = _driods.FirstOrDefault(d => d.Element == element && d.Level == level);
        if (driod == null)
        {
            driod = new Driod() {Element = element, Level = level};
            _driods.Add(driod);
        }
        driod.Count += count;
    }

    public Driod[] GetDriods()
    {
        _driods.Sort(delegate(Driod d1, Driod d2)
        {
            int compareDate = d1.Element.CompareTo(d2.Element);
            if (compareDate == 0)
            {
                return d1.Level.CompareTo(d2.Level);
            }
            return compareDate;
        });
        return _driods.ToArray();
    }
}
