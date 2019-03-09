using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.Files.Xml
{
    public abstract class TagItem
    {
        protected abstract bool MatchesName(String name);
        protected readonly WeakReference<TagFile> _Parent = new WeakReference<TagFile>(null);

        public void Display()
        {
            this.Display(" ");
        }

        public abstract void Display(String prec);
        public abstract IEnumerable<TagItem> Enumerate();

        public TagFile GetParent(int levels = 1)
        {
            if (levels < 1) throw new Exception("TagItem GetParent levels less than 1");
            TagFile parent;
            if (this._Parent.TryGetTarget(out parent))
            {
                if (levels == 1) return parent;
                else return parent.GetParent(levels - 1);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Current level is first, highest parent is last.
        /// Tag Text will only match the name "Null"
        /// Tag Files will only match their given name
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        public bool MatchesHeirarchicalNaming(params String[] hits)
        {
            if (hits == null) throw new Exception("TagItem MatchesHeirarchicalNaming null input");
            if (hits.Length == 0) throw new Exception("TagItem MatchesHeirarchicalNaming 0 length input");

            if (this.MatchesName(hits[0]))
            {
                if (hits.Length == 1) return true;
                else
                {
                    TagFile parent;
                    if (this._Parent.TryGetTarget(out parent))
                    {
                        return parent.MatchesHeirarchicalNaming(hits.SubArray(1));
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

    }

}
