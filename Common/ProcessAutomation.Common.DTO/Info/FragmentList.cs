using System.Collections.Generic;
using System.Linq;

namespace ProcessAutomation.Common.DTO.Info
{
    public class FragmentList : List<FragmentInfo>
    {
        //public string GetVariableByFragment(string fragment)
        //{
        //    string variable = null;
        //    if (!string.IsNullOrWhiteSpace(fragment))
        //    {
        //        variable = this.FirstOrDefault(f => f.Fragment.Trim() == fragment.Trim()).Variable;
        //    }
        //    return variable;
        //}

        //public string this[string fragment]
        //{
        //    get => GetVariableByFragment(fragment);
        //}

        //public List<FragmentInfo> GetAllByIndexAscendent()
        //{
        //    return this.OrderBy(s => s.FragmentIndex).ThenBy(s => s.VariableIndex).ToList();
        //}

        //public List<FragmentInfo> GetAllByIndexDescendent()
        //{
        //    return this.OrderByDescending(s => s.FragmentIndex).ThenByDescending(s => s.VariableIndex).ToList();
        //}

        //public void AddVariable(string fragmentName, string fragment, string variable, int fragmentIndex = 0)
        //{
        //    FragmentInfo fragmentInfo = this.FirstOrDefault(f => f.FragmentIndex == fragmentIndex);
        //    if (fragmentInfo != null)
        //    {
        //    }
        //}

        public void Add(FragmentInfo fragmentInfo, bool addIndex)
        {
            int index = 0;
            if (addIndex)
            {
                index = base.Count + 1;
            }

            fragmentInfo.EndPosition = fragmentInfo.StartPosition + fragmentInfo.Fragment!.Length;
            fragmentInfo.Length = fragmentInfo.Fragment.Length;
            fragmentInfo.FragmentIndex = index;

            base.Add(fragmentInfo);

            //base.Add(new FragmentInfo
            //{
            //    FragmentName = fragmentInfo.FragmentName,
            //    Fragment = fragmentInfo.Fragment,
            //    StartPosition = fragmentInfo.StartPosition,
            //    EndPosition = fragmentInfo.StartPosition + fragmentInfo.Fragment.Length,
            //    Length = fragmentInfo.Fragment.Length,
            //    FragmentIndex = index
            //});
        }

        public void Add(string fragmentName, string fragment, int startPosition, int endPosition)
        {
            base.Add(new FragmentInfo
            {
                FragmentName = fragmentName,
                Fragment = fragment,
                StartPosition = startPosition,
                EndPosition = endPosition,
                FragmentIndex = base.Count + 1
            });
        }
    }
}
