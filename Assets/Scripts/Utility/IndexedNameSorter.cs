using UnityEngine;
using System.Collections;

class IndexedNameSorter : IComparer
//class RaycastSorter extends IComparer //in unity 2.6
{
    public int Compare(object a, object b)
    {
		int aValue = ExtractNumber(((Transform)a).name);
		int bValue = ExtractNumber(((Transform)b).name);
		
        return aValue.CompareTo(bValue);
    }
	
	private int ExtractNumber(string input)
	{
		int number = int.Parse(input.Split('_')[1]);
		
		return number;
	}
}
